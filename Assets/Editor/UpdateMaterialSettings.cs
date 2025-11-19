using UnityEngine;
using UnityEditor;

public class UpdateMaterialSettings : MonoBehaviour
{
    [MenuItem("Tools/Update Crack Material Settings")]
    public static void Execute()
    {
        Material crackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CrackMaterial.mat");
        if (crackMaterial == null)
        {
            Debug.LogError("Crack material not found!");
            return;
        }
        
        // Set enhanced values for better visibility
        crackMaterial.SetFloat("_DamageProgress", 0f);
        crackMaterial.SetFloat("_ThickenAmount", 0.35f); // Increased from 0.15
        crackMaterial.SetColor("_EmissionColor", new Color(1f, 0.5f, 0f, 1f)); // Bright orange
        crackMaterial.SetFloat("_EmissionIntensity", 3.0f); // Increased
        crackMaterial.SetFloat("_PulseSpeed", 4f);
        crackMaterial.SetFloat("_RevealMode", 0f); // Radial reveal
        
        EditorUtility.SetDirty(crackMaterial);
        AssetDatabase.SaveAssets();
        
        Debug.Log("✅ Crack material settings updated for better visibility!");
        Debug.Log("  - ThickenAmount: 0.35 (more visible thickening)");
        Debug.Log("  - EmissionIntensity: 3.0 (brighter glow)");
        Debug.Log("  - EmissionColor: Bright Orange");
    }
}
