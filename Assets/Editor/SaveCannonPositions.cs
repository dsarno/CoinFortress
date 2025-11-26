using UnityEngine;
using UnityEditor;

public class SaveCannonPositions : MonoBehaviour
{
    public static void Execute()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("Could not find Player object");
            return;
        }

        CannonManager manager = player.GetComponent<CannonManager>();
        if (manager == null)
        {
            Debug.LogError("Could not find CannonManager on Player");
            return;
        }

        // Find the current cannon instance
        Transform currentCannon = player.transform.Find("SingleBarrelCannon");
        if (currentCannon != null)
        {
            manager.singleBarrelPosition = currentCannon.localPosition;
            Debug.Log($"Saved Single Barrel Position: {manager.singleBarrelPosition}");
        }

        Transform doubleCannon = player.transform.Find("DoubleBarrelCannon");
        if (doubleCannon != null)
        {
            manager.doubleBarrelPosition = doubleCannon.localPosition;
            Debug.Log($"Saved Double Barrel Position: {manager.doubleBarrelPosition}");
        }
        else
        {
            // If double barrel isn't in scene, maybe we can find the clone?
            Transform doubleCannonClone = player.transform.Find("DoubleBarrelCannon(Clone)");
            if (doubleCannonClone != null)
            {
                manager.doubleBarrelPosition = doubleCannonClone.localPosition;
                Debug.Log($"Saved Double Barrel Position (from Clone): {manager.doubleBarrelPosition}");
            }
        }

        EditorUtility.SetDirty(manager);
    }
}
