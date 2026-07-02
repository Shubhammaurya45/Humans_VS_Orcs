using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Projectile_Unit projectile_Unit;

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private int damage = 10;

    private Units target;

    void Update()
    {
        if (target == null || target.CurrentState == UnitState.Dead)
        {
            return;
        }

        var direction = (target.transform.position - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hi");
        if (collision.TryGetComponent<Units>(out var targetUnit))
        {
            if (targetUnit == target)
            {
                targetUnit.TakeDamage(damage, targetUnit);

                projectile_Unit.ProjectilePool.Release(projectile_Unit.Projectile_Object);
            }
        }
    }

    public void Initialize(Units target, Projectile_Unit projectile_Unit)
    {
        this.target = target;
        this.projectile_Unit = projectile_Unit;
    }
}
