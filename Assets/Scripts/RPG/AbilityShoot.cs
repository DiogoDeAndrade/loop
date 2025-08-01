using System.Collections.Generic;
using UnityEngine;

public class AbilityShoot : Ability
{
    [System.Serializable]
    private struct ProjectileShot
    {
        public Transform    position;
        public Projectile   projectile;
    }

    [SerializeField]
    private List<ProjectileShot>    projectiles;


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
}
