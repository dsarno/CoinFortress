using UnityEngine;
using UnityEditor;
using System.IO;

public class SetupUISprites : MonoBehaviour
{
    [MenuItem("Tools/Setup UI Sprites")]
    public static void SetupSprites()
    {
        SetupSprite("Assets/Sprites/UI/UI_Panel_Stone.png", new Vector4(128, 128, 128, 128));
        SetupSprite("Assets/Sprites/UI/UI_Button_Stone.png", new Vector4(64, 64, 64, 64)); // Smaller border for button
        SetupSprite("Assets/Sprites/UI/Font_Atlas_Stone.png", Vector4.zero);
        
        AssetDatabase.Refresh();
        Debug.Log("UI Sprites Setup Complete.");
    }

    private static void SetupSprite(string path, Vector4 border)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            return;
        }

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spriteBorder = border;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            Debug.Log($"Configured sprite: {path}");
        }
        else
        {
            Debug.LogError($"Could not get TextureImporter for: {path}");
        }
    }
}
