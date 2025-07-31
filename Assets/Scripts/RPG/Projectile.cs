using UC;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float      speed = 500.0f;
    [SerializeField] private float      maxDuration = 5.0f;
    [SerializeField] private float      radius;
    [SerializeField] private LayerMask  obstacleLayers;
    [SerializeField] private GameObject destroyEffect;

    float currentDuration;

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
