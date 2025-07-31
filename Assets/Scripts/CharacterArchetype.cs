using UnityEngine;
using NaughtyAttributes;
using UC;

[CreateAssetMenu(fileName = "CharacterArchetype", menuName = "Loop/Character Archetype")]
public class CharacterArchetype : ScriptableObject
{
    public Sprite   bodySprite;
    public Sprite   headSpriteUp;
    public Sprite   headSpriteDown;
    public Sprite   headSpriteLeft;
    public Sprite   headSpriteRight;
    public float    maxSpeed = 200.0f;
    [HorizontalLine(color: EColor.Red)]
    public bool     isPlayer = false;
    [ShowIf(nameof(isPlayer))]
    public float    acceleration = 400.0f;
    [ShowIf(nameof(isPlayer))]
    public float    dragCoeff = 200.0f;
    [ShowIf(nameof(isPlayer))]
    public Inventory.Items[]    initialItems;
}
