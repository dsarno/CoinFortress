using UnityEngine;
using UnityEditor;

public class CleanupCrackOverlays : MonoBehaviour
{
    private static readonly string[] PrefabPaths = new []
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

    [MenuItem("Tools/Cracks/Cleanup Prefabs and Scene (remove CrackOverlay)")]
    public static void Execute()
    {
        int prefabsCleaned = 0;
        foreach (var path in PrefabPaths)
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            if (root == null) continue;

            // Remove CrackOverlay child if present
            var overlay = root.transform.Find("CrackOverlay");
            if (overlay != null)
            {
                Object.DestroyImmediate(overlay.gameObject);
            }

            // Ensure a CrackEffect exists on root
            var effect = root.GetComponent<CrackEffect>();
            if (effect == null) root.AddComponent<CrackEffect>();

            // Ensure root has a SpriteRenderer (base block) and no extra SRs on children
            var sr = root.GetComponent<SpriteRenderer>();
            if (sr == null) root.AddComponent<SpriteRenderer>();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            prefabsCleaned++;
        }

        // Clean active scene
        var overlaysInScene = GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        int sceneRemoved = 0;
        foreach (var t in overlaysInScene)
        {
            if (t.name == "CrackOverlay")
            {
                Object.DestroyImmediate(t.gameObject);
                sceneRemoved++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ Cleanup complete. Prefabs cleaned: {prefabsCleaned}, Scene CrackOverlay removed: {sceneRemoved}");
    }
}
