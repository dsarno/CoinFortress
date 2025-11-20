using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UpdateButtonColor : MonoBehaviour
{
    [MenuItem("Tools/Tint Buttons Gold")]
    public static void TintButtons()
    {
        // Gold color approximation from the logo (Vibrant Yellow-Orange)
        Color goldColor = new Color(1.0f, 0.8f, 0.1f, 1.0f);
        
        // Find all buttons in the scene (including inactive ones if we look at root objects, 
        // but Resources.FindObjectsOfTypeAll is better for editor scripts to catch everything)
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        
        int count = 0;
        foreach (Button btn in buttons)
        {
            // Skip assets in the project window
            if (EditorUtility.IsPersistent(btn.gameObject)) continue;
            
            // Skip if the button is part of a prefab that shouldn't be touched (optional, but good practice)
            // For now, we want to update scene buttons.

            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.color = goldColor;
                count++;
            }
            
            // Ensure text is readable (Dark Brown/Black looks good on Gold)
            Text txt = btn.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.color = new Color(0.2f, 0.1f, 0.0f, 1.0f); // Dark Brown
            }
        }

        Debug.Log($"Updated {count} buttons to Gold color.");
    }
}
