using UnityEngine;
using UnityEditor;

public class LevelConfigTools : MonoBehaviour
{
    [MenuItem("Tools/Coin Fortress/Save Current Level Positions")]
    public static void SaveLevelPositions()
    {
        // Find LevelProgressionManager
        LevelProgressionManager progressionManager = FindFirstObjectByType<LevelProgressionManager>();
        if (progressionManager == null)
        {
            Debug.LogError("Could not find LevelProgressionManager in the scene.");
            return;
        }

        // Get Current Level Config
        LevelConfig currentConfig = progressionManager.GetCurrentLevel();
        if (currentConfig == null)
        {
            Debug.LogError("No active Level Config found in LevelProgressionManager.");
            return;
        }

        // Find FortressRoot
        GameObject fortressRoot = GameObject.Find("FortressRoot");
        if (fortressRoot != null)
        {
            // Update Fortress Position
            Undo.RecordObject(currentConfig, "Update Fortress Position");
            currentConfig.fortressSpawnPosition = fortressRoot.transform.position;
            Debug.Log($"Updated Fortress Spawn Position to {currentConfig.fortressSpawnPosition} for {currentConfig.name}");
        }
        else
        {
            Debug.LogWarning("FortressRoot not found in scene. Skipping fortress position update.");
        }

        // Find Player
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            // Update Player Position
            Undo.RecordObject(currentConfig, "Update Player Position");
            currentConfig.playerSpawnPosition = player.transform.position;
            Debug.Log($"Updated Player Spawn Position to {currentConfig.playerSpawnPosition} for {currentConfig.name}");
        }
        else
        {
            Debug.LogWarning("Player not found in scene. Skipping player position update.");
        }

        // Save Changes
        EditorUtility.SetDirty(currentConfig);
        AssetDatabase.SaveAssets();
    }
}
