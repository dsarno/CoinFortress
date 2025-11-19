using UnityEngine;
using UnityEditor;

public class FixCrackReferences : MonoBehaviour
{
    [MenuItem("Tools/Fix Crack Database References")]
    public static void Execute()
    {
        // Find the manager
        CrackEffectManager manager = Object.FindFirstObjectByType<CrackEffectManager>();
        if (manager == null)
        {
            Debug.LogError("CrackEffectManager not found!");
            return;
        }
        
        // Load the database
        CrackSpritesDatabase database = AssetDatabase.LoadAssetAtPath<CrackSpritesDatabase>("Assets/Data/CrackSpritesDatabase.asset");
        if (database == null)
        {
            Debug.LogError("CrackSpritesDatabase not found!");
            return;
        }
        
        // Ensure manager has the database
        manager.spritesDatabase = database;
        EditorUtility.SetDirty(manager);
        
        // Find all CrackEffect components and assign database
        CrackEffect[] allCracks = Object.FindObjectsByType<CrackEffect>(FindObjectsSortMode.None);
        int fixedCount = 0;
        
        foreach (var crack in allCracks)
        {
            crack.spritesDatabase = database;
            fixedCount++;
        }
        
        Debug.Log($"✅ Fixed {fixedCount} CrackEffect components with database reference");
        Debug.Log($"   Database has {database.crackSprites?.Length ?? 0} crack sprites");
    }
}
