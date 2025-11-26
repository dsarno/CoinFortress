using UnityEngine;

public class BlockShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.1f;
    public float shakeSpeed = 20f;
    public bool usePerlinNoise = true;
    
    private Vector3 originalPosition;
    private float currentShakeDuration;
    private float currentShakeIntensity;
    private bool isShaking = false;
    private float seed;
    
    private void Awake()
    {
        // Store local position since blocks are children of FortressRoot
        originalPosition = transform.localPosition;
        seed = Random.Range(0f, 100f);
    }
    
    public void Shake(float intensityMultiplier = 1f)
    {
        currentShakeDuration = shakeDuration;
        currentShakeIntensity = shakeIntensity * intensityMultiplier;
        isShaking = true;
        
        // Reset position to ensure we don't drift if interrupted
        transform.localPosition = originalPosition;
    }
    
    private void Update()
    {
        if (isShaking)
        {
            if (currentShakeDuration > 0)
            {
                Vector3 offset;
                
                if (usePerlinNoise)
                {
                    // Perlin noise for smoother shake
                    float noiseX = Mathf.PerlinNoise(seed + Time.time * shakeSpeed, seed) * 2f - 1f;
                    float noiseY = Mathf.PerlinNoise(seed, seed + Time.time * shakeSpeed) * 2f - 1f;
                    offset = new Vector3(noiseX, noiseY, 0f) * currentShakeIntensity;
                }
                else
                {
                    // Random inside unit circle for chaotic shake
                    offset = (Vector3)Random.insideUnitCircle * currentShakeIntensity;
                }
                
                transform.localPosition = originalPosition + offset;
                
                currentShakeDuration -= Time.deltaTime;
                
                // Optional: Decay intensity over time
                // currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0f, Time.deltaTime * 5f);
            }
            else
            {
                isShaking = false;
                transform.localPosition = originalPosition;
            }
        }
    }
}
