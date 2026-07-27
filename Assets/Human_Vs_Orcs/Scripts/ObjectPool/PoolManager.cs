using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;

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
        if (!pools.TryGetValue(prefab, out var pool))
        {
            component = null;
            return false;
        }

        if (!pool.TryGet(position, rotation, out Transform instanceTransform))
        {
            component = null;
            return false;
        }

        // Try to get the requested component from the pooled Transform
        T comp = instanceTransform.GetComponent<T>();
        if (comp == null)
        {
            // requested component not present on this prefab instance: return it to the pool
            pool.Release(instanceTransform);
            component = null;
            return false;
        }

        component = comp;

        if (resourceLinks.TryGetValue(prefab, out var resType) && resType.HasValue)
            ResourceManager.Instance.Spend(resType.Value, 1);

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
