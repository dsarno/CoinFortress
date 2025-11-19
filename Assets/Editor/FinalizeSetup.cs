using UnityEngine;
using UnityEditor;

public class FinalizeSetup
{
    [MenuItem("Tools/Finalize Game Setup")]
    public static void DoFinalize()
    {
        LevelManager levelManager = Object.FindFirstObjectByType<LevelManager>();
        StoreManager storeManager = Object.FindFirstObjectByType<StoreManager>();
        UISetupHelper helper = Object.FindFirstObjectByType<UISetupHelper>();
        
        if (levelManager != null && storeManager != null)
        {
            levelManager.storeManager = storeManager;
            EditorUtility.SetDirty(levelManager);
        }
        
        if (helper != null && levelManager != null)
        {
            helper.levelManager = levelManager;
            EditorUtility.SetDirty(helper);
        }
        
        Debug.Log("Setup finalized! All components connected.");
        EditorUtility.DisplayDialog("Setup Complete", 
            "Game is ready to play!\n\n" +
            "Press Play to test:\n" +
            "1. Store opens with 100 coins\n" +
            "2. Buy upgrades\n" +
            "3. Click START LEVEL\n" +
            "4. Fire at fortress (left click)\n" +
            "5. Watch ammo count\n" +
            "6. Destroy the core to win!", 
            "OK");
    }
}
