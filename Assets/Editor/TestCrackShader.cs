using UnityEngine;
using UnityEditor;

public class TestCrackShader : MonoBehaviour
{
    [MenuItem("Tools/Test Crack Shader Progression")]
    public static void Execute()
    {
        // Find all FortressBlock objects with CrackEffect components
        FortressBlock[] blocks = Object.FindObjectsOfType<FortressBlock>();
        
        if (blocks.Length == 0)
        {
            Debug.LogWarning("No FortressBlock objects found in the scene!");
            return;
        }
        
        Debug.Log($"Testing crack shader on {blocks.Length} blocks...");
        
        // Test different damage levels on different blocks
        for (int i = 0; i < Mathf.Min(blocks.Length, 10); i++)
        {
            FortressBlock block = blocks[i];
            CrackEffect crackEffect = block.GetComponent<CrackEffect>();
            
            if (crackEffect != null)
            {
                // Test progressive damage (0%, 25%, 50%, 75%, 100%)
                float damagePercent = (i % 5) * 0.25f;
                crackEffect.UpdateCrackProgress(damagePercent);
                
                Debug.Log($"Block '{block.name}': Set crack progress to {damagePercent * 100}%");
            }
        }
        
        Debug.Log("✅ Crack shader test complete! Check scene view to see progressive reveal, thickening, and emission.");
    }
    
    [MenuItem("Tools/Reset All Cracks")]
    public static void ResetCracks()
    {
        FortressBlock[] blocks = Object.FindObjectsOfType<FortressBlock>();
        
        foreach (var block in blocks)
        {
            CrackEffect crackEffect = block.GetComponent<CrackEffect>();
            if (crackEffect != null)
            {
                crackEffect.UpdateCrackProgress(0f);
            }
        }
        
        Debug.Log("✅ All cracks reset!");
    }
}
