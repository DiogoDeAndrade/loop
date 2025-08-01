using NaughtyAttributes;
using UnityEngine;
using UC;
using System;

public abstract class Ability : MonoBehaviour
{
    public enum TriggerMode { Single, Charge, Continuous };

    [SerializeField]
    protected int   _abilityIndex = 0;
    [SerializeField]
    protected TriggerMode   _triggerMode = TriggerMode.Single;
    [SerializeField]
    protected Item  ammoItem;
    [SerializeField, ShowIf(nameof(hasAmmo))]
    protected int   itemPerShot = 1;
    [SerializeField]
    protected ResourceType resourceType;
    [SerializeField, ShowIf(nameof(hasResource))]
    protected float resourcePerShot = 1;
    [SerializeField]
    protected bool  hasCooldown;
    [SerializeField, ShowIf(nameof(hasCooldown))]
    protected float cooldownTime;

    protected bool hasAmmo => ammoItem != null;
    protected bool hasResource => resourceType != null;
    protected float lastTriggerTime = -float.MaxValue;
    protected Character character;
    protected Inventory inventory;
    protected ResourceHandler resource;

    public int abilityIndex => _abilityIndex;
    public TriggerMode triggerMode => _triggerMode;

    public void Start()
    {
        character = GetComponentInParent<Character>();
        inventory = character.GetComponent<Inventory>();
        resource = character.FindResourceHandler(resourceType);
    }

    public virtual bool HasAmmo()
    {
        if (ammoItem != null)
        {
            if (inventory == null) return false;
            if (inventory.GetItemCount(ammoItem) < itemPerShot) return false;
        }
        return true;
    }
    public virtual bool HasResource()
    {
        if (resourceType != null)
        {
            if (resource == null) return false;
            if (resource.resource < resourcePerShot) return false;
        }
        return true;
    }

    public virtual bool CanTrigger(Vector3 targetPos)
    {
        if (hasCooldown)
        {
            var t = Time.time - lastTriggerTime;
            if (t < cooldownTime) return false;
        }

        if (!HasAmmo()) return false;
        if (!HasResource()) return false;

        return true;
    }

    public virtual void Trigger(float chargeDuration)
    {
        lastTriggerTime = Time.time;
        if (ammoItem != null)
        {
            inventory?.Remove(ammoItem, itemPerShot);
        }
        if (resourceType)
        {
            resource?.Change(ResourceHandler.ChangeType.Burst, -resourcePerShot, character.transform.position, Vector3.zero, character.gameObject);
        }
    }
    public virtual void Destroy()
    {
    }
}
