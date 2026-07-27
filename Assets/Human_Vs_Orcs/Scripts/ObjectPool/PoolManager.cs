using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonManager<PoolManager>
{
    private readonly Dictionary<GameObject, ObjectPool<Transform>> pools =
        new Dictionary<GameObject, ObjectPool<Transform>>();

    private readonly Dictionary<GameObject, ResourceType?> resourceLinks =
        new Dictionary<GameObject, ResourceType?>();

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int prewarmCount = 10;
        public bool tracksResource = false;
        public ResourceType resourceType;
    }

    [SerializeField]
    private List<PoolConfig> prewarmPools = new List<PoolConfig>();

    protected override void Awake()
    {
        base.Awake();
        foreach (var config in prewarmPools)
        {
            CreatePool(config.prefab, config.prewarmCount);

            resourceLinks[config.prefab] = config.tracksResource
                ? config.resourceType
                : (ResourceType?)null;
        }
    }

    private ObjectPool<Transform> CreatePool(GameObject prefab, int size)
    {
        var pool = new ObjectPool<Transform>(prefab.transform, size, transform);
        pools[prefab] = pool;
        return pool;
    }

    public bool TryGet<T>(GameObject prefab, Vector3 position, Quaternion rotation, out T component)
        where T : Component
    {
        component = null;

        if (!pools.TryGetValue(prefab, out var pool))
            return false;

        // 1) Check the resource FIRST, before touching the pool at all
        bool hasResourceLink =
            resourceLinks.TryGetValue(prefab, out var resType) && resType.HasValue;

        if (hasResourceLink && !ResourceManager.Instance.Spend(resType.Value, 1))
            return false; // not enough resource — pool is never touched

        // 2) Now try to actually get an instance from the pool
        if (!pool.TryGet(position, rotation, out Transform instanceTransform))
        {
            // pool failed for some other reason — refund what we just spent
            if (hasResourceLink)
                ResourceManager.Instance.Add(resType.Value, 1);
            return false;
        }

        // 3) Confirm the component we asked for actually exists on this prefab
        T comp = instanceTransform.GetComponent<T>();
        if (comp == null)
        {
            pool.Release(instanceTransform);
            if (hasResourceLink)
                ResourceManager.Instance.Add(resType.Value, 1);
            return false;
        }

        component = comp;
        return true;
    }

    public void Release(GameObject prefab, GameObject instance)
    {
        if (pools.TryGetValue(prefab, out var pool))
        {
            pool.Release(instance.transform);

            if (resourceLinks.TryGetValue(prefab, out var resType) && resType.HasValue)
                ResourceManager.Instance.Add(resType.Value, 1);
        }
        else
            Destroy(instance); // fallback safety net
    }
}
