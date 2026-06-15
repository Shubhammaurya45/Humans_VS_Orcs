using UnityEngine;

public class Worrior_Unit : Humanoid_Units
{
    private float timer;

    protected override void Update()
    {
        base.Update();
        //timer += Time.deltaTime;
        //if (timer > objectDetectionInterval)
        //{
        //    timer = 0;
        //    Transform target = GetTarget();
        //    //SetTarget(target);
        //}
    }
}
