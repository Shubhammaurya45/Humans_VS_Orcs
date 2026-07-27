using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
    where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> available = new Queue<T>();
    private readonly HashSet<T> active = new HashSet<T>();

    public int AvailableCount => available.Count;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateNew();
            obj.gameObject.SetActive(false);
            available.Enqueue(obj);
        }
    }

    private T CreateNew()
    {
        return Object.Instantiate(prefab, parent);
    }

    public bool TryGet(Vector3 position, Quaternion rotation, out T obj)
    {
        if (available.Count == 0)
        {
            obj = null;
            return false;
        }

        obj = available.Dequeue();
        Activate(obj, position, rotation);
        return true;
    }

    private void Activate(T obj, Vector3 position, Quaternion rotation)
    {
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
        active.Add(obj);
    }

    public void Release(T obj)
    {
        if (!active.Contains(obj))
            return; // guards against double-release

        active.Remove(obj);

        obj.gameObject.SetActive(false);
        available.Enqueue(obj);
    }

    public void ReleaseAll()
    {
        foreach (var obj in new List<T>(active))
            Release(obj);
    }
}
