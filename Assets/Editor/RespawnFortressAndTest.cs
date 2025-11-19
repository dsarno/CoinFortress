using UnityEngine;
using UnityEditor;

public class RespawnFortressAndTest : MonoBehaviour
{
    [MenuItem("Tools/Respawn Fortress and Test Cracks")]
    public static void Execute()
    {
        // Find the fortress spawner
        FortressSpawner spawner = Object.FindFirstObjectByType<FortressSpawner>();
        if (spawner == null)
        {
            Debug.LogError("No FortressSpawner found in scene!");
            return;
        }
        
        // Clear existing fortress
        spawner.ClearFortress();
        Debug.Log("Cleared old fortress");
        
        // Spawn new fortress
        spawner.SpawnFortress();
        Debug.Log("Spawned new fortress");
        
        // Wait for spawning to complete and Awake to be called
        EditorApplication.delayCall += () =>
        {
            // Add another delay to ensure Awake has been called
            EditorApplication.delayCall += () =>
            {
                TestCracksOnNewFortress();
            };
        };
    }
    
    private static void TestCracksOnNewFortress()
    {
        // Find all FortressBlock objects
        FortressBlock[] blocks = Object.FindObjectsByType<FortressBlock>(FindObjectsSortMode.None);
        
        if (blocks.Length == 0)
        {
            Debug.LogWarning("No FortressBlock objects found after spawn!");
            return;
        }
        
        Debug.Log($"Found {blocks.Length} fortress blocks. Testing crack shader...");
        
        // Manually call Awake on all CrackEffect components (needed in edit mode)
        CrackEffect[] allCracks = Object.FindObjectsByType<CrackEffect>(FindObjectsSortMode.None);
        foreach (var crack in allCracks)
        {
            crack.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
        }
        
        // Apply progressive crack damage to first 10 blocks
        for (int i = 0; i < Mathf.Min(blocks.Length, 10); i++)
        {
            FortressBlock block = blocks[i];
            CrackEffect crackEffect = block.GetComponent<CrackEffect>();
            
            if (crackEffect == null)
            {
                Debug.LogWarning($"Block '{block.name}' has no CrackEffect component!");
                continue;
            }
            
            // Test different damage levels: 0%, 25%, 50%, 75%, 100%
            float damagePercent = (i % 5) * 0.25f;
            if (damagePercent == 0) damagePercent = 0.25f; // Skip 0% for first block
            
            crackEffect.UpdateCrackProgress(damagePercent);
            
            Debug.Log($"✅ Block '{block.name}': Crack progress = {damagePercent * 100}%");
        }
        
        Debug.Log("🎨 Crack shader test complete! Check scene view for cracks with progressive reveal, thickening, and glowing emission.");
    }
}
