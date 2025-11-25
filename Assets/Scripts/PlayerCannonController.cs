using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerCannonController : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public GameObject powerUpProjectilePrefab;
    public GameObject fireEffectPrefab;
    public Transform firePoint;
    
    [Header("Settings")]
    public float projectileSpeed = 15f;
    public float fireForce = 10f; // Initial upward force
    
    [Header("Effect Settings")]
    public float weakShotSizeMin = 3f;
    public float weakShotSizeMax = 5f;
    
    private PlayerStats playerStats;
    private float lastFireTime;
    private float lastWeakFireTime;
    private Camera mainCamera;
    private Mouse mouse;
    private LevelManager levelManager;
    
    private void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();
        mainCamera = Camera.main;
        mouse = Mouse.current;
        levelManager = FindFirstObjectByType<LevelManager>();
        
        // Create fire point if not assigned
        if (firePoint == null)
        {
            Transform existingFirePoint = transform.Find("FirePoint");
            if (existingFirePoint != null)
            {
                firePoint = existingFirePoint;
            }
            else
            {
                GameObject firePointObj = new GameObject("FirePoint");
                firePointObj.transform.SetParent(transform);
                firePointObj.transform.localPosition = new Vector3(1f, 0f, 0f);
                firePoint = firePointObj.transform;
            }
        }
    }
    
    private void Update()
    {
        // Only allow cannon control during gameplay
        if (levelManager != null && !levelManager.levelInProgress)
        {
            return;
        }
        
        // Don't aim or fire if mouse is over UI
        if (IsPointerOverUI())
        {
            return;
        }
        
        AimAtMouse();
        HandleFiring();
    }
    
    private void AimAtMouse()
    {
        if (mouse == null || mainCamera == null) return;
        
        // Get mouse world position
        Vector2 mouseScreenPos = mouse.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
        mouseWorldPos.z = 0f;
        
        // Calculate direction and angle
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Rotate cannon to face mouse
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    private void HandleFiring()
    {
        if (mouse == null) return;
        
        // Don't fire if mouse is over UI
        if (IsPointerOverUI())
        {
            return;
        }
        
        // Fire on left click
        if (mouse.leftButton.isPressed)
        {
            if (playerStats.HasAmmo() && CanFireNormal())
            {
                FireNormalShot();
            }
            else if (!playerStats.HasAmmo() && CanFireWeak())
            {
                FireWeakShot();
            }
        }
    }
    
    private bool IsPointerOverUI()
    {
        // Check if mouse is over any UI element
        if (EventSystem.current == null) return false;
        
        return EventSystem.current.IsPointerOverGameObject();
    }
    
    private bool CanFireNormal()
    {
        return Time.time >= lastFireTime + playerStats.fireCooldown;
    }
    
    private bool CanFireWeak()
    {
        return Time.time >= lastWeakFireTime + playerStats.weakShotCooldown;
    }
    
    private void FireNormalShot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing projectile prefab or fire point!");
            return;
        }
        
        lastFireTime = Time.time;
        playerStats.ConsumeAmmo();
        
        // Play cannon firing sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCannonFireSound();
        }
        
        // Determine prefab and speed based on upgrades
        GameObject prefabToUse = projectilePrefab;
        float speedToUse = projectileSpeed;
        
        if (playerStats.damageLevel > 1 && powerUpProjectilePrefab != null)
        {
            prefabToUse = powerUpProjectilePrefab;
            speedToUse = projectileSpeed * 1.5f;
        }
        
        // Spawn projectile
        GameObject projectile = Instantiate(prefabToUse, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // Spawn fire effect
        if (fireEffectPrefab != null)
        {
            GameObject effect = Instantiate(fireEffectPrefab, firePoint.position, firePoint.rotation);
            // Normal shot size (default prefab size)
        }
        
        if (projectileScript != null)
        {
            // Fire in the direction the cannon is pointing
            Vector2 direction = firePoint.right;
            int damage = GetProjectileDamage();
            projectileScript.Initialize(direction, speedToUse, damage, playerStats.ammoTier);
        }
        
    }
    
    private void FireWeakShot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing projectile prefab or fire point!");
            return;
        }
        
        lastWeakFireTime = Time.time;
        
        // Play cannon firing sound (quieter for weak shot)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCannonFireSound(0.6f);
        }
        
        // Spawn weak projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // Spawn fire effect
        if (fireEffectPrefab != null)
        {
            GameObject effect = Instantiate(fireEffectPrefab, firePoint.position, firePoint.rotation);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                // Reduce size for weak shot
                main.startSize = new ParticleSystem.MinMaxCurve(weakShotSizeMin, weakShotSizeMax);
            }
        }
        
        if (projectileScript != null)
        {
            Vector2 direction = firePoint.right;
            projectileScript.Initialize(direction, projectileSpeed * 0.7f, playerStats.weakShotDamage, 0);
            
            // Make it visually distinct (darker color)
            SpriteRenderer sr = projectile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
        
    }
    
    private int GetProjectileDamage()
    {
        switch (playerStats.ammoTier)
        {
            default:
            case 0: return playerStats.damage; // Standard
            case 1: return Mathf.RoundToInt(playerStats.damage * 1.5f); // Heavy (+50%)
            case 2: return Mathf.RoundToInt(playerStats.damage * 1.5f); // Explosive (damage + splash)
        }
    }
}