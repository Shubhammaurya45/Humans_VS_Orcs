using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum ResourceType
{
    Coins,
    Wood,
    Workers,
    Worrior,
    Archer,
}

public class ResourceManager : SingletonManager<ResourceManager>
{
    [Serializable]
    public class ResourceEntry
    {
        public ResourceType type;
        public int startValue;
    }

    [SerializeField]
    private List<ResourceEntry> startingResources = new();

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    // Subscribe to this from UI: (type, newValue)
    public event Action<ResourceType, int> OnResourceChanged;
    public event Action<ResourceType> OnInsufficientResource;

    protected override void Awake()
    {
        base.Awake();

        foreach (var entry in startingResources)
        {
            resources[entry.type] = entry.startValue;
        }
    }

    public int Get(ResourceType type)
    {
        return resources.TryGetValue(type, out int value) ? value : 0;
    }

    public void Add(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0;

        resources[type] += amount;
        OnResourceChanged?.Invoke(type, resources[type]);
    }

    public bool Spend(ResourceType type, int amount)
    {
        if (Get(type) < amount)
        {
            OnInsufficientResource?.Invoke(type);
            return false;
        }

        resources[type] -= amount;
        OnResourceChanged?.Invoke(type, resources[type]);
        return true;
    }

    public void Set(ResourceType type, int amount)
    {
        resources[type] = amount;
        OnResourceChanged?.Invoke(type, resources[type]);
    }
}
