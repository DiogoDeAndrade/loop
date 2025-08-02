using NaughtyAttributes;
using UC;
using UnityEngine;

public class Enemy : Character
{
    protected enum TargetMode { AgroList, LineOfSight };
    protected enum State { Idle, Engaging, Search };

    [HorizontalLine(color: EColor.Red)]
    [SerializeField]
    private float       maxDetectionRadius = 500.0f;
    [SerializeField]
    private LayerMask   obstacleLayers;
    [SerializeField]
    private float       abilityTriggerCooldown = 0.0f;
    [SerializeField]
    private bool        aimIsPartOfAbility = false;
    [SerializeField]
    private TargetMode  targetMode;
    [SerializeField, ShowIf(nameof(isTargetLoS))]
    private float       maxChaseRadius = 200.0f;
    [SerializeField, ShowIf(nameof(isTargetLoS))]
    private float       maxSearchTime = 20.0f;

    protected AgroList      agroList;    
    protected RotateTowards rotationTowards;
    protected float         abilityTriggerTimer;

    protected State         state = State.Idle;
    protected Vector3       targetLastSeenPos;
    protected float         targetLastSeenTime;
    protected Character     currentTarget;

    bool isTargetLoS => targetMode == TargetMode.LineOfSight;

    protected override void Start()
    {
        base.Start();

        agroList = GetComponent<AgroList>();
        health.onChange += AddAgro;
        health.onResourceEmpty += RemoveFromAgro;

        rotationTowards = GetComponentInChildren<RotateTowards>();
        targetLastSeenPos = transform.position;
    }

    private void RemoveFromAgro(GameObject changeSource)
    {
        agroList?.SetAgro(changeSource, -float.MaxValue);
    }

    private void AddAgro(ResourceHandler.ChangeType changeType, float deltaValue, Vector3 changeSrcPosition, Vector3 changeSrcDirection, GameObject changeSource)
    {
        if (deltaValue < 0.0f)
            agroList?.AddAgro(changeSource, -deltaValue);
    }

    private void Update()
    {
        if (!isAlive) return;

        SelectTarget();

        if (currentTarget != null)
        {
            state = State.Engaging;
        }
        else
        {
            float elapsedSearchTime = Time.time - targetLastSeenTime;
            if (elapsedSearchTime > maxSearchTime)
            {
                state = State.Idle;
            }
            else
            {
                state = State.Search;
            }
        }

        switch (state)
        {
            case State.Idle:
                MoveTo(spawnPosition);
                break;
            case State.Engaging:
                TriggerAbilities();
                break;
            case State.Search:
                MoveTo(targetLastSeenPos);
                break;
            default:
                break;
        }        
    }

    void SelectTarget()
    {
        if (targetMode == TargetMode.AgroList)
        {
            if (currentTarget)
            {
                targetLastSeenTime = Time.time;
                targetLastSeenPos = currentTarget.transform.position;
            }

            var colliders = Physics2D.OverlapCircleAll(transform.position, maxDetectionRadius, ~0);
            foreach (var collider in colliders)
            {
                Character character = collider.GetComponent<Character>();
                if (character == null) continue;
                if (!character.isAlive) continue;
                if (character.faction.IsHostile(faction))
                {
                    // Check LoS
                    if (HasLoS(character))
                    { 
                        if (agroList.GetAgro(character.gameObject) < 100.0f)
                        {
                            agroList.SetAgro(character.gameObject, 100.0f);
                        }
                    }
                }
            }

            var agroTop = agroList.GetTop(IsCharacterAndAlive);
            if (agroTop != null)
            {
                currentTarget = agroTop.GetComponent<Character>();
                if (currentTarget)
                {
                    targetLastSeenTime = Time.time;
                    targetLastSeenPos = currentTarget.transform.position;
                }
            }
            else
            {
                currentTarget = null;
            }
        }
        else if (targetMode == TargetMode.LineOfSight)
        {
            if (currentTarget)
            {
                float dist = Vector3.Distance(currentTarget.GetTargetPosition(), transform.GetTargetPosition());
                if (dist < maxChaseRadius)
                {
                    if (HasLoS(currentTarget))
                    {
                        targetLastSeenPos = currentTarget.GetTargetPosition();
                        targetLastSeenTime = Time.time;
                    }
                    else
                    {
                        currentTarget = null;
                    }
                }
                else
                {
                    currentTarget = null;
                }
            }
            if (currentTarget == null)
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
                                currentTarget = character;
                                closestDist = distToCharacter;
                            }
                        }
                    }
                }
            }            
        }
    }

    void TriggerAbilities()
    {
        if (currentTarget)
        {
            headSpriteRenderer?.UpdateHead((currentTarget.GetTargetPosition() - transform.position).normalized);
        }

        abilityTriggerTimer -= Time.deltaTime;
        if ((currentTarget != null) && (abilityTriggerTimer <= 0.0f))
        {
            rotationTowards.target = currentTarget.transform;

            if (rotationTowards.rotationDistance < 5.0f)
            {
                // Trigger abilities
                for (int i = 0; i < abilities.Count; i++)
                {
                    var ability = abilities[i].ability;
                    if (ability == null) continue;
                    if (!ability.CanTrigger(currentTarget.GetTargetPosition())) continue;

                    switch (ability.triggerMode)
                    {
                        case Ability.TriggerMode.Single:
                            ability.Trigger(0.0f);
                            break;
                        case Ability.TriggerMode.Charge:
                            // Charge is not supported currently
                            break;
                        case Ability.TriggerMode.Continuous:
                            ability.Trigger(0.0f);
                            break;
                        default:
                            break;
                    }

                    abilityTriggerTimer = abilityTriggerCooldown;
                }
            }
        }
        else
        {
            if ((aimIsPartOfAbility) || (currentTarget == null))
                rotationTowards.target = null;
            else if (currentTarget != null)
                rotationTowards.target = currentTarget.transform;
        }
    }

    private bool IsCharacterAndAlive(GameObject target, float value)
    {
        var character = target.GetComponent<Character>();
        if (character == null) return false;
        return character.isAlive;
    }

    bool HasLoS(Character character)
    {
        Vector2 dir = character.transform.GetTargetPosition() - transform.GetTargetPosition();
        var hit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude, obstacleLayers);
        return (hit.collider == null);
    }

    bool MoveTo(Vector3 targetPosition)
    {
        float maxDisp = characterType.maxSpeed * Time.deltaTime;

        Vector3 moveDir = (targetPosition - transform.position);
        if (moveDir.magnitude < maxDisp)
        {
            rb.linearVelocity = Vector2.zero;
            return false;
        }

        moveDir.Normalize();

        rb.linearVelocity = moveDir * characterType.maxSpeed;

        headSpriteRenderer?.UpdateHead(moveDir);

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDetectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxChaseRadius);

        if ((state == State.Idle) || (state == State.Search))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetLastSeenPos);
        }
    }
}
