using NaughtyAttributes;
using UnityEngine;
using UC;

public class TestPath : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    NavMeshAgent2d  agent;
    bool            wander;
    Vector3         spawnPos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent2d>();

        agent.onComplete += Agent_onComplete;
        agent.onStopped = Agent_OnStopped;

        transform.position = startPos.position;
        spawnPos = transform.position;
    }

    private bool Agent_OnStopped(NavMeshAgent2d agent)
    {
        Debug.Log("Obstacles stopped the agent!");
        if (wander)
        {
            agent.ResetStopDetection();
            StartWander();
            return true;
        }

        return false;
    }

    private void Agent_onComplete(NavMeshAgent2d agent)
    {
        if (wander)
        {
            StartWander();
        }
    }

    [Button("Teleport To Start")]
    void TeleportToStart()
    {
        transform.position = startPos.position;
    }

    [Button("Move To Start")]
    void MoveToStart()
    {
        wander = false;
        agent.SetDestination(startPos.position);
    }

    [Button("Move To End")]
    void MoveToEnd()
    {
        wander = false;
        agent.SetDestination(endPos.position);
    }

    [Button("Start Wander")]
    void StartWander()
    {
        wander = true;

        var randomPoint = GetRandomPoint();
        if (!agent.SetDestination(randomPoint))
        {
            Debug.LogWarning($"Can't set path for {randomPoint}!");
        }
    }

    Vector3 GetRandomPoint()
    {
        return spawnPos + Random.insideUnitCircle.xy0() * 400.0f;
    }
}
