using NaughtyAttributes;
using UC;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : Character
{
    protected enum TargetMode { AgroList, LineOfSight };
    public enum State { Idle, Patrol, Engaging, GotoLastSeen, Search };

    [HorizontalLine(color: EColor.Red)]
    [SerializeField]
    private float       abilityTriggerCooldown = 0.0f;
    [SerializeField]
    private bool        aimIsPartOfAbility = false;
    [SerializeField]
    private bool        enablePatrol = false;
    [SerializeField, ShowIf(nameof(enablePatrol))]
    private float       patrolRadius;
    [SerializeField]
    private float       maxSearchTime = 20.0f;
    [SerializeField]
    private float       searchRadius = 100.0f;

    protected AgroList      agroList;    
    protected RotateTowards rotationTowards;
    protected float         abilityTriggerTimer;

    protected State             _state = State.Idle;
    protected float             lastStateTransitionTime = -float.MaxValue; 
    protected TargetSelection   targetSelection;
    protected Vector2           searchPivot;

    protected NavMeshAgent2d    agent;

    public State state => _state;

    protected override void Start()
    {
        base.Start();

        agroList = GetComponent<AgroList>();

        rotationTowards = GetComponentInChildren<RotateTowards>();
        targetSelection = GetComponent<TargetSelection>();

        agent = GetComponent<NavMeshAgent2d>();
        agent.SetMaxSpeed(characterType.maxSpeed);
    }


    private void Update()
    {
        if (!isAlive) return;

        targetSelection.UpdateSelection();

        var currentTarget = targetSelection.currentTarget;

        switch (_state)
        {
            case State.Idle:
                if (currentTarget)
                {
                    SetState(State.Engaging);
                }
                else
                {
                    agent.SetDestination(spawnPosition);

                    if (!agent.isMoving)
                    {
                        if (enablePatrol)
                        {
                            SetState(State.Patrol);
                        }
                    }
                }
                break;
            case State.Patrol:
                if (currentTarget)
                {
                    SetState(State.Engaging);
                }
                else
                {
                    if (!agent.isMoving)
                    {
                        Vector2 newTargetPos = spawnPosition + Random.insideUnitCircle * patrolRadius;
                        agent.SetDestination(newTargetPos);
                    }
                }
                break;
            case State.Engaging:
                if (currentTarget)
                {
                    TriggerAbilities();
                }
                else
                {
                    SetState(State.GotoLastSeen);
                }
                break;
            case State.GotoLastSeen:
                if (targetSelection.GetLastSeen(out Vector3 lastSeen))
                {
                    agent.SetDestination(lastSeen);

                    if (!agent.isMoving)
                    {
                        SetState(State.Search);
                        searchPivot = transform.position;
                    }
                }
                else
                {
                    SetState(State.Idle);
                }
                    break;
            case State.Search:
                {
                    if (stateElapsedTime > maxSearchTime)
                    {
                        SetState(State.Idle);
                    }
                    else
                    {
                        if (!agent.isMoving)
                        {
                            Vector2 newTargetPos = searchPivot + Random.insideUnitCircle * searchRadius;
                            agent.SetDestination(newTargetPos);
                        }
                    }
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

    void SetState(State newState)
    {
        if (_state == newState) return;
        _state = newState;
        lastStateTransitionTime = Time.time;
    }
    public float stateElapsedTime => Time.time - lastStateTransitionTime;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;

        string label = $"{gameObject.name} [{_state}]";
        Handles.color = Color.red;
        Handles.Label(transform.position + Vector3.up * 1.5f, label);
#endif
    }
}
