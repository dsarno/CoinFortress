using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class CheckScenePath : MonoBehaviour
{
    [MenuItem("Tools/Check Scene Info")]
    public static void Check()
    {
        Scene current = SceneManager.GetActiveScene();
        Debug.Log($"📂 Current Open Scene: {current.path}");
        
        Debug.Log("📋 Scenes in Build Settings:");
        foreach (var s in EditorBuildSettings.scenes)
        {
            Debug.Log($"   - {s.path} [Enabled: {s.enabled}]");
            if (s.path == current.path)
            {
                Debug.Log("   ✅ MATCH FOUND!");
            }
        }
        
        // Check Canvas tag
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Debug.Log($"🎨 Canvas found. Tag: {canvas.tag}. Layer: {LayerMask.LayerToName(canvas.layer)}");
            if (canvas.tag == "EditorOnly")
            {
                Debug.LogError("❌ CANVAS IS TAGGED 'EditorOnly'! IT WILL NOT BUILD!");
            }
        }
        else
        {
            Debug.LogError("❌ Canvas not found in scene!");
        }
    }
}


