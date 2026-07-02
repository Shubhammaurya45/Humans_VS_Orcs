using UnityEngine;

public abstract class ActionSO : ScriptableObject
{
    public Sprite buildingIcon;
    public string buildingName;
    public string Guid = System.Guid.NewGuid().ToString();
    public abstract void Execute();
}
