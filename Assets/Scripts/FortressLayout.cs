using UnityEngine;

[CreateAssetMenu(fileName = "NewFortressLayout", menuName = "Fortress/Layout")]
public class FortressLayout : ScriptableObject
{
    [Header("Grid Dimensions")]
    public int width = 10;
    public int height = 8;
    
    [Header("Grid Data")]
    [SerializeField]
    private BlockType[] cells;
    
    public BlockType GetCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return BlockType.Empty;
        
        int index = y * width + x;
        if (cells == null || index >= cells.Length)
            return BlockType.Empty;
        
        return cells[index];
    }
    
    public void SetCell(int x, int y, BlockType blockType)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;
        
        EnsureCellsSize();
        int index = y * width + x;
        cells[index] = blockType;
    }
    
    public void Clear()
    {
        EnsureCellsSize();
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = BlockType.Empty;
        }
    }
    
    private void EnsureCellsSize()
    {
        int requiredSize = width * height;
        
        if (cells == null || cells.Length != requiredSize)
        {
            BlockType[] newCells = new BlockType[requiredSize];
            
            // Copy existing data if possible
            if (cells != null)
            {
                int copyCount = Mathf.Min(cells.Length, newCells.Length);
                System.Array.Copy(cells, newCells, copyCount);
            }
            
            cells = newCells;
        }
    }
    
    private void OnValidate()
    {
        // Clamp dimensions
        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);
        
        // Ensure cells array matches dimensions
        EnsureCellsSize();
    }
}