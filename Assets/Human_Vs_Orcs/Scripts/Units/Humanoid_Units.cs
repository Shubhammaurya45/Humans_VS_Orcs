using UnityEngine;

public class Humanoid_Units : Units
{
    protected Vector2 veloctiy;
    protected Vector3 lastPostion;

    public float CurrentSpeed => veloctiy.magnitude;

    protected virtual void Update()
    {
        UpdateBehaviour();
        UpdateVelocity();
    }

    private void UpdateVelocity()
    {
        veloctiy =
            new Vector2(
                (transform.position.x - lastPostion.x),
                (transform.position.y - lastPostion.y)
            ) / Time.deltaTime;
        lastPostion = transform.position;
        if (CurrentState != UnitState.Attacking)
        {
            var state = veloctiy.magnitude > 0 ? UnitState.Moving : UnitState.Idle;
            SetState(state);
        }
        Animator.SetFloat("Speed", Mathf.Clamp01(CurrentSpeed));
    }

    protected virtual void UpdateBehaviour()
    {
        //
    }

    protected override void PerformAttackAnimation(Transform target)
    {
        base.PerformAttackAnimation(target);
        Vector3 direction = (target.position - transform.position).normalized;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            spriteRenderer.flipX = direction.x < 0;
            Animator.SetTrigger("AttackHorizontal");
        }
        else
        {
            Animator.SetTrigger(direction.y > 0 ? "AttackUP" : "AttackDown");
        }
    }
}
