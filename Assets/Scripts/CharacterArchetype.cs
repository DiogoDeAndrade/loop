using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "CharacterArchetype", menuName = "Loop/Character Archetype")]
public class CharacterArchetype : ScriptableObject
{
    public Sprite   headSprite;
    public Sprite   bodySprite;
    public float    maxSpeed = 200.0f;
    [HorizontalLine(color: EColor.Red)]
    public bool     isPlayer = false;
    [ShowIf(nameof(isPlayer))]
    public float    acceleration = 400.0f;
    [ShowIf(nameof(isPlayer))]
    public float    dragCoeff = 200.0f;
}
