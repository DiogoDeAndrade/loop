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
                Instantiate(p.projectile, p.position.position, p.position.rotation);
            }
        }

        base.Trigger(chargeDuration);
    }
}
