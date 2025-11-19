using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class FixEventSystem
{
    [MenuItem("Tools/Fix EventSystem Input")]
    public static void Fix()
    {
        EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("No EventSystem found!");
            return;
        }
        
        // Remove old standalone input module if present
        var oldModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (oldModule != null)
        {
            Object.DestroyImmediate(oldModule);
            Debug.Log("Removed old StandaloneInputModule");
        }
        
        // Add new Input System UI Input Module
        var newModule = eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (newModule == null)
        {
            eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            Debug.Log("Added InputSystemUIInputModule");
        }
        
        EditorUtility.SetDirty(eventSystem);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("EventSystem fixed for new Input System!");
    }
}
