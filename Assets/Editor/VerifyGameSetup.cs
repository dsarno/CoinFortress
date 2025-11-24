using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class VerifyGameSetup : MonoBehaviour
{
    [MenuItem("Tools/Verify & Fix Game Setup")]
    public static void VerifySetup()
    {
        Debug.Log("--- Starting Game Setup Verification ---");
        
        // 1. Check for EventSystem
        var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogWarning("❌ Missing EventSystem! Creating one...");
            GameObject go = new GameObject("EventSystem");
            eventSystem = go.AddComponent<UnityEngine.EventSystems.EventSystem>();
            go.AddComponent<InputSystemUIInputModule>();
            Debug.Log("✅ Created EventSystem with InputSystemUIInputModule");
        }
        else
        {
            // Check for old StandaloneInputModule
            var oldModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (oldModule != null)
            {
                Debug.LogWarning("❌ Found old StandaloneInputModule! Replacing with InputSystemUIInputModule...");
                DestroyImmediate(oldModule);
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                Debug.Log("✅ Replaced Input Module");
            }
            
            // Check for new InputSystemUIInputModule
            var newModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (newModule == null)
            {
                Debug.LogWarning("❌ Missing InputSystemUIInputModule! Adding it...");
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                Debug.Log("✅ Added InputSystemUIInputModule");
            }
            else
            {
                 Debug.Log("✅ EventSystem looks correct.");
            }
        }

        // 2. Check Canvas Scaler
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }
            
            if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                Debug.LogWarning("⚠️ Canvas Scaler was not 'Scale With Screen Size'. Fixing...");
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
                Debug.Log("✅ Fixed Canvas Scaler settings");
            }
            else
            {
                Debug.Log("✅ Canvas Scaler looks correct.");
            }
        }
        else
        {
            Debug.LogError("❌ No Canvas found in scene!");
        }

        // 3. Check MainMenuController
        var mainMenu = Object.FindFirstObjectByType<MainMenuController>();
        if (mainMenu == null)
        {
            Debug.LogError("❌ MainMenuController script is missing from the scene!");
            // Attempt to find the Main Menu Panel to attach it to
            if (canvas != null)
            {
                Transform panel = canvas.transform.Find("Main Menu Panel");
                if (panel != null)
                {
                    Debug.Log("   Found 'Main Menu Panel'. Attaching script there...");
                    mainMenu = panel.gameObject.AddComponent<MainMenuController>();
                    mainMenu.mainMenuPanel = panel.gameObject;
                    mainMenu.startGameButton = panel.GetComponentInChildren<Button>();
                    Debug.Log("✅ Attached MainMenuController and auto-assigned references.");
                }
            }
        }
        else
        {
             Debug.Log("✅ MainMenuController found.");
             if (mainMenu.mainMenuPanel == null) Debug.LogWarning("⚠️ MainMenuController.mainMenuPanel is unassigned!");
             if (mainMenu.startGameButton == null) Debug.LogWarning("⚠️ MainMenuController.startGameButton is unassigned!");
        }

        Debug.Log("--- Verification Complete. Save your scene! ---");
    }
}
