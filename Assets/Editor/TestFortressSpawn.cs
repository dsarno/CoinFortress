using UnityEngine;
using UnityEditor;

public class TestFortressSpawn
{
    [MenuItem("Tools/Test Fortress Spawn")]
    public static void Test()
    {
        FortressSpawner spawner = Object.FindFirstObjectByType<FortressSpawner>();
        if (spawner == null)
        {
            Debug.LogError("FortressSpawner not found in scene!");
            return;
        }
        
        Debug.Log("=== TESTING FORTRESS SPAWN ===");
        Debug.Log($"Spawner: {spawner.name}");
        Debug.Log($"Layout: {spawner.layout?.name ?? "NULL"}");
        Debug.Log($"Spawn Point: {spawner.spawnPoint?.name ?? "NULL"}");
        Debug.Log($"Current fortressRoot: {spawner.fortressRoot?.name ?? "NULL"}");
        
        // Test spawn
        Debug.Log("\nCalling SpawnFortress()...");
        spawner.SpawnFortress();
        
        // Check result
        GameObject fortressRoot = GameObject.Find("FortressRoot");
        if (fortressRoot != null)
        {
            int childCount = fortressRoot.transform.childCount;
            Debug.Log($"✓ FortressRoot created with {childCount} children");
            
            if (childCount == 0)
            {
                Debug.LogWarning("⚠ FortressRoot exists but has no children! Check prefab assignments.");
            }
            else
            {
                // List first few blocks
                Debug.Log("\nFirst 5 blocks:");
                for (int i = 0; i < Mathf.Min(5, childCount); i++)
                {
                    Transform child = fortressRoot.transform.GetChild(i);
                    Vector3 pos = child.position;
                    Debug.Log($"  {i}: {child.name} at ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})");
                }
            }
        }
        else
        {
            Debug.LogError("✗ FortressRoot not found after spawn!");
        }
    }
}
