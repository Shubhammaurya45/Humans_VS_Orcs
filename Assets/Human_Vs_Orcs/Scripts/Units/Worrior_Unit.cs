using UnityEngine;

public class Worrior_Unit : Humanoid_Units
{
    [SerializeField]
    private LayerMask enemy;
    private float timer;

    protected override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        if (timer > objectDetectionInterval)
        {
            timer = 0;
            Transform target = GetTarget(enemy);
        }
    }
}
