using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixStartButton
{
    [MenuItem("Tools/Fix Start Level Button")]
    public static void Fix()
    {
        // Find components
        LevelManager levelManager = Object.FindFirstObjectByType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found!");
            return;
        }
        
        // Find the Start Level button
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found!");
            return;
        }
        
        Transform storeRoot = canvas.transform.Find("Store Root");
        if (storeRoot == null)
        {
            Debug.LogError("Store Root not found!");
            return;
        }
        
        Transform storePanel = storeRoot.Find("Store Panel");
        if (storePanel == null)
        {
            Debug.LogError("Store Panel not found!");
            return;
        }
        
        Transform startButton = storePanel.Find("Start Level Button");
        if (startButton == null)
        {
            Debug.LogError("Start Level Button not found!");
            return;
        }
        
        Button button = startButton.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component not found!");
            return;
        }
        
        // Clear existing listeners
        button.onClick.RemoveAllListeners();
        
        // Add the correct listener
        button.onClick.AddListener(levelManager.BeginLevel);
        
        Debug.Log("Start Level button fixed! Now connected to LevelManager.BeginLevel()");
        
        // Mark as dirty so it saves
        EditorUtility.SetDirty(button);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
