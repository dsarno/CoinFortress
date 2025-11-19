using UnityEngine;
using UnityEditor;
using System.IO;

public class SetupCrackSprites : MonoBehaviour
{
    public static void Execute()
    {
        // Load all crack sprites
        string[] crackPaths = new string[]
        {
            "Assets/Sprites/Cracks/Crack_01.png",
            "Assets/Sprites/Cracks/Crack_02.png",
            "Assets/Sprites/Cracks/Crack_03.png",
            "Assets/Sprites/Cracks/Crack_04.png",
            "Assets/Sprites/Cracks/Crack_05.png"
        };
        
        Sprite[] crackSprites = new Sprite[crackPaths.Length];
        
        // Configure each sprite's import settings
        for (int i = 0; i < crackPaths.Length; i++)
        {
            if (File.Exists(crackPaths[i]))
            {
                // Set import settings
                TextureImporter importer = AssetImporter.GetAtPath(crackPaths[i]) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.alphaIsTransparency = true;
                    importer.spritePixelsPerUnit = 100;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.SaveAndReimport();
                }
                
                // Load sprite
                crackSprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(crackPaths[i]);
                Debug.Log($"Loaded crack sprite: {crackPaths[i]}");
            }
            else
            {
                Debug.LogWarning($"Crack sprite not found: {crackPaths[i]}");
            }
        }
        
        // CrackEffect components will get the database from CrackEffectManager at runtime
        Debug.Log("Crack sprites loaded. CrackEffect components will use CrackEffectManager at runtime.");
        
        // Save reference sprites to a ScriptableObject for runtime use
        CrackSpritesDatabase database = ScriptableObject.CreateInstance<CrackSpritesDatabase>();
        database.crackSprites = crackSprites;
        
        string databasePath = "Assets/Data/CrackSpritesDatabase.asset";
        AssetDatabase.CreateAsset(database, databasePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Created CrackSpritesDatabase at {databasePath}");
    }
}
