using NaughtyAttributes;
using UnityEngine;

public class DamageModifier : MonoBehaviour
{
    private enum ModifierType { None, Multiplier };

    [SerializeField]
    private ModifierType    type;
    [SerializeField, ShowIf(nameof(isMultiplier))]
    private float           multiplier = 1.0f;

    bool isMultiplier => type == ModifierType.Multiplier;

    public float ModifyDamage(float damage, Projectile source)
    {
        switch (type)
        {
            case ModifierType.None:
                break;
            case ModifierType.Multiplier:
                return damage * multiplier;
            default:
                break;
        }

        return damage;
    }
}
