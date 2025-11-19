using UnityEngine;
using UnityEditor;

public class AssignCrackDatabase : MonoBehaviour
{
    public static void Execute()
    {
        // Load the database
        CrackSpritesDatabase database = AssetDatabase.LoadAssetAtPath<CrackSpritesDatabase>("Assets/Data/CrackSpritesDatabase.asset");
        
        if (database == null)
        {
            Debug.LogError("CrackSpritesDatabase not found at Assets/Data/CrackSpritesDatabase.asset");
            return;
        }
        
        // Find CrackEffectManager
        CrackEffectManager manager = GameObject.FindFirstObjectByType<CrackEffectManager>();
        
        if (manager == null)
        {
            Debug.LogError("CrackEffectManager not found in scene!");
            return;
        }
        
        // Assign the database
        manager.spritesDatabase = database;
        EditorUtility.SetDirty(manager);
        
        Debug.Log("Assigned CrackSpritesDatabase to CrackEffectManager");
        
        // Save scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
    }
}
