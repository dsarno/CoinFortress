using UnityEngine;
using UnityEditor;

public class UpdateThickenSettings : MonoBehaviour
{
    [MenuItem("Tools/Update Thicken Settings (Max Power)")]
    public static void Execute()
    {
        Material crackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CrackMaterial.mat");
        if (crackMaterial == null)
        {
            Debug.LogError("Crack material not found!");
            return;
        }
        
        // Set higher default thickening (new range: 0-2.0)
        crackMaterial.SetFloat("_ThickenAmount", 0.8f);
        
        EditorUtility.SetDirty(crackMaterial);
        AssetDatabase.SaveAssets();
        
        Debug.Log("✅ Thickening increased!");
        Debug.Log("   New range: 0.0 - 2.0");
        Debug.Log("   Default value: 0.8");
        Debug.Log("   Using 7x7 kernel with 100x multiplier");
    }
}
