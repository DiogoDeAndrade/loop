using UC;
using UnityEngine;

public class TargetSelection_LineOfSight : TargetSelection
{
    [SerializeField]
    private float maxChaseRadius = 200.0f;

    private void OnEnable()
    {
        var health = this.FindResourceHandler(Globals.healthResource);
        if (health) health.onChange += SetTargetIfHit;
    }

    private void OnDisable()
    {
        var health = this.FindResourceHandler(Globals.healthResource);
        if (health) health.onChange -= SetTargetIfHit;
    }

    private void SetTargetIfHit(ResourceHandler.ChangeType changeType, float deltaValue, Vector3 changeSrcPosition, Vector3 changeSrcDirection, GameObject changeSource)
    {
        if (_currentTarget) return;

        if (deltaValue < 0)
        {
            _currentTarget = changeSource.GetComponent<Character>();
        }
    }

    public override void UpdateSelection()
    {
        if (_currentTarget)
        {
            if (_currentTarget.isAlive)
            {
                float dist = Vector3.Distance(_currentTarget.GetTargetPosition(), transform.GetTargetPosition());
                if (dist < maxChaseRadius)
                {
                    if (HasLoS(_currentTarget))
                    {
                        _targetLastSeenPos = _currentTarget.GetTargetPosition();
                        _targetLastSeenTime = Time.time;
                    }
                    else
                    {
                        _currentTarget = null;
                    }
                }
                else
                {
                    _currentTarget = null;
                }
            }
            else
            {
                _currentTarget = null;
            }
        }
        if (_currentTarget == null)
        {
            // Find the closest target in range and in LoS
            float closestDist = float.MaxValue;
            var colliders = Physics2D.OverlapCircleAll(transform.position, maxDetectionRadius, ~0);
            foreach (var collider in colliders)
            {
                Character character = collider.GetComponent<Character>();
                if (character == null) continue;
                if (!character.isAlive) continue;
                if (character.faction.IsHostile(faction))
                {
                    float distToCharacter = Vector3.Distance(character.transform.position, transform.position);
                    if (distToCharacter < closestDist)
                    {
                        // Check LoS
                        if (HasLoS(character))
                        {
                            _currentTarget = character;
                            closestDist = distToCharacter;
                        }
                    }
                }
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxChaseRadius);
    }
}
