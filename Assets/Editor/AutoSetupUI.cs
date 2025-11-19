using UnityEngine;
using UnityEditor;

public class AutoSetupUI
{
    [MenuItem("Tools/Setup Game UI")]
    public static void SetupUI()
    {
        UISetupHelper helper = Object.FindFirstObjectByType<UISetupHelper>();
        if (helper == null)
        {
            Debug.LogError("UISetupHelper not found in scene! Please add it to a GameObject first.");
            return;
        }
        
        helper.SetupCompleteUI();
        
        // Wire up final references
        LevelManager levelManager = helper.GetComponent<LevelManager>();
        StoreManager storeManager = Object.FindFirstObjectByType<StoreManager>();
        
        if (levelManager != null && storeManager != null)
        {
            levelManager.storeManager = storeManager;
            EditorUtility.SetDirty(levelManager);
        }
        
        Debug.Log("Game UI setup complete! Ready to play.");
    }
}
