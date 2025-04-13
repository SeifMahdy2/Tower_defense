using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform rotationPoint;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected LayerMask enemyLayer;

    [Header("Attributes")]
    [SerializeField] protected float range = 5f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected int damage = 10;

    protected Transform target;
    protected bool hasTarget = false;
    protected float fireCountdown = 0f;

    protected virtual void Start()
    {
        // Find components if not assigned
        if (rotationPoint == null)
            rotationPoint = transform.Find("RotationPoint");
        
        if (firePoint == null && rotationPoint != null)
            firePoint = rotationPoint.Find("FirePoint");
    }

    protected virtual void Update()
    {
        if (hasTarget)
        {
            // Rotate tower to face target
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (rotationPoint != null)
            {
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                rotationPoint.rotation = Quaternion.Lerp(rotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Check if target is still in range
            if (Vector2.Distance(transform.position, target.position) > range)
            {
                hasTarget = false;
                target = null;
            }
            else
            {
                // Fire at target
                if (fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
                
                fireCountdown -= Time.deltaTime;
            }
        }
        else
        {
            // Find new target
            FindTarget();
        }
    }

    protected virtual void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            
            if (projectile != null)
            {
                projectile.Seek(target, damage);
            }
            else
            {
                Debug.LogWarning("Projectile prefab does not have a Projectile component!");
            }
        }
        else
        {
            Debug.LogWarning("Tower is missing projectile prefab or fire point reference!");
        }
    }

    protected virtual void FindTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (colliders.Length > 0)
        {
            // Get the closest enemy
            float shortestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (Collider2D collider in colliders)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestEnemy = collider.transform;
                }
            }

            if (closestEnemy != null)
            {
                target = closestEnemy;
                hasTarget = true;
            }
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    // Method to expose range for the tower placement manager
    public float GetRange()
    {
        return range;
    }
} 