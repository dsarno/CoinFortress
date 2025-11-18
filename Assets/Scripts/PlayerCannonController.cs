using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCannonController : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    
    [Header("Settings")]
    public float projectileSpeed = 15f;
    public float fireForce = 10f; // Initial upward force
    
    private PlayerStats playerStats;
    private float lastFireTime;
    private Camera mainCamera;
    private Mouse mouse;
    
    private void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();
        mainCamera = Camera.main;
        mouse = Mouse.current;
        
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
        
        // Fire on left click with cooldown
        if (mouse.leftButton.isPressed && CanFire())
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
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing projectile prefab or fire point!");
            return;
        }
        
        lastFireTime = Time.time;
        
        // Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        
        if (projectileScript != null)
        {
            // Fire in the direction the cannon is pointing
            Vector2 direction = firePoint.right;
            projectileScript.Initialize(direction, projectileSpeed, playerStats.damage);
        }
    }
}