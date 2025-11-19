using UnityEngine;

public class CrackEffect : MonoBehaviour
{
    [Header("Crack Sprites Database")]
    public CrackSpritesDatabase spritesDatabase;
    
    [Header("Variation Settings")]
    public float minRotation = -15f;
    public float maxRotation = 15f;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    
    private SpriteRenderer crackRenderer;
    private Transform blockTransform;
    
    private void Awake()
    {
        blockTransform = transform.parent;
        
        // Create crack overlay as child
        GameObject crackObj = new GameObject("CrackOverlay");
        crackObj.transform.SetParent(transform, false);
        crackObj.transform.localPosition = Vector3.zero;
        
        // Setup sprite renderer
        crackRenderer = crackObj.AddComponent<SpriteRenderer>();
        crackRenderer.sortingLayerName = "Default";
        crackRenderer.sortingOrder = 1; // Above the block sprite
        crackRenderer.color = Color.white;
        
        // Scale crack to fit the block sprite
        ScaleCrackToBlock();
        
        // Start hidden
        crackRenderer.enabled = false;
    }
    
    private void ScaleCrackToBlock()
    {
        // We'll calculate the proper scale in ShowCrack when we know the crack sprite size
        crackRenderer.transform.localScale = Vector3.one;
    }
    
    public void ShowCrack()
    {
        // Get database from manager if not assigned
        if (spritesDatabase == null && CrackEffectManager.Instance != null)
        {
            spritesDatabase = CrackEffectManager.Instance.GetDatabase();
        }
        
        if (spritesDatabase == null || spritesDatabase.crackSprites == null || spritesDatabase.crackSprites.Length == 0)
        {
            Debug.LogWarning("No crack sprites database or sprites assigned!");
            return;
        }
        
        // Pick random crack sprite
        Sprite randomCrack = spritesDatabase.crackSprites[Random.Range(0, spritesDatabase.crackSprites.Length)];
        crackRenderer.sprite = randomCrack;
        
        // Calculate proper scale to fit block
        SpriteRenderer blockRenderer = GetComponentInParent<SpriteRenderer>();
        if (blockRenderer != null && blockRenderer.sprite != null && randomCrack != null)
        {
            // Get block size in world units (using bounds which accounts for scale)
            Bounds blockBounds = blockRenderer.bounds;
            Vector2 blockSize = new Vector2(blockBounds.size.x, blockBounds.size.y);
            
            // Target crack size is 80% of block size
            float targetSizePercent = 0.8f;
            Vector2 targetSize = blockSize * targetSizePercent;
            
            // Get crack sprite size at scale 1
            Vector2 crackSize = randomCrack.bounds.size;
            
            // Calculate scale needed to fit target size
            float scaleX = targetSize.x / crackSize.x;
            float scaleY = targetSize.y / crackSize.y;
            float uniformScale = Mathf.Min(scaleX, scaleY); // Use minimum to fit within bounds
            
            // Apply random rotation (clamped)
            float randomRotation = Random.Range(minRotation, maxRotation);
            crackRenderer.transform.localRotation = Quaternion.Euler(0, 0, randomRotation);
            
            // Apply scale with random variation (clamped)
            float randomScaleVariation = Random.Range(minScale, maxScale);
            crackRenderer.transform.localScale = Vector3.one * uniformScale * randomScaleVariation;
        }
        
        // Show the crack
        crackRenderer.enabled = true;
    }
    
    public void HideCrack()
    {
        if (crackRenderer != null)
        {
            crackRenderer.enabled = false;
        }
    }
    
    public void UpdateCrackProgress(float damagePercent)
    {
        // This will be used in step 2 with the shader
        // For now, just show/hide based on any damage
        if (damagePercent > 0)
        {
            if (!crackRenderer.enabled)
            {
                ShowCrack();
            }
        }
        else
        {
            HideCrack();
        }
    }
}
