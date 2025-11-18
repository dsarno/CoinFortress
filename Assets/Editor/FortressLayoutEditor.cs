using UnityEngine;
using UnityEditor;

public class FortressLayoutEditor : EditorWindow
{
    private FortressLayout layout;
    private BlockType selectedBlockType = BlockType.Stone;
    private Vector2 scrollPos;
    private const float cellSize = 40f;
    private bool isDragging = false;
    
    [MenuItem("Tools/Fortress Layout Editor")]
    public static void ShowWindow()
    {
        GetWindow<FortressLayoutEditor>("Fortress Layout Editor");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        
        // Layout selection
        EditorGUILayout.LabelField("Fortress Layout", EditorStyles.boldLabel);
        layout = (FortressLayout)EditorGUILayout.ObjectField("Layout", layout, typeof(FortressLayout), false);
        
        if (layout == null)
        {
            EditorGUILayout.HelpBox("Select or create a Fortress Layout asset to begin.", MessageType.Info);
            
            if (GUILayout.Button("Create New Layout"))
            {
                CreateNewLayout();
            }
            return;
        }
        
        EditorGUILayout.Space(10);
        
        // Dimensions
        EditorGUI.BeginChangeCheck();
        int newWidth = EditorGUILayout.IntField("Width", layout.width);
        int newHeight = EditorGUILayout.IntField("Height", layout.height);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(layout, "Change Layout Dimensions");
            layout.width = Mathf.Clamp(newWidth, 1, 30);
            layout.height = Mathf.Clamp(newHeight, 1, 30);
            EditorUtility.SetDirty(layout);
        }
        
        EditorGUILayout.Space(10);
        
        // Palette
        EditorGUILayout.LabelField("Block Palette", EditorStyles.boldLabel);
        DrawBlockPalette();
        
        EditorGUILayout.Space(10);
        
        // Clear button
        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear Layout", "Are you sure you want to clear the entire layout?", "Yes", "No"))
            {
                Undo.RecordObject(layout, "Clear Layout");
                layout.Clear();
                EditorUtility.SetDirty(layout);
            }
        }
        
        EditorGUILayout.Space(10);
        
        // Grid
        EditorGUILayout.LabelField("Grid Editor (Click/Drag to Paint)", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DrawGrid();
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawBlockPalette()
    {
        EditorGUILayout.BeginHorizontal();
        
        BlockType[] blockTypes = { BlockType.Empty, BlockType.Stone, BlockType.Iron, BlockType.Gold, BlockType.Core, BlockType.Turret };
        
        foreach (BlockType blockType in blockTypes)
        {
            Color blockColor = GetBlockColor(blockType);
            
            // Highlight selected
            GUI.backgroundColor = (selectedBlockType == blockType) ? Color.white : Color.gray;
            
            if (GUILayout.Button(blockType.ToString(), GUILayout.Width(80), GUILayout.Height(40)))
            {
                selectedBlockType = blockType;
            }
            
            GUI.backgroundColor = Color.white;
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Show selected color
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selected:", GUILayout.Width(60));
        Color selectedColor = GetBlockColor(selectedBlockType);
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Width(100), GUILayout.Height(20)), selectedColor);
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawGrid()
    {
        Event e = Event.current;
        
        Rect gridRect = GUILayoutUtility.GetRect(layout.width * cellSize, layout.height * cellSize);
        
        for (int y = 0; y < layout.height; y++)
        {
            for (int x = 0; x < layout.width; x++)
            {
                Rect cellRect = new Rect(
                    gridRect.x + x * cellSize,
                    gridRect.y + (layout.height - 1 - y) * cellSize, // Flip Y to draw bottom-up
                    cellSize,
                    cellSize
                );
                
                BlockType cellType = layout.GetCell(x, y);
                Color cellColor = GetBlockColor(cellType);
                
                // Draw cell
                EditorGUI.DrawRect(cellRect, cellColor);
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellRect.width, 1), Color.black);
                EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1, cellRect.height), Color.black);
                
                // Handle mouse input
                if (e.type == EventType.MouseDown && cellRect.Contains(e.mousePosition))
                {
                    isDragging = true;
                    PaintCell(x, y);
                    e.Use();
                }
                else if (e.type == EventType.MouseDrag && isDragging && cellRect.Contains(e.mousePosition))
                {
                    PaintCell(x, y);
                    e.Use();
                }
            }
        }
        
        if (e.type == EventType.MouseUp)
        {
            isDragging = false;
        }
    }
    
    private void PaintCell(int x, int y)
    {
        if (layout.GetCell(x, y) != selectedBlockType)
        {
            Undo.RecordObject(layout, "Paint Cell");
            layout.SetCell(x, y, selectedBlockType);
            EditorUtility.SetDirty(layout);
            Repaint();
        }
    }
    
    private Color GetBlockColor(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.Empty: return new Color(0.2f, 0.2f, 0.2f);
            case BlockType.Stone: return new Color(0.6f, 0.4f, 0.2f);
            case BlockType.Iron: return new Color(0.5f, 0.5f, 0.6f);
            case BlockType.Gold: return new Color(0.9f, 0.8f, 0.2f);
            case BlockType.Core: return new Color(1f, 0.3f, 0f);
            case BlockType.Turret: return new Color(0.3f, 0.3f, 0.5f);
            default: return Color.gray;
        }
    }
    
    private void CreateNewLayout()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Fortress Layout",
            "NewFortressLayout",
            "asset",
            "Create a new fortress layout asset"
        );
        
        if (!string.IsNullOrEmpty(path))
        {
            FortressLayout newLayout = CreateInstance<FortressLayout>();
            AssetDatabase.CreateAsset(newLayout, path);
            AssetDatabase.SaveAssets();
            layout = newLayout;
        }
    }
}