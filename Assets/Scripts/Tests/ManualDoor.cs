using UC;
using UnityEngine;

public class ManualDoor : MonoBehaviour, INavMeshLinkCondition
{
    [SerializeField] private bool isOpen;

    public bool NavCanPass(NavMeshAgent2d agent, NavMeshLink2d link)
    {
        return isOpen;
    }
}
