using UC;
using UnityEngine;

public class ManualDoor : MonoBehaviour, INavMeshLinkCondition
{
    [SerializeField] private bool isOpen;

    Collider2D _collider;

    public bool NavCanPass(NavMeshAgent2d agent, NavMeshLink2d link)
    {
        return isOpen;
    }

    void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        _collider.enabled = !isOpen;
    }
}
