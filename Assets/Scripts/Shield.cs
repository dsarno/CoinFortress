using UnityEngine;

public class Shield : MonoBehaviour
{
    private PlayerStats playerStats;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    [Header("Visual Feedback")]
    public Color hitColor = Color.cyan;
    public float hitFlashDuration = 0.1f;
    
    private void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        UpdateVisibility();
    }
    
    private void Update()
    {
        UpdateVisibility();
    }
    
    private void UpdateVisibility()
    {
        if (playerStats == null) return;
        
        // Show shield only if unlocked and has HP
        bool shouldBeVisible = playerStats.shieldUnlocked && playerStats.shieldCurrentHP > 0;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = shouldBeVisible;
        }
        
        // Update opacity based on HP percentage
        if (shouldBeVisible && spriteRenderer != null)
        {
            float hpPercent = (float)playerStats.shieldCurrentHP / playerStats.shieldMaxHP;
            Color color = originalColor;
            color.a = Mathf.Lerp(0.3f, 1f, hpPercent);
            spriteRenderer.color = color;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only block enemy projectiles
        // Player projectiles should pass through (use layers to control this)
        if (collision.CompareTag("EnemyProjectile"))
        {
            TakeHit(1);
            Destroy(collision.gameObject);
        }
    }
    
    private void TakeHit(int damage)
    {
        if (playerStats == null) return;
        
        playerStats.shieldCurrentHP = Mathf.Max(0, playerStats.shieldCurrentHP - damage);
        
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashHit());
        }
        
        Debug.Log($"Shield hit! HP: {playerStats.shieldCurrentHP}/{playerStats.shieldMaxHP}");
        
        if (playerStats.shieldCurrentHP <= 0)
        {
            Debug.Log("Shield broken!");
        }
    }
    
    private System.Collections.IEnumerator FlashHit()
    {
        Color originalFlashColor = spriteRenderer.color;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalFlashColor;
    }
}
