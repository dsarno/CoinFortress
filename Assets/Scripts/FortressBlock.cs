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
        
        // Get or add crack effect component
        crackEffect = GetComponent<CrackEffect>();
        if (crackEffect == null)
        {
            crackEffect = gameObject.AddComponent<CrackEffect>();
        }
    }
    
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        
        // Visual feedback
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }
        
        // Update crack effect
        if (crackEffect != null)
        {
            float damagePercent = 1f - ((float)currentHP / maxHP);
            crackEffect.UpdateCrackProgress(damagePercent);
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
        
        // If this is the core or treasure chest, trigger level completion
        if (blockType == BlockType.Core || blockType == BlockType.TreasureChest)
        {
            TriggerLevelComplete();
        }
        
        // TODO: Add destruction VFX, coin explosion for treasure
        Destroy(gameObject);
    }
    
    private void TriggerLevelComplete()
    {
        string blockName = blockType == BlockType.TreasureChest ? "TREASURE CHEST" : "CORE";
        Debug.Log($"{blockName} DESTROYED! Level complete!");
        
        // Notify level manager
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnCoreDestroyed(transform.position);
        }
    }
}