using UnityEngine;

public class FortressSpawner : MonoBehaviour
{
    [Header("Layout")]
    public FortressLayout layout;
    
    [Header("Block Prefabs")]
    public GameObject stonePrefab;
    public GameObject ironPrefab;
    public GameObject goldPrefab;
    public GameObject corePrefab;
    public GameObject turretPrefab;
    public GameObject windowPrefab;
    public GameObject roofPrefab;
    public GameObject treasureChestPrefab;
    
    [Header("Settings")]
    public float cellSize = 1f;
    public Transform spawnPoint; // Lower-right brick will be placed here
    
    [Header("Runtime")]
    public Transform fortressRoot;
    
    public void SpawnFortress()
    {
        if (layout == null)
        {
            Debug.LogError("No fortress layout assigned!");
            return;
        }
        
        // Create or find fortress root
        if (fortressRoot == null)
        {
            if (spawnPoint != null)
            {
                Transform existingRoot = spawnPoint.Find("FortressRoot");
                if (existingRoot != null)
                {
                    fortressRoot = existingRoot;
                }
                else
                {
                    GameObject rootObj = new GameObject("FortressRoot");
                    fortressRoot = rootObj.transform;
                    fortressRoot.SetParent(spawnPoint);
                    fortressRoot.localPosition = Vector3.zero;
                }
            }
            else
            {
                GameObject rootObj = new GameObject("FortressRoot");
                fortressRoot = rootObj.transform;
            }
        }
        
        // Clear existing fortress
        ClearFortress();
        
        // Spawn blocks from layout
        for (int y = 0; y < layout.height; y++)
        {
            for (int x = 0; x < layout.width; x++)
            {
                BlockType blockType = layout.GetCell(x, y);
                if (blockType == BlockType.Empty)
                    continue;
                
                GameObject prefab = GetPrefabForType(blockType);
                if (prefab == null)
                {
                    Debug.LogWarning($"No prefab assigned for {blockType}!");
                    continue;
                }
                
                // Calculate position relative to spawn point
                // Lower-right brick (width-1, 0) is at spawn point (0, 0)
                Vector3 spawnPos;
                if (spawnPoint != null)
                {
                    float xPos = (x - (layout.width - 1)) * cellSize;
                    float yPos = y * cellSize;
                    spawnPos = spawnPoint.TransformPoint(new Vector3(xPos, yPos, 0f));
                }
                else
                {
                    // Fallback: centered positioning
                    float xPos = (x - layout.width / 2f + 0.5f) * cellSize;
                    float yPos = y * cellSize;
                    spawnPos = new Vector3(xPos, yPos, 0f);
                }
                
                // Spawn block
                GameObject block = Instantiate(prefab, spawnPos, Quaternion.identity, fortressRoot);
                block.name = $"{blockType}_{x}_{y}";
                
                // Configure block component
                FortressBlock blockScript = block.GetComponent<FortressBlock>();
                if (blockScript != null)
                {
                    blockScript.blockType = blockType;
                }
            }
        }
        
        Debug.Log($"Spawned fortress: {layout.width}x{layout.height} grid");
    }
    
    public void ClearFortress()
    {
        if (fortressRoot == null)
            return;
        
        // Destroy all children
        while (fortressRoot.childCount > 0)
        {
            DestroyImmediate(fortressRoot.GetChild(0).gameObject);
        }
    }
    
    private GameObject GetPrefabForType(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.Stone: return stonePrefab;
            case BlockType.Iron: return ironPrefab;
            case BlockType.Gold: return goldPrefab;
            case BlockType.Core: return corePrefab;
            case BlockType.Turret: return turretPrefab;
            case BlockType.Window: return windowPrefab;
            case BlockType.Roof: return roofPrefab;
            case BlockType.TreasureChest: return treasureChestPrefab;
            default: return null;
        }
    }
    
    private void OnValidate()
    {
        cellSize = Mathf.Max(0.1f, cellSize);
    }
}