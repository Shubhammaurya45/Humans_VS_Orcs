using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public enum ArcherAnimation
{
    Idle,
    Run,
    AttackUP_1,
    AttackUp_2,
    AttackHorizontal,
    AttackDown_1,
    AttackDown_2,
}

public class Projectile_Unit : Humanoid_Units
{
    [SerializeField]
    private Projectile projectilePrefab;

    [SerializeField]
    private Transform projectilePoolParent;

    private ObjectPool<Projectile> projectilePool;
    private Projectile projectile;
    private float attackTimer;

    [SerializeField]
    private float attackInterval;

    public Units target;
    private float targetDistance;

    private float timer;

    public ObjectPool<Projectile> ProjectilePool => projectilePool;
    public Projectile Projectile_Object => projectile;

    private void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(
            createFunc: () => Instantiate(projectilePrefab, projectilePoolParent), // only when pool is empty
            actionOnGet: unit => unit.gameObject.SetActive(true), // on borrow
            actionOnRelease: unit => unit.gameObject.SetActive(false), // on return
            actionOnDestroy: unit =>
            {
                if (unit != null)
                {
                    Destroy(unit.gameObject);
                }
            }, // if pool overflows
            defaultCapacity: 3,
            maxSize: 10
        );
    }

    protected override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        if (timer > objectDetectionInterval)
        {
            timer = 0;
            target = GetTarget();
            if (target != null)
            {
                targetDistance = Vector3.Distance(transform.position, target.transform.position);
                Debug.Log(targetDistance);
            }
        }
    }

    protected override void UpdateBehaviour()
    {
        base.UpdateBehaviour();

        if (target != null)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attackInterval)
            {
                SetState(UnitState.Attacking);
                StartAttack();
                //GetProjectile();
                attackTimer = 0;
            }
        }
    }

    //Initialize from animation trigger
    public void GetProjectile()
    {
        projectile = projectilePool.Get();
        projectile.transform.position = this.transform.position;
        projectile.Initialize(target, this);
    }

    private void StartAttack()
    {
        switch (CurrentState)
        {
            case UnitState.Idle:
                break;

            case UnitState.Moving:
                break;

            case UnitState.Attacking:
                if (target != null)
                {
                    if (
                        TryToAttackCurrentTarget(target)
                        && IsTargetInRange(target.transform.position)
                    )
                    {
                        PerformAttackAnimation(target);
                    }
                }
                else if (target.CurrentState == UnitState.Dead)
                {
                    SetState(UnitState.Idle);
                }
                break;
        }
    }
}
