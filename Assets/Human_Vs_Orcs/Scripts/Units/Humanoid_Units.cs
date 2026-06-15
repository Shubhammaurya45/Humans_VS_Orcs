using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Humanoid_Units : Units
{
    protected Vector2 veloctiy;
    protected Vector3 lastPostion;

    public float CurrentSpeed => veloctiy.magnitude;

    protected virtual void Update()
    {
        if (CurrentState == UnitState.Dead)
            return;

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

    //------------------------------TextPopup---------------------------------
    private Vector3 GetTopPosition(Vector3 targetPosition)
    {
        if (targetCollider == null)
            return targetPosition;

        return targetPosition + Vector3.up * targetCollider.size.y / 2;
    }

    protected void PerformAttackAnimation(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
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

    protected override void TakeDamage(int damage, Units target)
    {
        base.TakeDamage(damage, target);
        UIManager.Instance.ShowTextPopup(
            damage.ToString(),
            GetTopPosition(target.transform.position),
            Color.red
        );
        if (target.currentHealth <= 0)
        {
            Die(target);
        }
    }

    protected virtual void Die(Units target)
    {
        Debug.Log("Unit is Dead");
        target.SetState(UnitState.Dead);
        RunDeadEffect(target);
        target = null;
    }

    protected void RunDeadEffect(Units target)
    {
        target.Animator.SetTrigger("Dead");
        StartCoroutine(LateObjectDestroy(1.2f, target));
    }

    // Destroy gameobject after some time
    private IEnumerator LateObjectDestroy(float delay, Units target)
    {
        yield return new WaitForSeconds(delay);
        Destroy(target.gameObject);
    }
}
