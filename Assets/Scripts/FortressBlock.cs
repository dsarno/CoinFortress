using UnityEngine;

public enum BlockType
{
    Empty,
    Stone,
    Iron,
    Gold,
    Core,
    Turret,
    Window,
    Roof,
    TreasureChest
}

public class FortressBlock : MonoBehaviour
{
    [Header("Block Settings")]
    public BlockType blockType = BlockType.Stone;
    public int maxHP = 1;
    public int currentHP;
    
    [Header("Visual Feedback")]
    public Color damageColor = Color.red;
    public float damageFlashDuration = 0.1f;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private CrackEffect crackEffect;
    
    private void Start()
    {
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Cache crack effect component
        crackEffect = GetComponent<CrackEffect>();
    }
    
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        
        // Calculate damage percentage (0 = no damage, 1 = fully destroyed)
        float damagePercent = 1f - (currentHP / (float)maxHP);
        damagePercent = Mathf.Clamp01(damagePercent);
        
        // Update crack effect
        if (crackEffect != null)
        {
            crackEffect.UpdateCrackProgress(damagePercent);
            Debug.Log($"[{gameObject.name}] Crack _Reveal set to {damagePercent:F2} (HP: {currentHP}/{maxHP})");
        }
        
        // Visual feedback
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }
        
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHP}/{maxHP}");
        
        if (currentHP <= 0)
        {
            Die();
        }
    }
    
    private System.Collections.IEnumerator FlashDamage()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = originalColor;
    }
    
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} destroyed!");
        
        // If this is the core, trigger special behavior
        if (blockType == BlockType.Core)
        {
            TriggerCoreDestruction();
        }
        
        // TODO: Add destruction VFX
        Destroy(gameObject);
    }
    
    private void TriggerCoreDestruction()
    {
        Debug.Log("CORE DESTROYED! Level complete!");
        
        // Notify level manager
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnCoreDestroyed(transform.position);
        }
    }
}