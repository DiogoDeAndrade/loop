using NaughtyAttributes;
using System.Collections.Generic;
using UC;
using UnityEngine;

public class AbilityShoot : MonoBehaviour
{
    private struct ProjectileShot
    {
        public Transform    position;
        public Projectile   projectile;
    }

    [SerializeField] 
    private Item   ammoItem;
    [SerializeField, ShowIf(nameof(hasAmmo))] 
    private int    itemPerShot = 0;
    [SerializeField]
    private List<ProjectileShot>    projectiles;

    bool hasAmmo => ammoItem != null;

    
}
