using UnityEngine;

public class CannonManager : MonoBehaviour
{
    [Header("Cannon Prefabs")]
    public GameObject singleBarrelPrefab;
    public GameObject doubleBarrelPrefab;
    
    [Header("References")]
    public Transform cannonMountPoint;
    public PlayerStats playerStats;
    
    [Header("Positioning")]
    public Vector3 singleBarrelPosition = new Vector3(-3f, 0f, 0f);
    public Vector3 doubleBarrelPosition = new Vector3(-3f, 0f, 0f);
    
    public GameObject currentCannonInstance;

    [ContextMenu("Save Current Position")]
    public void SaveCurrentPosition()
    {
        if (currentCannonInstance != null)
        {
            if (playerStats != null && playerStats.doubleBarrelUnlocked)
            {
                doubleBarrelPosition = currentCannonInstance.transform.localPosition;
                Debug.Log($"Saved Double Barrel Position: {doubleBarrelPosition}");
            }
            else
            {
                singleBarrelPosition = currentCannonInstance.transform.localPosition;
                Debug.Log($"Saved Single Barrel Position: {singleBarrelPosition}");
            }
        }
    }
    private bool wasDoubleBarrelUnlocked = false;
    
    private void Start()
    {
        if (playerStats == null)
        {
            playerStats = GetComponentInParent<PlayerStats>();
        }
        
        // If no cannon is assigned, spawn the correct one
        if (currentCannonInstance == null)
        {
            UpdateCannonModel();
        }
        // If one is assigned, check if we need to upgrade immediately
        else if (playerStats != null && playerStats.doubleBarrelUnlocked)
        {
             UpdateCannonModel();
        }
        
        if (playerStats != null)
        {
            wasDoubleBarrelUnlocked = playerStats.doubleBarrelUnlocked;
        }
    }
    
    private void Update()
    {
        // Check for upgrade changes
        if (playerStats != null)
        {
            if (playerStats.doubleBarrelUnlocked != wasDoubleBarrelUnlocked)
            {
                UpdateCannonModel();
                wasDoubleBarrelUnlocked = playerStats.doubleBarrelUnlocked;
            }
        }
    }
    
    public void UpdateCannonModel()
    {
        // Determine which prefab to use
        GameObject prefabToUse = singleBarrelPrefab;
        if (playerStats != null && playerStats.doubleBarrelUnlocked)
        {
            prefabToUse = doubleBarrelPrefab;
        }
        
        if (prefabToUse == null)
        {
            Debug.LogWarning("Cannon prefab missing in CannonManager!");
            return;
        }
        
        // Destroy existing cannon
        if (currentCannonInstance != null)
        {
            Destroy(currentCannonInstance);
        }
        
        // Instantiate new cannon
        if (cannonMountPoint != null)
        {
            currentCannonInstance = Instantiate(prefabToUse, cannonMountPoint.position, cannonMountPoint.rotation, cannonMountPoint);
            
            // Ensure local transform is reset (prefabs might have offsets)
            // The prefab itself should have the correct local position relative to the player
            // But based on previous steps, the cannon was at local position (-3, 0, 0)
            currentCannonInstance.transform.localPosition = new Vector3(-3f, 0f, 0f); 
            currentCannonInstance.transform.localRotation = Quaternion.identity;
            
            // Reset scale but keep Z at 1
            currentCannonInstance.transform.localScale = new Vector3(1f, 1f, 1f);
            
            // If it's the single barrel, we might need to adjust scale to match what it was
            // The single barrel had scale (0.7558, 0.60015, 1) in the scene
            if (prefabToUse == singleBarrelPrefab)
            {
                 currentCannonInstance.transform.localScale = new Vector3(0.7558f, 0.60015f, 1f);
                 currentCannonInstance.transform.localPosition = singleBarrelPosition;
            }
            // The double barrel had scale (0.7558, 0.60015, 1) too when we set it up
            else if (prefabToUse == doubleBarrelPrefab)
            {
                 // Use scale 1 for double barrel as it's already sized correctly in prefab
                 currentCannonInstance.transform.localScale = Vector3.one;
                 currentCannonInstance.transform.localPosition = doubleBarrelPosition;
                 // Flip 180 on Y to face right
                 currentCannonInstance.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            
            // Ensure the cannon is active
            currentCannonInstance.SetActive(true);

            // Handle CannonBase visibility
            // If we are using the double barrel, we might need to hide the player's default CannonBase if it exists separately
            // Assuming "CannonBase" is a child of Player or a sibling
            Transform playerTransform = cannonMountPoint; // Assuming mount point is player
            if (playerTransform != null)
            {
                Transform baseObj = playerTransform.Find("CannonBase");
                if (baseObj != null)
                {
                    // Hide base if using double barrel (which has its own base), show otherwise
                    bool showBase = (prefabToUse == singleBarrelPrefab);
                    baseObj.gameObject.SetActive(showBase);
                }
            }
            
            // Force update the controller reference in StoreManager if needed
            // (StoreManager finds it via FindFirstObjectByType, so it should be fine on next frame/click)
        }
    }
}
