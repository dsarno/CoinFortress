using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerCannonController : MonoBehaviour
{
    [Header("References")]
    // Index 0: Weak, 1: Normal, 2: PowerUp, 3: Mega
    public GameObject[] projectilePrefabs;
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
        if (projectilePrefabs == null || projectilePrefabs.Length == 0 || firePoint == null)
        {
            Debug.LogWarning("Missing projectile prefabs or fire point!");
            return;
        }
        
        lastFireTime = Time.time;
        playerStats.ConsumeAmmo();
        
        // Play cannon firing sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCannonFireSound();
        }
        
        // Determine prefab based on damage level
        // Level 1 = Normal (Index 1), Level 2 = PowerUp (Index 2), Level 3 = Mega (Index 3)
        int prefabIndex = Mathf.Clamp(playerStats.damageLevel, 1, projectilePrefabs.Length - 1);
        GameObject prefabToUse = projectilePrefabs[prefabIndex];
        
        // Spawn projectile
        // Use prefab's rotation if it's not the standard ball (to maintain trails/orientation)
        Quaternion spawnRotation = firePoint.rotation;
        if (prefabIndex >= 2) // PowerUp and Mega have specific orientations
        {
            spawnRotation = prefabToUse.transform.rotation;
        }
        
        GameObject projectile = Instantiate(prefabToUse, firePoint.position, spawnRotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // Spawn fire effect
        if (fireEffectPrefab != null)
        {
            Instantiate(fireEffectPrefab, firePoint.position, firePoint.rotation);
        }
        
        if (projectileScript != null)
        {
            Vector2 direction = firePoint.right;
            // Pass -1 to use prefab's damage and speed
            projectileScript.Initialize(direction, -1f, -1, playerStats.ammoTier);
        }
    }
    
    private void FireWeakShot()
    {
        if (projectilePrefabs == null || projectilePrefabs.Length == 0 || firePoint == null)
        {
            Debug.LogWarning("Missing projectile prefabs or fire point!");
            return;
        }
        
        lastWeakFireTime = Time.time;
        
        // Play cannon firing sound (quieter for weak shot)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayCannonFireSound(0.6f);
        }
        
        // Spawn weak projectile (Index 0)
        GameObject projectile = Instantiate(projectilePrefabs[0], firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        // Spawn fire effect
        if (fireEffectPrefab != null)
        {
            GameObject effect = Instantiate(fireEffectPrefab, firePoint.position, firePoint.rotation);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startSize = new ParticleSystem.MinMaxCurve(weakShotSizeMin, weakShotSizeMax);
            }
        }
        
        if (projectileScript != null)
        {
            Vector2 direction = firePoint.right;
            // Weak shot uses specific weak damage (usually 1)
            // Pass -1 for speed to use the prefab's configured speed
            // Pass -1 for damage to use the prefab's configured damage
            projectileScript.Initialize(direction, -1f, -1, 0);
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