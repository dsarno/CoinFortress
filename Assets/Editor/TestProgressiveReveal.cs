using UnityEngine;
using UnityEditor;
using System.Collections;

public class TestProgressiveReveal : MonoBehaviour
{
    [MenuItem("Tools/Test Progressive Crack Reveal (Simulated Hits)")]
    public static void Execute()
    {
        // Find a test block
        FortressBlock[] blocks = Object.FindObjectsByType<FortressBlock>(FindObjectsSortMode.None);
        
        if (blocks.Length == 0)
        {
            Debug.LogWarning("No fortress blocks found! Spawn fortress first.");
            return;
        }
        
        // Find a block with 2+ HP for testing
        FortressBlock testBlock = null;
        foreach (var block in blocks)
        {
            if (block.maxHP >= 2)
            {
                testBlock = block;
                break;
            }
        }
        
        if (testBlock == null)
        {
            testBlock = blocks[0]; // Fallback to any block
        }
        
        // Reset the block
        testBlock.currentHP = testBlock.maxHP;
        CrackEffect crackEffect = testBlock.GetComponent<CrackEffect>();
        if (crackEffect != null)
        {
            crackEffect.UpdateCrackProgress(0f);
        }
        
        Debug.Log($"Testing progressive reveal on {testBlock.name} (HP: {testBlock.maxHP})");
        Debug.Log("Dealing damage every 2 seconds...");
        
        // Deal damage progressively
        EditorApplication.delayCall += () => SimulateDamage(testBlock, 1);
    }
    
    private static void SimulateDamage(FortressBlock block, int hitCount)
    {
        if (block == null) return;
        
        block.TakeDamage(1);
        Debug.Log($"💥 Hit {hitCount}: {block.name} HP = {block.currentHP}/{block.maxHP} | Crack Progress = {(1f - (float)block.currentHP / block.maxHP) * 100}%");
        
        if (block.currentHP > 0)
        {
            // Schedule next hit in 2 seconds (2000ms delay calls)
            for (int i = 0; i < 200; i++)
            {
                int capturedCount = hitCount;
                EditorApplication.delayCall += () => { };
            }
            
            // After delays, do next hit
            EditorApplication.delayCall += () => SimulateDamage(block, hitCount + 1);
        }
        else
        {
            Debug.Log("✅ Block destroyed! Progressive reveal test complete.");
        }
    }
}
