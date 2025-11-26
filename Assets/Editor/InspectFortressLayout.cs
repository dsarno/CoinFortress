using UnityEngine;
using UnityEditor;

public class InspectFortressLayout
{
    [MenuItem("Tools/Inspect Fortress Layouts")]
    public static void Inspect()
    {
        // Find all FortressLayout assets
        string[] guids = AssetDatabase.FindAssets("t:FortressLayout");
        
        if (guids.Length == 0)
        {
            Debug.LogWarning("No FortressLayout assets found!");
            return;
        }
        
        Debug.Log($"=== FORTRESS LAYOUTS ({guids.Length} found) ===\n");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            FortressLayout layout = AssetDatabase.LoadAssetAtPath<FortressLayout>(path);
            
            if (layout != null)
            {
                Debug.Log($"Layout: {layout.name}");
                Debug.Log($"  Path: {path}");
                Debug.Log($"  Size: {layout.width}x{layout.height}");
                
                // Count block types
                int totalBlocks = 0;
                int emptyBlocks = 0;
                int stoneBlocks = 0;
                int treasureBlocks = 0;
                int windowBlocks = 0;
                int roofBlocks = 0;
                int turretBlocks = 0;
                
                for (int y = 0; y < layout.height; y++)
                {
                    for (int x = 0; x < layout.width; x++)
                    {
                        BlockType blockType = layout.GetCell(x, y);
                        totalBlocks++;
                        
                        switch (blockType)
                        {
                            case BlockType.Empty: emptyBlocks++; break;
                            case BlockType.Stone: stoneBlocks++; break;
                            case BlockType.TreasureChest: treasureBlocks++; break;
                            case BlockType.Window: windowBlocks++; break;
                            case BlockType.Roof: roofBlocks++; break;
                            case BlockType.Diamond: turretBlocks++; break; // Reusing variable for now
                        }
                    }
                }
                
                Debug.Log($"  Total Cells: {totalBlocks}");
                Debug.Log($"  Empty: {emptyBlocks}");
                Debug.Log($"  Stone: {stoneBlocks}");
                Debug.Log($"  Treasure: {treasureBlocks}");
                Debug.Log($"  Window: {windowBlocks}");
                Debug.Log($"  Roof: {roofBlocks}");
                Debug.Log($"  Non-empty blocks: {totalBlocks - emptyBlocks}\n");
            }
        }
        
        // Check which layout is assigned to FortressSpawner
        FortressSpawner spawner = Object.FindFirstObjectByType<FortressSpawner>();
        if (spawner != null && spawner.layout != null)
        {
            Debug.Log($"✓ FortressSpawner is using: {spawner.layout.name}");
        }
        else
        {
            Debug.LogWarning("FortressSpawner doesn't have a layout assigned!");
        }
    }
}
