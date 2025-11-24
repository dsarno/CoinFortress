using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CoinController : MonoBehaviour
{
    [Header("Physics Settings")]
    public float minUpForce = 5f;
    public float maxUpForce = 10f;
    public float minSideForce = -5f;
    public float maxSideForce = -12f; // Toss to the left
    public float torqueAmount = 10f;
    
    [Header("Collection Settings")]
    public float collectionSpeed = 40f;
    public float magnetDistance = 2f;
    
    [Header("Visuals")]
    public float rotationSpeed = 360f;
    public Sprite frontSprite;
    public Sprite backSprite;
    public float flipInterval = 0.1f;
    
    [Header("Fountain Settings")]
    public bool isVisual = false;
    public bool givesCoins = true;
    public float autoFadeDelay = -1f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;
    private Transform targetUI;
    private Camera mainCamera;
    private float currentMoveSpeed;
    private float acceleration = 50f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }
    
    private void Start()
    {
        // Initial toss
        float upForce = Random.Range(minUpForce, maxUpForce);
        float sideForce = Random.Range(minSideForce, maxSideForce);
        rb.AddForce(new Vector2(sideForce, upForce), ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-torqueAmount, torqueAmount));
        
        StartCoroutine(AnimateFlip());

        if (autoFadeDelay > 0 && !IsInvoking(nameof(BeginFadeOut)))
        {
            Invoke(nameof(BeginFadeOut), autoFadeDelay);
        }
    }

    public void TriggerVisualEffect(float delay)
    {
        isVisual = true;
        autoFadeDelay = delay;
        
        CancelInvoke(nameof(BeginFadeOut));
        Invoke(nameof(BeginFadeOut), autoFadeDelay);
    }
    
    private void Update()
    {
        if (isCollected)
        {
            MoveToUI();
            return;
        }
        
        CheckMouseHover();
    }

    private void BeginFadeOut()
    {
        StartCoroutine(FadeOutRoutine(1.5f));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startAlpha = spriteRenderer.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = newAlpha;
                spriteRenderer.color = c;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
    
    private IEnumerator AnimateFlip()
    {
        while (!isCollected)
        {
            if (spriteRenderer != null && frontSprite != null && backSprite != null)
            {
                spriteRenderer.sprite = (spriteRenderer.sprite == frontSprite) ? backSprite : frontSprite;
            }
            yield return new WaitForSeconds(flipInterval);
        }
    }
    
    private void CheckMouseHover()
    {
        // Use new Input System
        if (Mouse.current == null) return;
        
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.nearClipPlane));
        mousePos.z = transform.position.z; // Match Z depth for accurate distance check
        
        float distance = Vector2.Distance(transform.position, mousePos);
        
        if (distance < 1.5f) // Increased hover radius for better feel
        {
            Collect();
        }
    }
    
    public void Collect()
    {
        if (isCollected) return;
        
        isCollected = true;
        rb.simulated = false; // Disable physics
        
        // Stop any fade out
        CancelInvoke(nameof(BeginFadeOut));
        StopAllCoroutines(); // Stops fade and flip
        
        // Reset alpha
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }
        
        // Find UI target (CoinTarget)
        GameObject coinsUI = GameObject.Find("CoinTarget");
        if (coinsUI != null)
        {
            targetUI = coinsUI.transform;
        }
        
        // Play collection sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCoinHover(); // Play hover sound immediately
            // We'll play the "cha-ching" (PurchaseSuccess) when it actually hits the UI target
        }
    }
    
    private void MoveToUI()
    {
        if (targetUI == null)
        {
            Destroy(gameObject);
            return;
        }
        
        // Calculate world position of UI element
        // We need to pass the distance from camera to the object plane (z=0)
        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 screenPos = targetUI.position;
        screenPos.z = distanceToCamera;
        
        Vector3 targetPos = mainCamera.ScreenToWorldPoint(screenPos);
        targetPos.z = 0; // Ensure it stays on the 2D plane
        
        // Accelerate
        currentMoveSpeed += acceleration * Time.deltaTime;
        
        transform.position = Vector3.MoveTowards(transform.position, targetPos, (collectionSpeed + currentMoveSpeed) * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPos) < 0.5f)
        {
            // Add coin to player stats
            if (givesCoins)
            {
                PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.AddCoins(1);
                    
                    // Play collection sound (cha-ching) when it hits the target
                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlayPurchaseSuccess();
                    }
                }
            }
            
            Destroy(gameObject);
        }
    }
}
