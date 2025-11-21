using UnityEngine;
using UnityEditor;

public class FixCannonEffectSpecifics
{
    [MenuItem("Tools/Fix Cannon Effect Specifics")]
    public static void Fix()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Effects/CannonSmokeEffect.prefab");
        if (prefab != null)
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            ParticleSystem ps = instance.GetComponent<ParticleSystem>();
            ParticleSystemRenderer psr = instance.GetComponent<ParticleSystemRenderer>();
            
            // 1. Fix Size Issue
            if (psr != null)
            {
                // The MinParticleSize was set to 0.45, which forces every particle to be at least 45% of the screen height!
                // This is why they look huge in the Game View.
                psr.minParticleSize = 0.0f; 
                psr.maxParticleSize = 1.0f;
                Debug.Log("Fixed: Reset MinParticleSize to 0");
            }

            // 2. Fix Transparency Issue
            if (ps != null)
            {
                var col = ps.colorOverLifetime;
                col.enabled = true;
                
                // Ensure we are using a SINGLE gradient, not "Random Between Two Gradients"
                // The previous setting was "Random Between Two Gradients" (MinMaxState = 1)
                // and the 'min' gradient might have been fully transparent.
                Gradient grad = new Gradient();
                grad.SetKeys(
                    new GradientColorKey[] { 
                        new GradientColorKey(Color.white, 0.0f), 
                        new GradientColorKey(Color.white, 1.0f) 
                    },
                    new GradientAlphaKey[] { 
                        new GradientAlphaKey(1.0f, 0.0f),   // Start Fully Opaque
                        new GradientAlphaKey(1.0f, 0.6f),   // Stay Opaque for 60% of life
                        new GradientAlphaKey(0.0f, 1.0f)    // Fade to Transparent at end
                    }
                );
                
                col.color = new ParticleSystem.MinMaxGradient(grad); // Use single gradient constructor
                
                // Also ensure Start Color is white/opaque
                var main = ps.main;
                main.startColor = Color.white;
                
                Debug.Log("Fixed: Set ColorOverLifetime to single gradient (Opaque -> Transparent)");
            }

            PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
            Object.DestroyImmediate(instance);
        }
        
        AssetDatabase.SaveAssets();
    }
}
