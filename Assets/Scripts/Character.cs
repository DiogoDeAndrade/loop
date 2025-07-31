using NaughtyAttributes;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField, Expandable]
    protected CharacterArchetype characterType;
    [SerializeField]
    protected SpriteRenderer    bodySpriteRenderer;
    [SerializeField]
    protected HeadRotate        headSpriteRenderer;

    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        characterType = GameManager.selectedCharacter;
        rb = GetComponent<Rigidbody2D>();

        bodySpriteRenderer.sprite = characterType.bodySprite;
        headSpriteRenderer.SetHeads(characterType.headSpriteDown,
                            characterType.headSpriteRight, false,
                            (characterType.headSpriteLeft != null) ? (characterType.headSpriteLeft) : (characterType.headSpriteRight), (characterType.headSpriteLeft == null),
                            characterType.headSpriteUp);
    }
}
