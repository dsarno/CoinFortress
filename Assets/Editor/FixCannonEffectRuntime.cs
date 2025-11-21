using UnityEngine;
using UnityEditor;

public class FixCannonEffectRuntime
{
    [MenuItem("Tools/Fix Cannon Effect Runtime")]
    public static void Fix()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Effects/CannonSmokeEffect.prefab");
        if (prefab != null)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            ParticleSystem ps = instance.GetComponent<ParticleSystem>();
            
            if (ps != null)
            {
                var main = ps.main;
                // CRITICAL FIX: Set Scaling Mode to Local
                // When set to Hierarchy, if the parent (Cannon) has a scale (e.g. 1,1,1), it should be fine.
                // But if the instantiated prefab is parented to something with a weird scale, it can break.
                // However, the script instantiates it at a position/rotation but DOES NOT set a parent.
                // So it spawns at root.
                // If Scaling Mode is Hierarchy, it uses its own transform scale.
                // If Scaling Mode is Local, it ignores hierarchy scale for particles? No, Local means particle size is in local space.
                // Shape means shape is scaled.
                
                // Let's try setting it to Local to be safe and consistent.
                main.scalingMode = ParticleSystemScalingMode.Local;
                
                // Also ensure the Transform scale of the prefab itself is 1,1,1
                instance.transform.localScale = Vector3.one;
                
                Debug.Log("Fixed: Set Scaling Mode to Local and Transform Scale to 1,1,1");
            }

            PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
            Object.DestroyImmediate(instance);
        }
        
        AssetDatabase.SaveAssets();
    }
}
