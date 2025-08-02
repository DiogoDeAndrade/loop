using System.Collections.Generic;
using UC;
using UnityEngine;

public class AbilityShoot : Ability
{
    [System.Serializable]
    private struct ProjectileShot
    {
        public Transform    position;
        public Projectile   projectile;
        public bool         checkLineOfSight;
    }

    [SerializeField]
    private List<ProjectileShot>    projectiles;

    public override bool CanTrigger(Vector3 targetPos)
    {
        if (!base.CanTrigger(targetPos)) return false;

        // Check LoS - Only if it's not the player
        if (character.isPlayer) return true;

        foreach (var p in projectiles)
        {
            Vector3 targetDir = targetPos - p.position.position;

            RaycastHit2D hit = Physics2D.Raycast(p.position.position, targetDir.normalized, targetDir.magnitude, Globals.obstacleMask);
            if (hit.collider == null) return true;
        }

        return false;
    }

    public override void Trigger(float chargeDuration)
    {
        if (projectiles != null)
        {
            foreach (var p in projectiles)
            {
                var projectile = Instantiate(p.projectile, p.position.position, p.position.rotation);
                projectile.faction = character.faction;
                projectile.owner = character;
            }
        }

        base.Trigger(chargeDuration);
    }

    public override void Destroy()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr)
        {
            sr.FadeTo(sr.color.ChangeAlpha(0.0f), 0.25f);
        }
        base.Destroy();
        Destroy(gameObject, 0.25f);
    }
}
