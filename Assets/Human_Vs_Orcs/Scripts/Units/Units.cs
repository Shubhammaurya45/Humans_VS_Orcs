using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

public abstract class Units : MonoBehaviour
{
    [Header("Layer_Mask")]
    [SerializeField]
    private LayerMask enemyLayerMask;

    [SerializeField]
    private LayerMask alliesLayerMask;

    //-------------------------------------------------------
    [Header("Materials")]
    [SerializeField]
    private Material outlineMaterail;
    private Material originalMaterial;

    //============================================================
    [SerializeField]
    private ActionSO[] actions;

    protected SpriteRenderer spriteRenderer;
    private Animator animator;
    private AI_Pawns ai_Pawns;
    public ActionSO[] Actions => actions;
    public Animator Animator => animator;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    public UnitState CurrentState { get; protected set; }
    public UnitTask CurrentTask { get; protected set; }

    //========================================================================
    [Header("Object Detection Details")]
    [SerializeField]
    private float objectDetectionRadius;

    //=======================================================================
    [Header("Attack Details")]
    protected CapsuleCollider2D targetCollider;

    [SerializeField]
    protected float autoAttackDamageDelay = 0.5f;

    [SerializeField]
    protected int autoAttackDamage = 7;

    [SerializeField]
    private float objectAttackRange;

    [SerializeField]
    private int totalHealth = 100;

    [SerializeField]
    protected Color damageFlashColor = new Color(1f, 0.27f, 0.25f, 1f);

    private readonly List<Collider2D> hitResults = new List<Collider2D>();
    private ContactFilter2D contactFilter;

    public bool isTargeted;
    protected float objectDetectionInterval = 0.5f;
    private float autoAttackFrequency = 1.5f;
    protected float nextAutoAttack;
    public float currentHealth;

    public float CurrentHealth => currentHealth;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ai_Pawns = GetComponent<AI_Pawns>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(enemyLayerMask);
        contactFilter.useTriggers = false;
        currentHealth = totalHealth;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, objectDetectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, objectAttackRange);
    }

    //------------------------ Detect Near Object -----------------------------
    private int DetectNearbyObject()
    {
        return Physics2D.OverlapCircle(
            transform.position,
            objectDetectionRadius,
            contactFilter,
            hitResults
        );
    }

    public Units GetClosestObject(Vector2 originPosition)
    {
        int totalHits = DetectNearbyObject();
        Units closet = null;
        float minObjectDistance = Mathf.Infinity;

        for (int i = 0; i < totalHits; i++)
        {
            if (CurrentState == UnitState.Dead)
                continue;

            float objectDistance = Vector2.Distance(
                originPosition,
                hitResults[i].transform.position
            );
            if (objectDistance < minObjectDistance)
            {
                minObjectDistance = objectDistance;
                closet = hitResults[i].GetComponent<Units>();
            }
        }
        return closet;
    }

    //------------------------ Attack ----------------------------------------
    public Units GetTarget()
    {
        var currentTarget = GetClosestObject(transform.position);
        targetCollider = currentTarget?.GetComponent<CapsuleCollider2D>();
        return currentTarget;
    }

    public bool IsTargetInRange(Vector3 targetPosition)
    {
        return Vector3.Distance(targetPosition, transform.position) <= objectAttackRange;
    }

    public bool TryToAttackCurrentTarget(Units target)
    {
        if (target.CurrentState == UnitState.Dead)
            return false;

        if (Time.time >= nextAutoAttack)
        {
            nextAutoAttack = Time.time + autoAttackFrequency;
            return true;
        }
        return false;
    }

    //------------------------------------ Damage -----------------------------
    protected IEnumerator DelayDamage(float delay, int damage, Units target)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            TakeDamage(damage, target);
        }
    }

    protected virtual void TakeDamage(int damage, Units target)
    {
        if (CurrentState == UnitState.Dead)
            return;

        target.currentHealth -= damage;
        Debug.Log(target.currentHealth);

        StartCoroutine(FlashEffect(target, 0.2f, 2, damageFlashColor));
    }

    protected IEnumerator FlashEffect(Units target, float duration, int flashCount, Color color)
    {
        Color originalColor = target.spriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            target.spriteRenderer.color = color;
            yield return new WaitForSeconds(duration / 2f);

            target.spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(duration / 2f);
        }
    }

    //------------------- Movement -------------------------------------------
    public void MoveTo(Vector3 destination)
    {
        var direction = (destination - transform.position).normalized;
        spriteRenderer.flipX = direction.x < 0;
        ai_Pawns.SetDistination(destination);
    }

    protected void StopMovement()
    {
        ai_Pawns.Stop();
    }

    //------------------------Highlight--------------------
    //Highlight materials which get selected
    void Highlight()
    {
        spriteRenderer.material = outlineMaterail;
    }

    //UnHighlight materials when get unselected
    void UnHighlight()
    {
        spriteRenderer.material = originalMaterial;
    }

    // ------------------------Active Unit -----------------------

    //Select unit
    public void Select()
    {
        Highlight();
        isTargeted = true;
    }

    //Unselect unit
    public void Deselect()
    {
        UnHighlight();
        isTargeted = false;
    }

    public void SetState(UnitState state)
    {
        ApplyState(CurrentState, state);
    }

    public void SetTask(UnitTask task)
    {
        ApplyTask(CurrentTask, task);
        SetAnimation();
    }

    private void ApplyState(UnitState oldState, UnitState newState)
    {
        CurrentState = newState;
    }

    private void ApplyTask(UnitTask oldTask, UnitTask newTask)
    {
        CurrentTask = newTask;
    }

    public void SetAnimation()
    {
        switch (CurrentTask)
        {
            case UnitTask.Build:
                animator.SetBool("Build", true);
                break;
            case UnitTask.Chop:
                animator.SetBool("Chop", true);
                break;
            default:
                animator.SetBool("Idle", true);
                animator.SetBool("Chop", false);
                animator.SetBool("Build", false);
                break;
        }
    }
}
