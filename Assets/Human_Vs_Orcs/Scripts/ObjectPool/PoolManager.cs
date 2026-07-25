using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonManager<PoolManager>
{
    private readonly Dictionary<GameObject, ObjectPool<Transform>> pools =
        new Dictionary<GameObject, ObjectPool<Transform>>();

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int prewarmCount = 10;
    }

    [SerializeField]
    private List<PoolConfig> prewarmPools = new List<PoolConfig>();

    protected override void Awake()
    {
        base.Awake();
        foreach (var config in prewarmPools)
            CreatePool(config.prefab, config.prewarmCount);
    }

    private ObjectPool<Transform> CreatePool(GameObject prefab, int size)
    {
        var pool = new ObjectPool<Transform>(prefab.transform, size, transform);
        pools[prefab] = pool;
        return pool;
    }

    //public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    //{
    //    if (!_pools.TryGetValue(prefab, out var pool))
    //        pool = CreatePool(prefab, 0);

    //    return pool.TryGet(position, rotation).gameObject;
    //}

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
        return true;
    }

    public void Release(GameObject prefab, GameObject instance)
    {
        if (pools.TryGetValue(prefab, out var pool))
            pool.Release(instance.transform);
        else
            Destroy(instance); // fallback safety net
    }
}
