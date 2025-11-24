using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
    [Header("Physics Settings")]
    public float minUpForce = 3f;
    public float maxUpForce = 6f;
    public float minSideForce = -2f;
    public float maxSideForce = -5f; // Toss to the left
    public float torqueAmount = 10f;
    
    [Header("Collection Settings")]
    public float collectionSpeed = 15f;
    public float magnetDistance = 2f;
    
    [Header("Visuals")]
    public float rotationSpeed = 360f;
    public Sprite frontSprite;
    public Sprite backSprite;
    public float flipInterval = 0.1f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;
    private Transform targetUI;
    private Camera mainCamera;
    
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
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(transform.position, mousePos);
        
        if (distance < 0.5f) // Hover radius
        {
            Collect();
        }
    }
    
    public void Collect()
    {
        if (isCollected) return;
        
        isCollected = true;
        rb.simulated = false; // Disable physics
        
        // Find UI target (Coins Text)
        GameObject coinsUI = GameObject.Find("Coins Text");
        if (coinsUI != null)
        {
            targetUI = coinsUI.transform;
        }
        
        // Play collection sound
        if (SoundManager.Instance != null)
        {
            // SoundManager.Instance.PlayCoinCollect(); // Assuming this exists or use a generic sound
        }
    }
    
    private void MoveToUI()
    {
        if (targetUI == null)
        {
            Destroy(gameObject);
            return;
        }
        
        Vector3 targetPos = mainCamera.ScreenToWorldPoint(targetUI.position);
        targetPos.z = 0;
        
        transform.position = Vector3.MoveTowards(transform.position, targetPos, collectionSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPos) < 0.5f)
        {
            // Add coin to player stats
            PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddCoins(1);
            }
            
            Destroy(gameObject);
        }
    }
}
