using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixTitleLogo : MonoBehaviour
{
    [MenuItem("Tools/Fix Title Logo")]
    public static void Fix()
    {
        GameObject titleTextObj = GameObject.Find("Canvas/Main Menu Panel/Title Text");
        if (titleTextObj == null)
        {
            Debug.LogError("Title Text object not found.");
            return;
        }

        // Create a new object for the logo
        GameObject logoObj = GameObject.Find("Canvas/Main Menu Panel/Title Logo");
        if (logoObj == null)
        {
            logoObj = new GameObject("Title Logo");
            logoObj.transform.SetParent(titleTextObj.transform.parent, false);
        }

        // Setup Image
        Image img = logoObj.GetComponent<Image>();
        if (img == null) img = logoObj.AddComponent<Image>();

        string path = "Assets/Sprites/UI/Title_Logo.png";
        Sprite logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        
        if (logoSprite != null)
        {
            img.sprite = logoSprite;
            img.preserveAspect = true;
            img.SetNativeSize();

            RectTransform rt = logoObj.GetComponent<RectTransform>();
            // Position it where the text was
            RectTransform textRT = titleTextObj.GetComponent<RectTransform>();
            rt.anchoredPosition = textRT.anchoredPosition;
            rt.anchorMin = textRT.anchorMin;
            rt.anchorMax = textRT.anchorMax;
            rt.pivot = textRT.pivot;
            
            // Scale down if needed
            float targetWidth = 500f;
            float ratio = logoSprite.rect.height / logoSprite.rect.width;
            rt.sizeDelta = new Vector2(targetWidth, targetWidth * ratio);
        }
        else
        {
            Debug.LogError("Logo sprite not found at " + path);
        }

        // Disable the original text object
        titleTextObj.SetActive(false);

        Debug.Log("Title Logo Fixed.");
    }
}
