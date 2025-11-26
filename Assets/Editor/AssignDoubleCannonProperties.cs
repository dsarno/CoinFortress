using UnityEngine;
using UnityEditor;

public class AssignDoubleCannonProperties
{
    public static void Execute()
    {
        string prefabPath = "Assets/Prefabs/DoubleBarrelCannon.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("Could not find DoubleBarrelCannon prefab");
            return;
        }

        // Find the DoubleCannon child which has the controller
        Transform doubleCannon = prefab.transform.Find("CannonBase/DoubleCannon");
        if (doubleCannon == null)
        {
            Debug.LogError("Could not find DoubleCannon child in prefab");
            return;
        }

        PlayerCannonController controller = doubleCannon.GetComponent<PlayerCannonController>();
        if (controller == null)
        {
            controller = doubleCannon.gameObject.AddComponent<PlayerCannonController>();
        }

        // Assign fire points (children of DoubleCannon)
        controller.firePoint = doubleCannon.Find("FirePoint1");
        controller.firePoint2 = doubleCannon.Find("FirePoint1 (1)");
        
        // Assign effect
        controller.fireEffectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Effects/Explosion.prefab");

        // Assign projectile prefabs
        controller.projectilePrefabs = new GameObject[4];
        controller.projectilePrefabs[0] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WeakProjectile.prefab");
        controller.projectilePrefabs[1] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Projectile.prefab");
        controller.projectilePrefabs[2] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PowerUpProjectile.prefab");
        controller.projectilePrefabs[3] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MegaProjectile.prefab");

        // Save changes
        EditorUtility.SetDirty(prefab);
        PrefabUtility.SavePrefabAsset(prefab);
        
        Debug.Log("Assigned properties to DoubleBarrelCannon prefab");
    }
}
