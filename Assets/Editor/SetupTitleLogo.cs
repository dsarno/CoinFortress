using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SetupTitleLogo : MonoBehaviour
{
    [MenuItem("Tools/Setup Title Logo")]
    public static void Setup()
    {
        // 1. Import Settings
        string path = "Assets/Sprites/UI/Title_Logo.png";
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        // 2. Assign to Main Menu
        GameObject titleTextObj = GameObject.Find("Canvas/Main Menu Panel/Title Text");
        if (titleTextObj != null)
        {
            // Disable the text component
            Text textComp = titleTextObj.GetComponent<Text>();
            if (textComp != null) textComp.enabled = false;

            // Add Image component if not present (or use existing)
            Image img = titleTextObj.GetComponent<Image>();
            if (img == null) img = titleTextObj.AddComponent<Image>();

            Sprite logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (logoSprite != null)
            {
                img.sprite = logoSprite;
                img.preserveAspect = true;
                // Make it native size but scaled down if too big
                img.SetNativeSize();
                
                // Adjust scale/size to fit nicely
                RectTransform rt = titleTextObj.GetComponent<RectTransform>();
                if (rt != null)
                {
                    // Assuming the panel is around 600-800 wide
                    float targetWidth = 500f;
                    float ratio = logoSprite.rect.height / logoSprite.rect.width;
                    rt.sizeDelta = new Vector2(targetWidth, targetWidth * ratio);
                    rt.anchoredPosition = new Vector2(0, 100); // Move up a bit
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find Title Text object to replace.");
        }

        // 3. Revert Scene State (Enable Main Menu, Disable Store)
        GameObject store = GameObject.Find("Canvas/Store Root");
        if (store != null) store.SetActive(false);

        GameObject menu = GameObject.Find("Canvas/Main Menu Panel");
        if (menu != null) menu.SetActive(true);

        Debug.Log("Title Logo Setup and Scene Revert Complete.");
    }
}
