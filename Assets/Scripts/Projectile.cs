using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Splash Damage Settings")]
    public float splashRadius = 1.5f;
    public LayerMask fortressLayerMask;
    
    private Vector2 direction;
    private float speed;
    private int damage;
    private int ammoTier;
    private float lifetime = 5f; // Auto-destroy after 5 seconds
    private Rigidbody2D rb;
    
    public void Initialize(Vector3 dir, float spd, int dmg, int tier = 0)
    {
        direction = ((Vector2)dir).normalized;
        speed = spd;
        damage = dmg;
        ammoTier = tier;
        
        // Physics-based movement with gravity arc
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            // Don't override gravity - let projectiles arc naturally
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
            // Play impact sound
            if (SoundManager.Instance != null)
            {
                // Play impact sound with pitch variation
                SoundManager.Instance.PlayImpactSound(block.blockType, collision.contacts[0].point);
            }
            
            block.TakeDamage(damage);
            
            // Tier 2 (Explosive) - apply splash damage
            if (ammoTier == 2)
            {
                ApplySplashDamage(collision.contacts[0].point);
            }
            
            DestroyProjectile();
            return;
        }
        
        // Anything else: just destroy
        DestroyProjectile();
    }
    
    private void ApplySplashDamage(Vector2 impactPoint)
    {
        // Find all colliders in splash radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(impactPoint, splashRadius);
        
        int splashDamage = Mathf.RoundToInt(damage * 0.5f); // 50% of main damage
        
        foreach (var hitCollider in hitColliders)
        {
            var block = hitCollider.GetComponent<FortressBlock>();
            if (block != null && hitCollider.gameObject != gameObject)
            {
                block.TakeDamage(splashDamage);
            }
        }
        
        Debug.Log($"Splash damage applied! Radius: {splashRadius}, Damage: {splashDamage}");
    }
    
    private void DestroyProjectile()
    {
        // TODO: Add impact VFX here
        Destroy(gameObject);
    }
}