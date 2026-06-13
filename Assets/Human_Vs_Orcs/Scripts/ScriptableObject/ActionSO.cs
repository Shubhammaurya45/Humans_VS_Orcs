using UnityEngine;

public abstract class ActionSO : ScriptableObject
{
    public Sprite icon;
    public string actionName;
    public string Guid=System.Guid.NewGuid().ToString();
    public abstract void Execute();
}
