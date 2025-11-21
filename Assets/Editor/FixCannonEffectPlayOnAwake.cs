using UnityEngine;
using UnityEditor;

public class FixCannonEffectPlayOnAwake
{
    [MenuItem("Tools/Fix Cannon Effect PlayOnAwake")]
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
                // Ensure Play On Awake is TRUE
                main.playOnAwake = true;
                
                // Ensure Loop is FALSE (we want a single burst)
                main.loop = false;
                
                Debug.Log("Fixed: Set PlayOnAwake to TRUE");
            }

            PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
            Object.DestroyImmediate(instance);
        }
        
        AssetDatabase.SaveAssets();
    }
}
