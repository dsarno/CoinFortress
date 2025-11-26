using UnityEngine;
using UnityEditor;

public class SaveLevelBackgroundTransform : MonoBehaviour
{
    [MenuItem("Tools/Save Level Background Transform")]
    public static void Execute()
    {
        // Find LevelProgressionManager
        LevelProgressionManager progressionManager = Object.FindFirstObjectByType<LevelProgressionManager>();
        if (progressionManager == null)
        {
            Debug.LogError("LevelProgressionManager not found in scene!");
            return;
        }

        // Get current level config
        LevelConfig currentConfig = progressionManager.GetCurrentLevel();
        if (currentConfig == null)
        {
            Debug.LogError("No current level config loaded in LevelProgressionManager!");
            return;
        }

        // Find Background object
        GameObject background = GameObject.Find("Background");
        if (background == null)
        {
            Debug.LogError("Background object not found in scene!");
            return;
        }

        // Save transform to config
        Undo.RecordObject(currentConfig, "Save Background Transform");
        currentConfig.backgroundPosition = background.transform.position;
        currentConfig.backgroundRotation = background.transform.rotation;
        currentConfig.backgroundScale = background.transform.localScale;
        
        EditorUtility.SetDirty(currentConfig);
        AssetDatabase.SaveAssets();

        Debug.Log($"Saved Background Transform for {currentConfig.levelName}:\nPos: {currentConfig.backgroundPosition}\nRot: {currentConfig.backgroundRotation.eulerAngles}\nScale: {currentConfig.backgroundScale}");
    }
}
