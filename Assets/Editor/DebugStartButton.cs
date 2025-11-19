using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class DebugStartButton
{
    [MenuItem("Tools/Debug Start Button")]
    public static void Debug()
    {
        // Find the button
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            UnityEngine.Debug.LogError("Canvas not found!");
            return;
        }
        
        Transform storeRoot = canvas.transform.Find("Store Root");
        if (storeRoot == null)
        {
            UnityEngine.Debug.LogError("Store Root not found!");
            return;
        }
        
        Transform storePanel = storeRoot.Find("Store Panel");
        if (storePanel == null)
        {
            UnityEngine.Debug.LogError("Store Panel not found!");
            return;
        }
        
        Transform startButtonTransform = storePanel.Find("Start Level Button");
        if (startButtonTransform == null)
        {
            UnityEngine.Debug.LogError("Start Level Button not found!");
            return;
        }
        
        Button button = startButtonTransform.GetComponent<Button>();
        if (button == null)
        {
            UnityEngine.Debug.LogError("Button component not found!");
            return;
        }
        
        UnityEngine.Debug.Log($"=== START BUTTON DEBUG ===");
        UnityEngine.Debug.Log($"Button found: {button.name}");
        UnityEngine.Debug.Log($"Button interactable: {button.interactable}");
        UnityEngine.Debug.Log($"Button onClick listener count: {button.onClick.GetPersistentEventCount()}");
        
        // Check each listener
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            var target = button.onClick.GetPersistentTarget(i);
            var methodName = button.onClick.GetPersistentMethodName(i);
            UnityEngine.Debug.Log($"  Listener {i}: Target={target?.name ?? "NULL"}, Method={methodName}");
        }
        
        // Find LevelManager
        LevelManager levelManager = Object.FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            UnityEngine.Debug.Log($"LevelManager found: {levelManager.name}");
            UnityEngine.Debug.Log($"LevelManager.levelInProgress: {levelManager.levelInProgress}");
            
            // Clear and re-add listener using UnityEvent
            button.onClick.RemoveAllListeners();
            
            // Add as persistent listener
            UnityEditor.Events.UnityEventTools.AddPersistentListener(
                button.onClick, 
                levelManager.BeginLevel
            );
            
            EditorUtility.SetDirty(button);
            UnityEngine.Debug.Log("✓ Listener added as PERSISTENT!");
            
            // Verify it was added
            UnityEngine.Debug.Log($"After fix - listener count: {button.onClick.GetPersistentEventCount()}");
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                var target = button.onClick.GetPersistentTarget(i);
                var methodName = button.onClick.GetPersistentMethodName(i);
                UnityEngine.Debug.Log($"  Listener {i}: Target={target?.name ?? "NULL"}, Method={methodName}");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("LevelManager not found!");
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
