using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

public class ConfigureBitmapFontGlyphs : EditorWindow
{
    private TMP_FontAsset fontAsset;
    private Texture2D fontTexture;
    private int charactersPerRow = 6;
    private int characterWidth = 64;
    private int characterHeight = 64;
    private string characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    [MenuItem("Tools/Configure Bitmap Font Glyphs")]
    public static void ShowWindow()
    {
        GetWindow<ConfigureBitmapFontGlyphs>("Configure Font Glyphs");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Configure Bitmap Font Glyphs", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox("This tool helps configure glyph rectangles for bitmap fonts from textures.\n\nFirst generate a font asset using Font Asset Creator, then use this to configure the glyphs.", MessageType.Info);
        GUILayout.Space(10);
        
        fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("Font Asset", fontAsset, typeof(TMP_FontAsset), false);
        fontTexture = (Texture2D)EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture2D), false);
        
        GUILayout.Space(10);
        GUILayout.Label("Character Layout:", EditorStyles.boldLabel);
        charactersPerRow = EditorGUILayout.IntField("Characters Per Row", charactersPerRow);
        characterWidth = EditorGUILayout.IntField("Character Width (pixels)", characterWidth);
        characterHeight = EditorGUILayout.IntField("Character Height (pixels)", characterHeight);
        characterSet = EditorGUILayout.TextField("Character Set", characterSet);
        
        GUILayout.Space(10);
        
        EditorGUI.BeginDisabledGroup(fontAsset == null || fontTexture == null);
        if (GUILayout.Button("Configure Glyph Rectangles", GUILayout.Height(30)))
        {
            ConfigureGlyphs();
        }
        EditorGUI.EndDisabledGroup();
        
        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Note: You'll need to manually set the Atlas Texture in the Font Asset inspector after configuring glyphs.\nThe atlas texture field may be read-only - you may need to duplicate the font asset or use a workaround.", MessageType.Warning);
    }
    
    private void ConfigureGlyphs()
    {
        if (fontAsset == null || fontTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select both Font Asset and Font Texture!", "OK");
            return;
        }
        
        // Make sure we can modify the font asset
        EditorUtility.SetDirty(fontAsset);
        
        // Try to set the atlas texture using SerializedObject
        SerializedObject serializedFont = new SerializedObject(fontAsset);
        SerializedProperty atlasTextureProp = serializedFont.FindProperty("m_AtlasTexture");
        
        if (atlasTextureProp != null)
        {
            atlasTextureProp.objectReferenceValue = fontTexture;
            serializedFont.ApplyModifiedProperties();
        }
        
        // Get the character table as a serialized property
        SerializedProperty characterTableProp = serializedFont.FindProperty("m_CharacterTable");
        
        if (characterTableProp == null || !characterTableProp.isArray)
        {
            EditorUtility.DisplayDialog("Error", "Could not access character table. The font asset may not have been generated properly.", "OK");
            return;
        }
        
        // Debug: Check what characters are in the font
        string foundChars = "";
        for (int i = 0; i < Mathf.Min(characterTableProp.arraySize, 10); i++)
        {
            SerializedProperty charProp = characterTableProp.GetArrayElementAtIndex(i);
            SerializedProperty unicodeProp = charProp.FindPropertyRelative("m_Unicode");
            if (unicodeProp != null)
            {
                foundChars += (char)unicodeProp.uintValue + " ";
            }
        }
        Debug.Log($"Font asset has {characterTableProp.arraySize} characters. First few: {foundChars}");
        
        int row = 0;
        int col = 0;
        int configuredCount = 0;
        
        foreach (char c in characterSet)
        {
            uint unicode = (uint)c;
            
            // Find the character in the serialized array
            bool found = false;
            for (int i = 0; i < characterTableProp.arraySize; i++)
            {
                SerializedProperty charProp = characterTableProp.GetArrayElementAtIndex(i);
                SerializedProperty unicodeProp = charProp.FindPropertyRelative("m_Unicode");
                
                if (unicodeProp != null && unicodeProp.uintValue == unicode)
                {
                    // Calculate glyph rectangle based on grid position
                    int x = col * characterWidth;
                    int y = fontTexture.height - ((row + 1) * characterHeight); // Flip Y coordinate
                    
                    // Update glyph rectangle - try different property paths
                    SerializedProperty glyphProp = charProp.FindPropertyRelative("m_Glyph");
                    if (glyphProp != null)
                    {
                        // Try m_GlyphRect first
                        SerializedProperty glyphRectProp = glyphProp.FindPropertyRelative("m_GlyphRect");
                        if (glyphRectProp != null)
                        {
                            SerializedProperty xProp = glyphRectProp.FindPropertyRelative("m_X");
                            SerializedProperty yProp = glyphRectProp.FindPropertyRelative("m_Y");
                            SerializedProperty widthProp = glyphRectProp.FindPropertyRelative("m_Width");
                            SerializedProperty heightProp = glyphRectProp.FindPropertyRelative("m_Height");
                            
                            if (xProp != null && yProp != null && widthProp != null && heightProp != null)
                            {
                                xProp.intValue = x;
                                yProp.intValue = y;
                                widthProp.intValue = characterWidth;
                                heightProp.intValue = characterHeight;
                                configuredCount++;
                                found = true;
                                break;
                            }
                        }
                        
                        // Try alternative property names
                        SerializedProperty rectProp = glyphProp.FindPropertyRelative("glyphRect");
                        if (rectProp != null)
                        {
                            rectProp.FindPropertyRelative("x").intValue = x;
                            rectProp.FindPropertyRelative("y").intValue = y;
                            rectProp.FindPropertyRelative("width").intValue = characterWidth;
                            rectProp.FindPropertyRelative("height").intValue = characterHeight;
                            configuredCount++;
                            found = true;
                            break;
                        }
                    }
                }
            }
            
            if (!found)
            {
                Debug.LogWarning($"Character '{c}' (Unicode {unicode}) not found in font asset. Make sure the font was generated with ASCII character set.");
            }
            
            col++;
            if (col >= charactersPerRow)
            {
                col = 0;
                row++;
            }
        }
        
        // Apply all changes
        serializedFont.ApplyModifiedProperties();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        if (configuredCount == 0)
        {
            EditorUtility.DisplayDialog("No Characters Found", 
                $"The font asset doesn't contain the characters you need (A-Z, 0-9).\n\n" +
                $"Current font has {characterTableProp.arraySize} characters starting with: {foundChars}\n\n" +
                $"To fix this:\n" +
                $"1. Open Font Asset Creator (Window > TextMeshPro > Font Asset Creator)\n" +
                $"2. Select LiberationSans font\n" +
                $"3. Set Character Set to 'ASCII' (not Custom)\n" +
                $"4. Set Render Mode to 'RASTER'\n" +
                $"5. Click 'Generate Font Atlas'\n" +
                $"6. Then run this tool again", 
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Glyphs Configured", 
                $"Configured {configuredCount} glyph rectangles.\n\n" +
                $"Atlas texture set to: {fontTexture.name}\n\n" +
                $"If the texture didn't update, you may need to:\n" +
                $"1. Select the font asset and check the Atlas Texture field\n" +
                $"2. Or regenerate the font asset with your texture", 
                "OK");
        }
    }
}

