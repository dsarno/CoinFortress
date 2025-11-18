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
    
    private void Start()
    {
        currentHP = maxHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
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
        
        // For now, just trigger the coin fountain directly
        // TODO: Move this to FortressManager when we create it
        StartCoroutine(SpawnCoinFountain());
    }
    
    private System.Collections.IEnumerator SpawnCoinFountain()
    {
        Debug.Log("Coin fountain activated!");
        // TODO: Implement actual coin spawning
        yield return null;
    }
}