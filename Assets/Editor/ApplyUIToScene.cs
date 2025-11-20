using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ApplyUIToScene : MonoBehaviour
{
    [MenuItem("Tools/Apply UI To Scene")]
    public static void ApplyUI()
    {
        Sprite panelSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/UI_Panel_Stone.png");
        Sprite buttonSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/UI_Button_Stone.png");

        if (panelSprite == null || buttonSprite == null)
        {
            Debug.LogError("Could not load UI sprites.");
            return;
        }

        // Apply to Panels
        ApplySpriteToGameObject("Canvas/Store Root/Store Panel", panelSprite);
        ApplySpriteToGameObject("Canvas/Main Menu Panel", panelSprite);

        // Apply to Buttons
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button btn in buttons)
        {
            // Check if it's a prefab asset or in the scene
            if (EditorUtility.IsPersistent(btn.gameObject)) continue;

            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = buttonSprite;
                img.type = Image.Type.Sliced;
                Debug.Log($"Applied button sprite to: {btn.name}");
            }

            Text btnText = btn.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.color = new Color(0.2f, 0.2f, 0.2f, 1.0f); // Dark Grey
                Debug.Log($"Updated text color for: {btn.name}");
            }
        }

        // Update all other text to be readable on stone
        Text[] allTexts = Resources.FindObjectsOfTypeAll<Text>();
        foreach (Text txt in allTexts)
        {
            if (EditorUtility.IsPersistent(txt.gameObject)) continue;
            
            // Skip if it's a button text we already handled (though setting it again is fine)
            
            // Default dark color
            txt.color = new Color(0.15f, 0.15f, 0.15f, 1.0f);

            // Special handling for Titles
            if (txt.name.Contains("Title"))
            {
                txt.fontStyle = FontStyle.Bold;
                txt.fontSize = Mathf.Max(txt.fontSize, 36); // Ensure titles are big
                
                // Add Shadow if not present
                if (txt.GetComponent<Shadow>() == null)
                {
                    Shadow shadow = txt.gameObject.AddComponent<Shadow>();
                    shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
                    shadow.effectDistance = new Vector2(2, -2);
                }
            }
        }

        // Fix HUD Text (should be light with shadow)
        GameObject hudPanel = GameObject.Find("Canvas/HUD Panel");
        if (hudPanel != null)
        {
            Text[] hudTexts = hudPanel.GetComponentsInChildren<Text>();
            foreach (Text txt in hudTexts)
            {
                txt.color = Color.white;
                txt.fontStyle = FontStyle.Bold;
                
                if (txt.GetComponent<Shadow>() == null)
                {
                    Shadow shadow = txt.gameObject.AddComponent<Shadow>();
                    shadow.effectColor = new Color(0f, 0f, 0f, 1f);
                    shadow.effectDistance = new Vector2(2, -2);
                }
                else
                {
                    Shadow shadow = txt.GetComponent<Shadow>();
                    shadow.effectColor = new Color(0f, 0f, 0f, 1f);
                }
                Debug.Log($"Fixed HUD text: {txt.name}");
            }
        }

        Debug.Log("UI Application Complete.");
    }

    private static void ApplySpriteToGameObject(string path, Sprite sprite)
    {
        GameObject go = GameObject.Find(path);
        if (go != null)
        {
            Image img = go.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprite;
                img.type = Image.Type.Sliced;
                // Reset color to white to show the sprite's color
                img.color = Color.white; 
                Debug.Log($"Applied panel sprite to: {path}");
            }
            else
            {
                Debug.LogWarning($"No Image component found on: {path}");
            }
        }
        else
        {
            Debug.LogWarning($"GameObject not found: {path}");
        }
    }
}
