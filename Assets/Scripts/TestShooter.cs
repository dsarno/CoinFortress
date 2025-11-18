using UnityEngine;

public class TestShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    public float projectileSpeed = 20f; // Increased speed
    public Transform firePoint;
    
    private float nextFireTime;
    
    private void Start()
    {
        // Find or create fire point
        if (firePoint == null)
        {
            Transform foundFirePoint = transform.Find("FirePoint");
            if (foundFirePoint != null)
            {
                firePoint = foundFirePoint;
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
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    private void Fire()
    {
        if (projectilePrefab == null || firePoint == null) return;
        
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        
        if (projectileScript != null)
        {
            projectileScript.Initialize(Vector3.right, projectileSpeed, 1);
        }
        
        Debug.Log($"Fired projectile from {firePoint.position}!");
    }
}