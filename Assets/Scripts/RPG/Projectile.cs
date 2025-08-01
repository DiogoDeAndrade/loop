using UC;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Faction        _faction;
    [SerializeField] private float          speed = 500.0f;
    [SerializeField] private float          maxDuration = 5.0f;
    [SerializeField] private float          radius;
    [SerializeField] private float          _damage;
    [SerializeField] private LayerMask      obstacleLayers;
    [SerializeField] private LayerMask      damageLayers;
    [SerializeField] private ResourceType   damageResource;
    [SerializeField] private GameObject     destroyEffect;

    float       currentDuration;
    Character   _owner;

    public Faction faction
    {
        get => _faction;
        set { _faction = value; }
    }

    public Character owner
    {
        get => _owner;
        set { _owner = value; }
    }

    private void Start()
    {
        currentDuration = maxDuration;
    }

    void Update()
    {
        var prevPos = transform.position;
        transform.position = transform.position + speed * transform.right * Time.deltaTime;

        currentDuration -= Time.deltaTime;
        if (currentDuration <= 0.0f)
        {
            Destroy(gameObject);
        }
        else
        {
            var dir = transform.position - prevPos;
            var dist = dir.magnitude;
            if (dist > 1e-3)
            {
                dir = dir / dist;

                var obstacleHit = Physics2D.CircleCast(prevPos, radius, dir, dist, obstacleLayers);
                if (obstacleHit.collider != null)
                {
                    DestroyProjectile(obstacleHit.point, obstacleHit.normal);
                }
                var damageHit = Physics2D.CircleCast(prevPos, radius, dir, dist, damageLayers);
                if (damageHit.collider != null)
                {
                    // Check if hit character, and if factions are different
                    Character character = damageHit.collider.GetComponent<Character>();
                    if (character != null)
                    {
                        if (!faction.IsHostile(character.faction)) return;
                    }
                    var resourceHandler = damageHit.collider.FindResourceHandler(damageResource);
                    if (resourceHandler)
                    {
                        float damage = _damage;
                        if (character)
                        {
                            damage = _owner.ModifyDamage(_damage, character);
                        }
                        resourceHandler.Change(ResourceHandler.ChangeType.Burst, -_damage, damageHit.point, -dir, _owner.gameObject);
                    }
                    
                    DestroyProjectile(damageHit.point, damageHit.normal);
                }
            }
        }
    }

    void DestroyProjectile(Vector3 pos, Vector3 dir)
    {
        if (destroyEffect)
        {
            Instantiate(destroyEffect, pos, Quaternion.LookRotation(Vector3.forward, -dir.PerpendicularXY()));
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
