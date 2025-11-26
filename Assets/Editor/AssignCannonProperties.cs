using UnityEngine;
using UnityEditor;

public class AssignCannonProperties
{
    public static void Execute()
    {
        GameObject cannon = GameObject.Find("DoubleCannon");
        if (cannon == null)
        {
            Debug.LogError("Could not find DoubleCannon object");
            return;
        }

        PlayerCannonController controller = cannon.GetComponent<PlayerCannonController>();
        if (controller == null)
        {
            Debug.LogError("Could not find PlayerCannonController component");
            return;
        }

        // Assign projectile prefabs
        controller.projectilePrefabs = new GameObject[4];
        controller.projectilePrefabs[0] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WeakProjectile.prefab");
        controller.projectilePrefabs[1] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Projectile.prefab");
        controller.projectilePrefabs[2] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PowerUpProjectile.prefab");
        controller.projectilePrefabs[3] = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MegaProjectile.prefab");

        Debug.Log("Assigned projectile prefabs to PlayerCannonController");
    }
}
