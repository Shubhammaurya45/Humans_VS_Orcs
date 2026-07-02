using System.Collections.Generic;
using UnityEngine;

public class UnitSeparator : MonoBehaviour
{
    /// <summary>
    /// Prevents units from stacking by applying a push force when
    /// another unit enters the separation radius.
    /// Add to every unit prefab (Enemy_Unit, Worker_Unit, warriors).
    /// Each unit also needs a CircleCollider2D (IsTrigger = true) so
    /// other units can detect it via Physics2D.
    /// </summary>
    [Header("Detection")]
    [SerializeField]
    private float separationRadius = 0.6f;

    [SerializeField]
    private LayerMask unitLayer;

    [Header("Push force")]
    [SerializeField]
    private float separationStrength = 2.5f;

    // Reused every frame instead of allocating — replaces the old NonAlloc array
    private ContactFilter2D _filter;
    private readonly List<Collider2D> _results = new List<Collider2D>(16);

    private void Awake()
    {
        _filter = new ContactFilter2D();
        _filter.SetLayerMask(unitLayer);
        _filter.useTriggers = true; // unit colliders are triggers, so this must be true
    }

    private void Update()
    {
        if (DetectNearFriend() != null)
        {
            var friends = DetectNearFriend();
            var friendsDistance = Vector3.Distance(transform.position, friends.transform.position);
            if (friendsDistance <= separationRadius)
            {
                Vector2 force = CalculateSeparationForce();
                if (force == Vector2.zero)
                    return;
                transform.position += (Vector3)(force * Time.deltaTime);
            }
        }
    }

    private Collider2D DetectNearFriend()
    {
        return Physics2D.OverlapCircle(transform.position, separationRadius, unitLayer);
    }

    private Vector2 CalculateSeparationForce()
    {
        _results.Clear();
        int count = Physics2D.OverlapCircle(
            transform.position,
            separationRadius,
            _filter,
            _results
        );

        Vector2 totalForce = Vector2.zero;
        for (int i = 0; i < count; i++)
        {
            if (_results[i].gameObject == gameObject)
                continue; // skip self

            Vector2 awayDirection = (Vector2)(transform.position - _results[i].transform.position);
            float distance = awayDirection.magnitude;

            if (distance < 0.001f)
            {
                awayDirection = Random.insideUnitCircle.normalized;
                distance = 0.001f;
            }

            float pushStrength = (1f - distance / separationRadius) * separationStrength;
            totalForce += awayDirection.normalized * pushStrength;
        }

        return totalForce;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.85f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
