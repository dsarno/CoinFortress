using UnityEngine;
using UnityEditor;

public class FixLevelColors : MonoBehaviour
{
    [MenuItem("Tools/Fix Level Background Colors")]
    public static void FixColors()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelConfig");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelConfig config = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
            
            if (config != null)
            {
                // Check if it has the old default dark color (approx 0.2, 0.2, 0.3)
                // 0.2 = 51/255, 0.3 = 76.5/255
                if (config.backgroundColor.r < 0.9f && config.backgroundColor.g < 0.9f && config.backgroundColor.b < 0.9f)
                {
                    config.backgroundColor = Color.white;
                    EditorUtility.SetDirty(config);
                    count++;
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"Fixed background color for {count} LevelConfig assets.");
    }
}
