using UnityEngine;
using UnityEditor;

public class AssignCrackShader : MonoBehaviour
{
    [MenuItem("Tools/Assign Crack Shader to Material")]
    public static void Execute()
    {
        // Load the shader
        Shader crackShader = Shader.Find("Custom/CrackShader");
        if (crackShader == null)
        {
            Debug.LogError("Crack shader not found! Make sure 'Custom/CrackShader' exists.");
            return;
        }
        
        // Load the material
        Material crackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CrackMaterial.mat");
        if (crackMaterial == null)
        {
            Debug.LogError("Crack material not found at Assets/Materials/CrackMaterial.mat");
            return;
        }
        
        // Assign shader to material
        crackMaterial.shader = crackShader;
        
        // Set default property values
        crackMaterial.SetFloat("_DamageProgress", 0f);
        crackMaterial.SetFloat("_ThickenAmount", 0.15f);
        crackMaterial.SetColor("_EmissionColor", new Color(1f, 0.4f, 0f, 1f)); // Orange glow
        crackMaterial.SetFloat("_EmissionIntensity", 2.5f);
        crackMaterial.SetFloat("_PulseSpeed", 3f);
        crackMaterial.SetFloat("_RevealMode", 0f); // Radial reveal
        
        // Save changes
        EditorUtility.SetDirty(crackMaterial);
        AssetDatabase.SaveAssets();
        
        Debug.Log("✅ Crack shader assigned to material with default settings!");
    }
}
