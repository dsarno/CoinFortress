using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    
    [Header("Settings")]
    public float projectileSpeed = 10f;
    
    private PlayerStats playerStats;
    private float lastFireTime;
    private Camera mainCamera;
    
    private void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();
        mainCamera = Camera.main;
        
        // Create fire point if not assigned
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(1f, 0f, 0f); // Right side of cannon
            firePoint = firePointObj.transform;
        }
    }
    
    private void Update()
    {
        AimAtMouse();
        HandleFiring();
    }
    
    private void AimAtMouse()
    {
        // Simple mouse aiming - we'll fix input system later
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, 0f));
        mouseWorldPos.z = 0f;
        
        // For now, just point right
        transform.rotation = Quaternion.identity;
    }
    
    private void HandleFiring()
    {
        // Auto-fire for testing
        if (CanFire())
        {
            Fire();
        }
    }
    
    private bool CanFire()
    {
        return Time.time >= lastFireTime + playerStats.fireCooldown;
    }
    
    private void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("No projectile prefab assigned to cannon!");
            return;
        }
        
        lastFireTime = Time.time;
        
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        
        if (projectileScript != null)
        {
            Vector3 direction = firePoint.right; // Right direction in local space
            projectileScript.Initialize(direction, projectileSpeed, playerStats.damage);
        }
    }
}