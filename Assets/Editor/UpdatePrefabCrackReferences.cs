using UnityEngine;
using UnityEditor;

public class UpdatePrefabCrackReferences : MonoBehaviour
{
    [MenuItem("Tools/Update Prefab Crack References")]
    public static void Execute()
    {
        // Load the database
        CrackSpritesDatabase database = AssetDatabase.LoadAssetAtPath<CrackSpritesDatabase>("Assets/Data/CrackSpritesDatabase.asset");
        if (database == null)
        {
            Debug.LogError("CrackSpritesDatabase not found!");
            return;
        }
        
        string[] prefabPaths = new string[]
        {
            "Assets/Prefabs/CoreBlock.prefab",
            "Assets/Prefabs/GoldBlock.prefab",
            "Assets/Prefabs/IronBlock.prefab",
            "Assets/Prefabs/RoofBlock.prefab",
            "Assets/Prefabs/StoneBlock.prefab",
            "Assets/Prefabs/TreasureChestBlock.prefab",
            "Assets/Prefabs/TurretBlock.prefab",
            "Assets/Prefabs/WindowBlock.prefab"
        };
        
        int updatedCount = 0;
        
        foreach (string path in prefabPaths)
        {
            // Load prefab contents
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
            
            // Get CrackEffect component
            CrackEffect crackEffect = prefabRoot.GetComponent<CrackEffect>();
            if (crackEffect != null)
            {
                // Set the database reference
                crackEffect.spritesDatabase = database;
                
                // Save the modified prefab
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                
                Debug.Log($"✅ Updated {prefabRoot.name} with crack database reference");
                updatedCount++;
            }
            else
            {
                Debug.LogWarning($"⚠️ {prefabRoot.name} has no CrackEffect component!");
            }
            
            // Unload prefab
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"🎨 Updated {updatedCount} prefabs with crack database references!");
    }
}
