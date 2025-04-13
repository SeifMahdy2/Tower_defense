using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public GameObject impactEffect;
    
    protected Transform target;
    protected int damage;
    
    public void Seek(Transform _target, int _damage)
    {
        target = _target;
        damage = _damage;
    }
    
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        // Move towards target
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        
        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        
        // Rotate towards direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    
    protected virtual void HitTarget()
    {
        // Create impact effect
        if (impactEffect != null)
        {
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
        }
        
        // Apply damage
        EnemyHealth enemy = target.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }
} 