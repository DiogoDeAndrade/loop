using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UC;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField, Expandable]
    protected CharacterArchetype characterType;
    [SerializeField]
    protected SpriteRenderer    bodySpriteRenderer;
    [SerializeField]
    protected HeadRotate        headSpriteRenderer;
    [SerializeField]
    protected ResourceHandler   health;
    [HorizontalLine(color: EColor.Green)]
    [SerializeField]
    protected Color             combatTextColorError = Color.red;

    protected List<SpriteEffect>    spriteEffects;
    protected Rigidbody2D           rb;

    public Faction faction => characterType.faction;

    protected virtual void Start()
    {
        if (characterType == null)
        {
            // Player character, fetch from GameManager
            characterType = GameManager.selectedCharacter;
        }
        rb = GetComponent<Rigidbody2D>();

        if (bodySpriteRenderer)
            bodySpriteRenderer.sprite = characterType.bodySprite;
        if (headSpriteRenderer)
            headSpriteRenderer.SetHeads(characterType.headSpriteDown,
                                        characterType.headSpriteRight, false,
                                        (characterType.headSpriteLeft != null) ? (characterType.headSpriteLeft) : (characterType.headSpriteRight), (characterType.headSpriteLeft == null),
                                        characterType.headSpriteUp);

        spriteEffects = new();
        if (bodySpriteRenderer)
        {
            var bodySpriteRendererSE = bodySpriteRenderer.GetComponent<SpriteEffect>();
            if (bodySpriteRendererSE) spriteEffects.Add(bodySpriteRendererSE);
        }
        if (headSpriteRenderer)
        {
            var headSpriteRendererSE = headSpriteRenderer.GetComponent<SpriteEffect>();
            if (headSpriteRendererSE) spriteEffects.Add(headSpriteRendererSE);
        }

        if (health)
        {
            health.onChange += OnDamage;
            health.onResourceEmpty += OnDeath;
        }
    }

    private void OnDeath(GameObject changeSource)
    {
        throw new NotImplementedException();
    }

    private void OnDamage(ResourceHandler.ChangeType changeType, float deltaValue, Vector3 changeSrcPosition, Vector3 changeSrcDirection, GameObject changeSource)
    {
        if (deltaValue < 0)
        {
            foreach (var spriteEffect in spriteEffects)
            {
                spriteEffect.FlashInvert(0.15f);
            }

            CombatTextManager.SpawnText(gameObject, string.Format("{0}", (int)deltaValue), combatTextColorError, combatTextColorError.ChangeAlpha(0.0f));
        }
    }

    public virtual float ModifyDamage(float baseDamage, Character target)
    {
        return baseDamage;
    }
}
