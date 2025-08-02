using UC;
using UnityEngine;

public class TargetSelection_AgroList : TargetSelection
{
    AgroList agroList;

    protected override void Start()
    {
        base.Start();

        agroList = GetComponent<AgroList>();
    }

    public override void UpdateSelection()
    {
        if (_currentTarget)
        {
            _targetLastSeenTime = Time.time;
            _targetLastSeenPos = _currentTarget.transform.position;
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
            _prevTarget = _currentTarget;
            _currentTarget = agroTop.GetComponent<Character>();
            if (_currentTarget)
            {
                _targetLastSeenTime = Time.time;
                _targetLastSeenPos = _currentTarget.transform.position;
            }
        }
        else
        {
            _currentTarget = null;
        }
    }

    private bool IsCharacterAndAlive(GameObject target, float value)
    {
        var character = target.GetComponent<Character>();
        if (character == null) return false;
        return character.isAlive;
    }
}
