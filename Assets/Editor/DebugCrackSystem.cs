using UnityEngine;
using UnityEditor;

public class DebugCrackSystem : MonoBehaviour
{
    [MenuItem("Tools/Debug Crack System")]
    public static void Execute()
    {
        // Check database
        CrackSpritesDatabase database = AssetDatabase.LoadAssetAtPath<CrackSpritesDatabase>("Assets/Data/CrackSpritesDatabase.asset");
        if (database == null)
        {
            Debug.LogError("❌ Database not found!");
            return;
        }
        
        Debug.Log($"Database found: {database.crackSprites?.Length ?? 0} sprites");
        if (database.crackSprites != null)
        {
            for (int i = 0; i < database.crackSprites.Length; i++)
            {
                Debug.Log($"  Sprite {i}: {database.crackSprites[i]?.name ?? "NULL"}");
            }
        }
        
        // Check manager
        CrackEffectManager manager = Object.FindFirstObjectByType<CrackEffectManager>();
        if (manager != null)
        {
            Debug.Log($"Manager found: {manager.gameObject.name}");
            Debug.Log($"  Database ref: {manager.spritesDatabase != null}");
            Debug.Log($"  Material ref: {manager.crackMaterial != null}");
        }
        
        // Check a specific crack overlay
        GameObject crackOverlay = GameObject.Find("FortressRoot/Stone_3_3/CrackOverlay");
        if (crackOverlay != null)
        {
            SpriteRenderer sr = crackOverlay.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"CrackOverlay SpriteRenderer:");
                Debug.Log($"  Sprite: {sr.sprite?.name ?? "NULL"}");
                Debug.Log($"  Material: {sr.material?.name ?? "NULL"}");
                Debug.Log($"  Shader: {sr.material?.shader?.name ?? "NULL"}");
            }
        }
    }
}
