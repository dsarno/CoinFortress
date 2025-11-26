using UnityEngine;

public enum BlockType
{
    Empty,
    Stone,
    Iron,
    Gold,
    Diamond,
    Silver,
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
    public GameObject hitEffectPrefab;

    [Header("Loot Settings")]
    public GameObject coinPrefab;
    public int coinDropCount = 0;
    
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
        }
        
        // Visual feedback
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }

        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        
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
        // Spawn coins
        SpawnCoins();
        
        // Only Treasure Chest triggers level complete
        if (blockType == BlockType.TreasureChest)
        {
            TriggerLevelComplete();
        }
        
        Destroy(gameObject);
    }

    private void SpawnCoins()
    {
        GameObject prefabToSpawn = coinPrefab;
        if (prefabToSpawn == null && LevelManager.Instance != null)
        {
            prefabToSpawn = LevelManager.Instance.coinPrefab;
        }

        if (prefabToSpawn != null && coinDropCount > 0)
        {
            for (int i = 0; i < coinDropCount; i++)
            {
                Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            }
        }
    }
    
    private void TriggerLevelComplete()
    {
        // Notify level manager
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnCoreDestroyed(transform.position);
        }
        else if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnCoreDestroyed(transform.position);
        }
    }
}