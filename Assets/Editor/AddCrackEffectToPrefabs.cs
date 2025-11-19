using UnityEngine;
using UnityEditor;

public class AddCrackEffectToPrefabs : MonoBehaviour
{
    [MenuItem("Tools/Add CrackEffect to All Block Prefabs")]
    public static void Execute()
    {
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
        
        int addedCount = 0;
        int skippedCount = 0;
        
        foreach (string path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab not found: {path}");
                continue;
            }
            
            // Check if already has CrackEffect
            CrackEffect existingCrack = prefab.GetComponent<CrackEffect>();
            if (existingCrack != null)
            {
                Debug.Log($"✓ {prefab.name} already has CrackEffect");
                skippedCount++;
                continue;
            }
            
            // Add CrackEffect component to prefab
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
            CrackEffect crackEffect = prefabRoot.AddComponent<CrackEffect>();
            
            // Save the modified prefab
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            
            Debug.Log($"✅ Added CrackEffect to {prefab.name}");
            addedCount++;
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"🎨 CrackEffect setup complete! Added to {addedCount} prefabs, {skippedCount} already had it.");
    }
}
