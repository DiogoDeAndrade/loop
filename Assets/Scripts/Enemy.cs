using NaughtyAttributes;
using UC;
using UnityEngine;

public class Enemy : Character
{
    protected enum TargetMode { AgroList, LineOfSight };
    public enum State { Idle, Engaging, Search };

    [HorizontalLine(color: EColor.Red)]
    [SerializeField]
    private LayerMask   obstacleLayers;
    [SerializeField]
    private float       abilityTriggerCooldown = 0.0f;
    [SerializeField]
    private bool        aimIsPartOfAbility = false;
    [SerializeField]
    private TargetMode  targetMode;

    protected AgroList      agroList;    
    protected RotateTowards rotationTowards;
    protected float         abilityTriggerTimer;

    protected State             _state = State.Idle;
    protected TargetSelection   targetSelection;

    public State state => _state;

    protected override void Start()
    {
        base.Start();

        agroList = GetComponent<AgroList>();
        health.onChange += AddAgro;
        health.onResourceEmpty += RemoveFromAgro;

        rotationTowards = GetComponentInChildren<RotateTowards>();
        targetSelection = GetComponent<TargetSelection>();
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

        targetSelection.UpdateSelection();

        var currentTarget = targetSelection.currentTarget;

        if (currentTarget != null)
        {
            _state = State.Engaging;
        }
        else
        {
            if (targetSelection.GetLastSeen(out Vector3 lastSeen))
            {
                _state = State.Search;
            }
            else
            {
                _state = State.Idle;
            }
        }

        switch (_state)
        {
            case State.Idle:
                MoveTo(spawnPosition);
                break;
            case State.Engaging:
                TriggerAbilities();
                break;
            case State.Search:
                if (targetSelection.GetLastSeen(out Vector3 lastSeen))
                {
                    MoveTo(lastSeen);
                }
                break;
            default:
                break;
        }        
    }

    void TriggerAbilities()
    {
        var currentTarget = targetSelection.currentTarget;
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

}
