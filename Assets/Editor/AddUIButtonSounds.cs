using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class AddUIButtonSounds : MonoBehaviour
{
    public static void Execute()
    {
        // Find all buttons in the scene
        Button[] allButtons = GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        int count = 0;
        foreach (Button button in allButtons)
        {
            // Check if it already has the component
            if (button.GetComponent<UIButtonSounds>() == null)
            {
                button.gameObject.AddComponent<UIButtonSounds>();
                count++;
                Debug.Log($"Added UIButtonSounds to: {button.gameObject.name}");
            }
        }
        
        Debug.Log($"Added UIButtonSounds component to {count} buttons");
        
        // Mark scene as dirty so changes are saved
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
    }
}
