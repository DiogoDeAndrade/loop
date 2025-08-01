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
    protected ResourceHandler   health;
    [HorizontalLine(color: EColor.Green)]
    [SerializeField]
    protected SpriteRenderer    bodySpriteRenderer;
    [SerializeField]
    protected HeadController        headSpriteRenderer;
    [SerializeField]
    protected SpriteRenderer    shadowRenderer;
    [SerializeField]
    protected ParticleSystem    deathPS;
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
        var colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders) collider.enabled = false;
        rb.simulated = false;
        if (bodySpriteRenderer) bodySpriteRenderer.FadeTo(shadowRenderer.color.ChangeAlpha(0.0f), 0.2f);
        if (shadowRenderer) shadowRenderer.FadeTo(shadowRenderer.color.ChangeAlpha(0.0f), 0.2f);
        if (headSpriteRenderer)
        {
            Vector3 dir = transform.position - changeSource.transform.position;
            dir.y = Mathf.Abs(dir.y);
            dir.z = 0.0f;
            dir.Normalize();
            headSpriteRenderer.PopOff(dir);
        }

        deathPS.Play();
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
