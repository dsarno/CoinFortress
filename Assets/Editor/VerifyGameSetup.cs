using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VerifyGameSetup
{
    [MenuItem("Tools/Verify Complete Setup")]
    public static void Verify()
    {
        bool allGood = true;
        System.Text.StringBuilder report = new System.Text.StringBuilder();
        report.AppendLine("=== GAME SETUP VERIFICATION ===\n");
        
        // Check PlayerCannonController
        PlayerCannonController cannon = Object.FindFirstObjectByType<PlayerCannonController>();
        if (cannon != null)
        {
            report.AppendLine("✓ PlayerCannonController found");
            
            // Check if it has the IsPointerOverUI method (indicates new code)
            var method = cannon.GetType().GetMethod("IsPointerOverUI", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                report.AppendLine("  ✓ Has IsPointerOverUI() method");
            }
            else
            {
                report.AppendLine("  ✗ Missing IsPointerOverUI() method - needs update!");
                allGood = false;
            }
        }
        else
        {
            report.AppendLine("✗ PlayerCannonController not found!");
            allGood = false;
        }
        
        // Check EventSystem
        EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
        if (eventSystem != null)
        {
            report.AppendLine("\n✓ EventSystem found");
            
            var inputModule = eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            if (inputModule != null)
            {
                report.AppendLine("  ✓ Has InputSystemUIInputModule");
            }
            else
            {
                report.AppendLine("  ✗ Missing InputSystemUIInputModule!");
                allGood = false;
            }
            
            var oldModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (oldModule == null)
            {
                report.AppendLine("  ✓ No old StandaloneInputModule");
            }
            else
            {
                report.AppendLine("  ⚠ Still has old StandaloneInputModule - should remove!");
            }
        }
        else
        {
            report.AppendLine("\n✗ EventSystem not found!");
            allGood = false;
        }
        
        // Check Store UI
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            report.AppendLine("\n✓ Canvas found");
            
            Transform storeRoot = canvas.transform.Find("Store Root");
            if (storeRoot != null)
            {
                report.AppendLine("  ✓ Store Root found");
                
                Transform blocker = storeRoot.Find("Store Blocker");
                if (blocker != null)
                {
                    report.AppendLine("    ✓ Store Blocker found");
                    
                    Image blockerImage = blocker.GetComponent<Image>();
                    if (blockerImage != null && blockerImage.raycastTarget)
                    {
                        report.AppendLine("      ✓ Blocker blocks raycasts");
                    }
                    else
                    {
                        report.AppendLine("      ✗ Blocker doesn't block raycasts!");
                        allGood = false;
                    }
                }
                else
                {
                    report.AppendLine("    ✗ Store Blocker not found!");
                    allGood = false;
                }
                
                Transform storePanel = storeRoot.Find("Store Panel");
                if (storePanel != null)
                {
                    report.AppendLine("    ✓ Store Panel found");
                    
                    Image panelImage = storePanel.GetComponent<Image>();
                    if (panelImage != null && panelImage.raycastTarget)
                    {
                        report.AppendLine("      ✓ Panel blocks raycasts");
                    }
                }
            }
            else
            {
                report.AppendLine("  ✗ Store Root not found!");
                allGood = false;
            }
        }
        else
        {
            report.AppendLine("\n✗ Canvas not found!");
            allGood = false;
        }
        
        // Check LevelManager
        LevelManager levelManager = Object.FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            report.AppendLine("\n✓ LevelManager found");
            
            if (levelManager.playerStats != null)
            {
                report.AppendLine("  ✓ PlayerStats assigned");
            }
            else
            {
                report.AppendLine("  ✗ PlayerStats not assigned!");
                allGood = false;
            }
            
            if (levelManager.storeManager != null)
            {
                report.AppendLine("  ✓ StoreManager assigned");
            }
            else
            {
                report.AppendLine("  ✗ StoreManager not assigned!");
                allGood = false;
            }
        }
        else
        {
            report.AppendLine("\n✗ LevelManager not found!");
            allGood = false;
        }
        
        // Check StoreManager
        StoreManager storeManager = Object.FindFirstObjectByType<StoreManager>();
        if (storeManager != null)
        {
            report.AppendLine("\n✓ StoreManager found");
            
            if (storeManager.playerStats != null)
            {
                report.AppendLine("  ✓ PlayerStats assigned");
            }
            
            if (storeManager.storePanel != null)
            {
                report.AppendLine("  ✓ Store Panel assigned");
                if (storeManager.storePanel.name == "Store Root")
                {
                    report.AppendLine("    ✓ Points to Store Root (correct)");
                }
            }
        }
        
        // Final verdict
        report.AppendLine("\n" + new string('=', 40));
        if (allGood)
        {
            report.AppendLine("✓ ALL CHECKS PASSED!");
            report.AppendLine("\nYour game is ready to play:");
            report.AppendLine("1. Press Play");
            report.AppendLine("2. Store should open");
            report.AppendLine("3. Click buttons to test (shouldn't fire cannon)");
            report.AppendLine("4. Click START LEVEL");
            report.AppendLine("5. Fire at fortress (should work)");
            report.AppendLine("6. Destroy core to complete level");
        }
        else
        {
            report.AppendLine("⚠ SOME ISSUES FOUND!");
            report.AppendLine("\nTry running these from Tools menu:");
            report.AppendLine("- Fix Store UI Input");
            report.AppendLine("- Fix EventSystem Input");
            report.AppendLine("- Finalize Game Setup");
        }
        
        Debug.Log(report.ToString());
        
        EditorUtility.DisplayDialog(
            allGood ? "Setup Verified ✓" : "Issues Found ⚠",
            report.ToString(),
            "OK"
        );
    }
}
