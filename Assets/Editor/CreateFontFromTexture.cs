using UnityEngine;
using UnityEditor;
using TMPro;

public class CreateFontFromTexture : EditorWindow
{
    private Texture2D fontTexture;
    
    [MenuItem("Tools/Create TextMesh Pro Font from Texture")]
    public static void ShowWindow()
    {
        GetWindow<CreateFontFromTexture>("Create TMP Font");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Create TextMesh Pro Bitmap Font from Texture", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox("For bitmap fonts from textures, TextMesh Pro requires using Unity's Font Asset Creator with a font file.\n\nThis helper will guide you through the process.", MessageType.Info);
        GUILayout.Space(10);
        
        fontTexture = (Texture2D)EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture2D), false);
        
        GUILayout.Space(10);
        
        EditorGUI.BeginDisabledGroup(fontTexture == null);
        if (GUILayout.Button("Open Font Asset Creator", GUILayout.Height(30)))
        {
            OpenFontAssetCreator();
        }
        EditorGUI.EndDisabledGroup();
        
        GUILayout.Space(20);
        GUILayout.Label("Instructions:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "1. Click the button above to open Font Asset Creator\n" +
            "2. In Font Asset Creator:\n" +
            "   - Select ANY font file (TTF/OTF) - this is just a placeholder\n" +
            "   - Set Rendering Mode to 'Bitmap'\n" +
            "   - Set Character Set to 'ASCII' or 'Custom'\n" +
            "   - Click 'Generate Font Atlas'\n" +
            "3. After creation:\n" +
            "   - Select the created Font Asset\n" +
            "   - In the Inspector, find 'Atlas Texture'\n" +
            "   - Replace it with your stone font texture\n" +
            "   - Manually adjust character glyph rectangles if needed\n\n" +
            "Alternative: Use TextMeshPro > Sprite Asset if your texture is sprite-based.",
            MessageType.None);
    }
    
    private void OpenFontAssetCreator()
    {
        if (fontTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a font texture first!", "OK");
            return;
        }
        
        // Find LiberationSans font (has font data enabled)
        Font templateFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");
        if (templateFont == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "Could not find LiberationSans.ttf font file.\n\n" +
                "Please manually select a font file in the Font Asset Creator.\n" +
                "Make sure it has 'Include Font Data' enabled in import settings.", 
                "OK");
        }
        else
        {
            // Select the template font
            Selection.activeObject = templateFont;
        }
        
        // Open Font Asset Creator
        EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Font Asset Creator");
        
        EditorUtility.DisplayDialog("Font Asset Creator Opened", 
            $"Font Asset Creator window opened.\n\n" +
            $"Template font selected: LiberationSans\n" +
            $"Your texture '{fontTexture.name}' is ready.\n\n" +
            $"Steps:\n" +
            $"1. Set Rendering Mode to 'Bitmap'\n" +
            $"2. Set Character Set (ASCII or Custom)\n" +
            $"3. Click 'Generate Font Atlas'\n" +
            $"4. After creation, replace Atlas Texture with your stone texture\n" +
            $"5. Adjust character glyph rectangles if needed", 
            "OK");
    }
}
