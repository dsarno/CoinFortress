using UnityEngine;
using UnityEditor;

public class SetupEmissionFadeIn : MonoBehaviour
{
    [MenuItem("Tools/Setup Emission Fade-In")]
    public static void Execute()
    {
        Material crackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CrackMaterial.mat");
        if (crackMaterial == null)
        {
            Debug.LogError("Crack material not found!");
            return;
        }
        
        // Set emission fade-in time (how long it takes for glow to appear after hit)
        crackMaterial.SetFloat("_EmissionFadeInTime", 1.5f); // 1.5 seconds
        crackMaterial.SetFloat("_TimeSinceCrack", 0f);
        
        EditorUtility.SetDirty(crackMaterial);
        AssetDatabase.SaveAssets();
        
        Debug.Log("✅ Emission fade-in configured!");
        Debug.Log("   Cracks will appear BLACK on impact");
        Debug.Log("   Glow fades in over 1.5 seconds");
        Debug.Log("   Then pulses continuously");
    }
}
