using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private float lifetime = 5f; // Auto-destroy after 5 seconds
    private Rigidbody2D rb;
    
    public void Initialize(Vector3 dir, float spd, int dmg)
    {
        direction = ((Vector2)dir).normalized;
        speed = spd;
        damage = dmg;
        
        // Physics-based movement
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 0f;
            rb.linearVelocity = direction * speed;
        }
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    private void Update()
    {
        // Movement handled by Rigidbody2D
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit a fortress block
        var block = collision.gameObject.GetComponent<FortressBlock>();
        if (block != null)
        {
            block.TakeDamage(damage);
            DestroyProjectile();
            return;
        }
        
        // Anything else: just destroy
        DestroyProjectile();
    }
    
    private void DestroyProjectile()
    {
        // TODO: Add impact VFX here
        Destroy(gameObject);
    }
}