using UnityEngine;
using UnityEditor;

public class UpdateBlockCoins : MonoBehaviour
{
    public static void UpdateCoins()
    {
        UpdatePrefab("Assets/Prefabs/GoldBlock.prefab", 5);
        UpdatePrefab("Assets/Prefabs/IronBlock.prefab", 3);
        UpdatePrefab("Assets/Prefabs/TreasureChestBlock.prefab", 20);
    }

    private static void UpdatePrefab(string path, int count)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab != null)
        {
            FortressBlock block = prefab.GetComponent<FortressBlock>();
            if (block != null)
            {
                block.coinDropCount = count;
                EditorUtility.SetDirty(prefab);
                Debug.Log($"Updated {prefab.name} coin count to {count}");
            }
            else
            {
                Debug.LogError($"FortressBlock component not found on {prefab.name}");
            }
        }
        else
        {
            Debug.LogError($"Prefab not found at {path}");
        }
    }
}
