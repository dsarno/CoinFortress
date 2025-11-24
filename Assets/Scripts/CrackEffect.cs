using UnityEngine;

// CrackEffect now drives a single SpriteRenderer (the block's own renderer)
// Your Shader Graph (CrackShaderCustom) blends _BlockTex and _CrackTex and reveals
// the crack by _Reveal in [0..1]. We set both textures and animate _Reveal only.
public class CrackEffect : MonoBehaviour
{
    [Header("Crack Sprites Database")]
    public CrackSpritesDatabase spritesDatabase;

    [Header("Crack Variation (sprite choice only)")]
    public float minRotation = -15f; // kept for future use if needed
    public float maxRotation = 15f;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    // IDs
    private static readonly int RevealID   = Shader.PropertyToID("_Reveal");
    private static readonly int BlockTexID = Shader.PropertyToID("_BlockTex");
    private static readonly int CrackTexID = Shader.PropertyToID("_CrackTex");

    // Cached
    private SpriteRenderer blockSR;          // the block's own sprite renderer
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        blockSR = GetComponent<SpriteRenderer>();
        if (blockSR == null)
        {
            Debug.LogError($"CrackEffect requires a SpriteRenderer on {name}.");
            enabled = false;
            return;
        }

        // Ensure material is the crack material from the manager
        if (CrackEffectManager.Instance != null)
        {
            var mat = CrackEffectManager.Instance.GetCrackMaterial();
            if (mat != null)
            {
                // use a material instance on this renderer
                blockSR.material = mat;
            }
        }

        // Property block for per-instance values
        mpb = new MaterialPropertyBlock();

        // Set the base block texture from the current sprite
        if (blockSR.sprite != null)
        {
            blockSR.GetPropertyBlock(mpb);
            mpb.SetTexture(BlockTexID, blockSR.sprite.texture);
            mpb.SetFloat(RevealID, 0f); // hidden by default
            blockSR.SetPropertyBlock(mpb);
        }
    }

    // Pick and assign a crack texture to the material (single time per first damage)
    private void EnsureCrackTextureAssigned()
    {
        if (spritesDatabase == null && CrackEffectManager.Instance != null)
            spritesDatabase = CrackEffectManager.Instance.GetDatabase();

        if (spritesDatabase == null || spritesDatabase.crackSprites == null || spritesDatabase.crackSprites.Length == 0)
        {
            Debug.LogWarning($"[{name}] Crack sprites database missing or empty.");
            return;
        }

        // If a crack already assigned, skip
        blockSR.GetPropertyBlock(mpb);
        var currentCrackTex = mpb != null ? mpb.GetTexture(CrackTexID) : null;
        if (currentCrackTex != null) return;

        // Choose random crack sprite
        var crackSprite = spritesDatabase.crackSprites[Random.Range(0, spritesDatabase.crackSprites.Length)];
        if (crackSprite == null)
        {
            Debug.LogWarning($"[{name}] Selected crack sprite is null.");
            return;
        }

        // Assign
        mpb.SetTexture(CrackTexID, crackSprite.texture);
        blockSR.SetPropertyBlock(mpb);
    }

    public void UpdateCrackProgress(float damagePercent)
    {
        if (blockSR == null) return;

        // One-pass MPB update
        blockSR.GetPropertyBlock(mpb);

        // Update base texture from current sprite (only if shader expects it)
        if (blockSR.sprite != null)
        {
            mpb.SetTexture(BlockTexID, blockSR.sprite.texture);
        }

        // Ensure crack texture assigned once
        if (mpb.GetTexture(CrackTexID) == null)
        {
            EnsureCrackTextureAssigned();
            // EnsureCrackTextureAssigned() already set CrackTex in mpb via SetPropertyBlock,
            // so refresh local copy to continue updating in one pass.
            blockSR.GetPropertyBlock(mpb);
        }

        // Reveal amount
        mpb.SetFloat(RevealID, Mathf.Clamp01(damagePercent));
        blockSR.SetPropertyBlock(mpb);
    }

    // Hide/reset cracks (e.g., on repair)
    public void HideCrack()
    {
        if (blockSR == null) return;
        blockSR.GetPropertyBlock(mpb);
        mpb.SetFloat(RevealID, 0f);
        blockSR.SetPropertyBlock(mpb);
    }
    
#if UNITY_EDITOR
    // Editor safety: ensure property IDs are valid at edit time
    private void OnValidate()
    {
        // Touch the property IDs to catch reference issues early
        _ = RevealID;
        _ = BlockTexID;
        _ = CrackTexID;
    }
#endif
}
