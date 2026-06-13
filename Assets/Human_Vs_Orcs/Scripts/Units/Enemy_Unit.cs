using UnityEngine;

public class Enemy_Unit : Humanoid_Units
{
    [SerializeField]
    private LayerMask units;
    private float timer;
    public Transform target;
    private float attackCommitmentTime = 1f;
    private float currentAttackCommitmentTime = 0;

    protected override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        if (timer > objectDetectionInterval)
        {
            timer = 0;
            target = GetTarget(units);
        }
    }

    protected override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
        MoveNearUnits();
    }

    private void MoveNearUnits()
    {
        switch (CurrentState)
        {
            case UnitState.Idle:
            case UnitState.Moving:
                if (target != null)
                {
                    if (IsTargetInRange(target))
                    {
                        SetState(UnitState.Attacking);
                        StopMovement();
                    }
                    else
                    {
                        MoveTo(target.position);
                    }
                }
                break;

            case UnitState.Attacking:
                if (target != null)
                {
                    if (IsTargetInRange(target))
                    {
                        if (TryToAttackCurrentTarget())
                        {
                            currentAttackCommitmentTime = attackCommitmentTime;
                            PerformAttackAnimation(target);
                            StartCoroutine(
                                DelayDamage(autoAttackDamageDelay, autoAttackDamage, target)
                            );
                        }
                    }
                    else
                    {
                        currentAttackCommitmentTime -= Time.deltaTime;
                        if (currentAttackCommitmentTime <= 0)
                        {
                            SetState(UnitState.Moving);
                        }
                    }
                }
                else
                {
                    SetState(UnitState.Idle);
                }
                break;
        }
    }
}
