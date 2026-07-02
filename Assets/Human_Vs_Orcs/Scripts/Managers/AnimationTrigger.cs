using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    private Projectile_Unit projectile_Unit;

    private void Start()
    {
        projectile_Unit = FindAnyObjectByType<Projectile_Unit>();
    }

    public void SpawnArrow()
    {
        projectile_Unit.GetProjectile();
    }
}
