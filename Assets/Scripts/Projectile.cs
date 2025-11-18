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
        
        // Physics-based movement with gravity arc
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Optimize physics settings to prevent jittering
            rb.bodyType = RigidbodyType2D.Dynamic;
            
            // CRITICAL: Enable interpolation for smooth movement between physics steps
            // This prevents jittering when physics timestep doesn't match render framerate
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            
            // Use Continuous collision detection for fast-moving projectiles
            // Prevents tunneling through colliders at high speeds
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            // Prevent the projectile from going to sleep (which can cause jitter)
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
            
            // Set drag to 0 for projectiles (they should maintain velocity)
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            
            // Set velocity after all settings are configured
            rb.linearVelocity = direction * speed;
            
            // Wake up the rigidbody to ensure it's active
            rb.WakeUp();
        }
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
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