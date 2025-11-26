using UnityEngine;

public class PlayParticleSystems : MonoBehaviour
{
    [Header("Debug")]
    [Tooltip("Click this checkbox to test the effect")]
    public bool testTrigger = false;

    private void Awake()
    {
        PlayAll();
    }

    private void Update()
    {
        if (testTrigger)
        {
            testTrigger = false;
            PlayAll();
        }
    }

    [ContextMenu("Test Play")]
    public void PlayAll()
    {
        // Find all particle systems in children
        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
        float maxDuration = 0f;
        
        foreach (var ps in systems)
        {
            // Stop first to reset if it was already playing
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            var main = ps.main;
            // Ensure loop is off for the test if desired, or just play
            // main.loop = false; 
            
            ps.Play();
            
            if (main.duration > maxDuration)
            {
                maxDuration = main.duration;
            }
        }
        Debug.Log($"Played {systems.Length} particle systems on {name}");
        
        // Auto-destroy after the longest particle system finishes
        Destroy(gameObject, maxDuration + 0.5f);
    }
}
