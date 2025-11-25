using UnityEngine;
using UnityEditor;

public class SetupLayers : MonoBehaviour
{
    public static void Execute()
    {
        // Create layers
        CreateLayer("Coins");
        CreateLayer("Projectile");
        
        // Setup collision matrix
        int defaultLayer = LayerMask.NameToLayer("Default");
        int coinsLayer = LayerMask.NameToLayer("Coins");
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        
        if (coinsLayer != -1 && projectileLayer != -1)
        {
            // Coins ignore Projectiles
            Physics2D.IgnoreLayerCollision(coinsLayer, projectileLayer, true);
            
            // Coins collide with Default (Ground/Blocks)
            Physics2D.IgnoreLayerCollision(coinsLayer, defaultLayer, false);
            
            // Projectiles collide with Default
            Physics2D.IgnoreLayerCollision(projectileLayer, defaultLayer, false);
            
            Debug.Log("Collision matrix updated: Coins ignore Projectiles.");
        }
        else
        {
            Debug.LogError("Layers not found!");
        }
    }

    private static void CreateLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        
        bool found = false;
        for (int i = 0; i < layers.arraySize; i++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
            if (layerSP.stringValue == layerName)
            {
                found = true;
                break;
            }
        }
        
        if (!found)
        {
            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(layerSP.stringValue))
                {
                    layerSP.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    Debug.Log("Created layer: " + layerName);
                    return;
                }
            }
        }
    }
}
