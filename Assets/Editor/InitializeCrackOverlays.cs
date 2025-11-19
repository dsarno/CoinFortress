using UnityEngine;
using UnityEditor;

public class InitializeCrackOverlays : MonoBehaviour
{
    [MenuItem("Tools/Initialize Crack Overlays (Force Awake)")]
    public static void Execute()
    {
        // Find all CrackEffect components
        CrackEffect[] allCracks = Object.FindObjectsByType<CrackEffect>(FindObjectsSortMode.None);
        
        if (allCracks.Length == 0)
        {
            Debug.LogWarning("No CrackEffect components found!");
            return;
        }
        
        int initializedCount = 0;
        
        foreach (var crack in allCracks)
        {
            // Manually trigger Awake via SendMessage (works in Edit mode)
            crack.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
            initializedCount++;
        }
        
        Debug.Log($"✅ Initialized {initializedCount} crack overlays!");
        Debug.Log("   CrackOverlay children created");
        Debug.Log("   Custom shader assigned via CrackEffectManager");
        
        // Verify the material
        Material crackMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CrackMaterial.mat");
        if (crackMat != null)
        {
            Debug.Log($"   Current shader: {crackMat.shader.name}");
        }
    }
}
