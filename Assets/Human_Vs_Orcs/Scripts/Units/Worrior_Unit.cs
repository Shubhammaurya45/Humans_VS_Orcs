using UnityEngine;

public class Worrior_Unit : Humanoid_Units
{
    private float timer;

    public Units target;
    private float attackCommitmentTime = 1f;
    private float currentAttackCommitmentTime = 0;
    private Vector3 lastKnownTargetPos = new Vector3();

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

                timer += Time.deltaTime;
                if (timer > objectDetectionInterval)
                {
                    timer = 0;
                    target = GetTarget();
                }

                if (target != null)
                {
                    MoveTo(target.transform.position);
                    SetState(UnitState.Moving);
                }

                break;

            case UnitState.Moving:
                if (target != null)
                {
                    if (IsTargetInRange(target.transform.position))
                    {
                        SetState(UnitState.Attacking);
                        StopMovement();
                    }
                    else if (CheckTargetPosition())
                    {
                        MoveTo(target.transform.position);
                    }
                }
                else
                    SetState(UnitState.Idle);

                break;

            case UnitState.Attacking:
                if (target != null)
                {
                    if (IsTargetInRange(target.transform.position))
                    {
                        if (TryToAttackCurrentTarget(target))
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

    private bool CheckTargetPosition()
    {
        if (target == null)
            return false;

        if (Vector2.Distance(lastKnownTargetPos, target.transform.position) > 0.5f)
        {
            lastKnownTargetPos = target.transform.position;
            return true;
        }

        return false;
    }
}
