using System.Collections;
using UnityEngine;

public enum UnitState
{
    Idle,
    Moving,
    Attacking,
    Chopping,
    Mining,
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
    [Header("Materials")]
    [SerializeField]
    private Material originalMaterial;

    [SerializeField]
    private Material outlineMaterail;

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

    // public Units Target { get; protected set; }

    public bool isTargeted;

    [Header("Object Detection Details")]
    [SerializeField]
    private float objectDetectionRadius;

    [SerializeField]
    private float objectAttackRange;
    protected float objectDetectionInterval = 2f;
    private float autoAttackFrequency = 1.5f;
    protected float nextAutoAttack;

    [Header("Attack Details")]
    [SerializeField]
    protected float autoAttackDamageDelay = 0.5f;

    [SerializeField]
    protected int autoAttackDamage = 7;
    protected CapsuleCollider2D targetCollider;

    protected void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        ai_Pawns = GetComponent<AI_Pawns>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, objectDetectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, objectAttackRange);
    }

    //------------------------ Detect Near Object -----------------------------
    private Collider2D[] DetectNearbyObject(LayerMask layerMask)
    {
        return Physics2D.OverlapCircleAll(transform.position, objectDetectionRadius, layerMask);
    }

    public Transform GetClosestObject(Vector2 origin, LayerMask layerMask)
    {
        Collider2D[] hits = DetectNearbyObject(layerMask);
        Transform closet = null;
        float minObjectDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float objectDistance = Vector2.Distance(origin, hit.transform.position);
            if (objectDistance < minObjectDistance)
            {
                minObjectDistance = objectDistance;
                closet = hit.transform;
            }
        }
        return closet;
    }

    //------------------------ Attack ----------------------------------------
    public Transform GetTarget(LayerMask layerMask)
    {
        return GetClosestObject(transform.position, layerMask);
    }

    public bool IsTargetInRange(Transform target)
    {
        return Vector3.Distance(target.transform.position, transform.position) <= objectAttackRange;
    }

    public bool TryToAttackCurrentTarget()
    {
        if (Time.time >= nextAutoAttack)
        {
            Debug.Log("Attack");
            nextAutoAttack = Time.time + autoAttackFrequency;
            return true;
        }
        Debug.Log("Attack at CD");
        return false;
    }

    //------------------------------------ Damage -----------------------------
    protected IEnumerator DelayDamage(float delay, int damage, Transform target)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            TakeDamage(damage, target);
        }
    }

    protected virtual void TakeDamage(int damage, Transform damager)
    {
        targetCollider = damager.GetComponent<CapsuleCollider2D>();
        UIManager.Instance.ShowTextPopup(damage.ToString(), GetTopPostion(damager), Color.red);
    }

    protected virtual void PerformAttackAnimation(Transform target)
    {
        //
    }

    //------------------------------TextPopup---------------------------------
    private Vector3 GetTopPostion(Transform target)
    {
        if (targetCollider == null)
            return target.position;
        return target.position + Vector3.up * targetCollider.size.y / 2;
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
        OnSetState(CurrentState, state);
    }

    public void SetTask(UnitTask task, Animator animator)
    {
        OnSetTask(CurrentTask, task);
        SetAnimation(animator);
    }

    private void OnSetState(UnitState oldState, UnitState newState)
    {
        CurrentState = newState;
    }

    private void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
        CurrentTask = newTask;

        Debug.Log(CurrentTask);
    }

    public void SetAnimation(Animator animator)
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
