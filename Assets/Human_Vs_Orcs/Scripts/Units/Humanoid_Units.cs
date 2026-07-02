using System.Collections;
using UnityEngine;

public enum UnitState
{
    Idle,
    Moving,
    Attacking,
    Chopping,
    Mining,
    Dead,
}

public enum UnitTask
{
    None,
    Build,
    Attack,
    Chop,
    Mine,
}

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
        //UpdateVelocity();
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

    //================================ AttackAnimation ==============================

    protected virtual void PerformAttackAnimation(Units target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            SpriteRenderer.flipX = direction.x < 0;
            Debug.Log(Animator);
            Animator.SetTrigger("AttackHorizontal");
        }
        else
        {
            Animator.SetTrigger(direction.y > 0 ? "AttackUP" : "AttackDown");
        }
    }

    public override void TakeDamage(int damage, Units target)
    {
        base.TakeDamage(damage, target);
        var targetType = target.GetComponent<Humanoid_Units>();
        if (targetType != null)
        {
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
        else if (target.gameObject.layer == LayerMask.NameToLayer("Building"))
        {
            Debug.Log(target.gameObject.layer);
            if (target.currentHealth <= 0)
            {
                DestroyBuilding(target);
            }
        }
    }

    protected virtual void Die(Units target)
    {
        Debug.Log("Unit is Dead");
        target.SetState(UnitState.Dead);
        RunDeadEffect(target);
        target = null;
    }

    private void DestroyBuilding(Units target)
    {
        var spriteRender = target.GetComponentInChildren<SpriteRenderer>();
        Debug.Log(spriteRender);
        target.HideHealthBar();
        spriteRender.sprite = BuildManager.Instance.buildAction.BuildingDestroyed;
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
        if (target != null)
        {
            Destroy(target.gameObject);
        }
    }
}
