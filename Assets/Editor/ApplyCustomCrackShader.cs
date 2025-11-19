using UnityEngine;
using UnityEditor;

public class ApplyCustomCrackShader : MonoBehaviour
{
    [MenuItem("Tools/Apply Custom Crack Shader")]
    public static void Execute()
    {
        // Load the custom shader
        Shader customShader = Shader.Find("Shader Graphs/CrackShaderCustom");
        if (customShader == null)
        {
            Debug.LogError("Custom crack shader not found! Looking for 'Shader Graphs/CrackShaderCustom'");
            return;
        }
        
        // Load the crack material
        Material crackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CrackMaterial.mat");
        if (crackMaterial == null)
        {
            Debug.LogError("Crack material not found at Assets/Materials/CrackMaterial.mat");
            return;
        }
        
        // Assign custom shader to material
        crackMaterial.shader = customShader;
        
        // Set default _Reveal value
        if (crackMaterial.HasProperty("_Reveal"))
        {
            crackMaterial.SetFloat("_Reveal", 0f);
            Debug.Log("✅ Set _Reveal property to 0");
        }
        
        // Save changes
        EditorUtility.SetDirty(crackMaterial);
        AssetDatabase.SaveAssets();
        
        Debug.Log("✅ Custom crack shader applied to material!");
        Debug.Log($"   Shader: {customShader.name}");
        Debug.Log("   Material: CrackMaterial.mat");
    }
}
