using UnityEngine;
using UnityEditor;

public class RefreshCrackMaterials : MonoBehaviour
{
    [MenuItem("Tools/Refresh Crack Materials in Scene")]
    public static void Execute()
    {
        // Load the crack material
        Material crackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CrackMaterial.mat");
        if (crackMaterial == null)
        {
            Debug.LogError("Crack material not found!");
            return;
        }
        
        // Find all CrackEffect components in the scene
        CrackEffect[] allCracks = Object.FindObjectsByType<CrackEffect>(FindObjectsSortMode.None);
        
        int refreshedCount = 0;
        
        foreach (var crack in allCracks)
        {
            // Access the crack renderer via reflection or find child
            Transform crackChild = crack.transform.Find("CrackOverlay");
            if (crackChild != null)
            {
                SpriteRenderer crackRenderer = crackChild.GetComponent<SpriteRenderer>();
                if (crackRenderer != null)
                {
                    // Re-assign the material to pick up new settings
                    crackRenderer.material = crackMaterial;
                    
                    // Force update the damage progress to refresh MaterialPropertyBlock
                    FortressBlock block = crack.GetComponentInParent<FortressBlock>();
                    if (block != null && block.maxHP > 0)
                    {
                        float damagePercent = 1f - ((float)block.currentHP / block.maxHP);
                        crack.UpdateCrackProgress(damagePercent);
                    }
                    
                    refreshedCount++;
                }
            }
        }
        
        Debug.Log($"✅ Refreshed crack materials on {refreshedCount} instances!");
        Debug.Log("   Material settings:");
        Debug.Log($"   - ThickenAmount: {crackMaterial.GetFloat("_ThickenAmount")}");
        Debug.Log($"   - PulseSpeed: {crackMaterial.GetFloat("_PulseSpeed")}");
        Debug.Log($"   - EmissionIntensity: {crackMaterial.GetFloat("_EmissionIntensity")}");
    }
}
