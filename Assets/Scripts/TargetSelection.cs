using UC;
using UnityEngine;

public class TargetSelection : MonoBehaviour
{
    [SerializeField]
    protected float     maxDetectionRadius = 500.0f;
    [SerializeField]
    private float       maxSearchTime = 20.0f;
    [SerializeField] 
    protected LayerMask obstacleLayers;

    protected Vector3   _targetLastSeenPos;
    protected float     _targetLastSeenTime;
    protected Character _currentTarget;
    protected Character _prevTarget;

    protected Enemy     ownerEnemy;

    protected Faction faction => ownerEnemy.faction;

    public Character currentTarget => _currentTarget;

    protected virtual void Start()
    {
        ownerEnemy = GetComponent<Enemy>();
        _targetLastSeenPos = transform.position;

    }

    public virtual void UpdateSelection()
    {

    }

    public virtual bool GetLastSeen(out Vector3 pos)
    {
        pos = _targetLastSeenPos;

        if ((_prevTarget != null) && (!_prevTarget.isAlive)) return false;

        return (Time.time - _targetLastSeenTime < maxSearchTime);
    }

    protected bool HasLoS(Character character)
    {
        Vector2 dir = character.transform.GetTargetPosition() - transform.GetTargetPosition();
        var hit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude, obstacleLayers);
        return (hit.collider == null);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDetectionRadius);

        if ((Application.isPlaying) && (ownerEnemy != null))
        {
            if ((ownerEnemy.state == Enemy.State.Idle) || (ownerEnemy.state == Enemy.State.Search))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _targetLastSeenPos);
            }
        }
    }

}
