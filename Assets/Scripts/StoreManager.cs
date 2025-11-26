using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public PlayerCannonController cannonController;
    public GameObject storePanel;
    public TextMeshProUGUI coinsText; // HUD coins text
    
    [Header("Buttons")]
    public Button startLevelButton;
    public Button buyAmmoPackButton;
    public Button buyBiggerBallsButton; // Damage Upgrade
    public Button buyNewCannonButton;   // Fire Rate Upgrade
    
    [Header("Cost Texts")]
    public TextMeshProUGUI ammoPackCostText;
    public TextMeshProUGUI biggerBallsCostText;
    public TextMeshProUGUI newCannonCostText;
    
    [Header("Pricing")]
    public int ammoPackCost = 10;
    public int ammoPackAmount = 5;
    
    public int biggerBallsBaseCost = 10;
    public int newCannonBaseCost = 10;
    
    private void Start()
    {
        // Auto-find references if missing (Helper for setup)
        if (storePanel == null) storePanel = GameObject.Find("Store Panel");
        
        if (storePanel != null)
        {
            if (startLevelButton == null) 
                startLevelButton = storePanel.transform.Find("Start Level Button")?.GetComponent<Button>();
                
            if (buyAmmoPackButton == null) 
                buyAmmoPackButton = storePanel.transform.Find("Buy Ammo Pack")?.GetComponent<Button>();
                
            if (buyBiggerBallsButton == null) 
                buyBiggerBallsButton = storePanel.transform.Find("Buy Bigger Balls")?.GetComponent<Button>();
                
            if (buyNewCannonButton == null) 
                buyNewCannonButton = storePanel.transform.Find("Canon Upgrade")?.GetComponent<Button>(); // Note: "Canon" spelling in hierarchy
                
            // Find Cost Texts
            if (ammoPackCostText == null && buyAmmoPackButton != null)
                ammoPackCostText = buyAmmoPackButton.transform.Find("Ammo Cost/Text")?.GetComponent<TextMeshProUGUI>();
                
            if (biggerBallsCostText == null && buyBiggerBallsButton != null)
                biggerBallsCostText = buyBiggerBallsButton.transform.Find("Balls Cost/Text")?.GetComponent<TextMeshProUGUI>();
                
            if (newCannonCostText == null && buyNewCannonButton != null)
                newCannonCostText = buyNewCannonButton.transform.Find("Cannon Cost/Text")?.GetComponent<TextMeshProUGUI>();
        }
        
        if (coinsText == null)
        {
            GameObject coinsObj = GameObject.Find("Coins Text");
            if (coinsObj != null) coinsText = coinsObj.GetComponent<TextMeshProUGUI>();
        }
        
        if (playerStats == null) playerStats = FindFirstObjectByType<PlayerStats>();

        // Setup Listeners
        if (startLevelButton != null)
        {
            startLevelButton.onClick.RemoveAllListeners();
            startLevelButton.onClick.AddListener(OnStartLevelClicked);
        }
        
        if (buyAmmoPackButton != null)
        {
            buyAmmoPackButton.onClick.RemoveAllListeners();
            buyAmmoPackButton.onClick.AddListener(BuyAmmoPack);
            SuppressDefaultClick(buyAmmoPackButton);
        }
        
        if (buyBiggerBallsButton != null)
        {
            buyBiggerBallsButton.onClick.RemoveAllListeners();
            buyBiggerBallsButton.onClick.AddListener(BuyBiggerBalls);
            SuppressDefaultClick(buyBiggerBallsButton);
        }
        
        if (buyNewCannonButton != null)
        {
            buyNewCannonButton.onClick.RemoveAllListeners();
            buyNewCannonButton.onClick.AddListener(BuyNewCannon);
            SuppressDefaultClick(buyNewCannonButton);
        }
        
        // Initial UI Update
        UpdateUI();
    }
    
    public void OpenStore()
    {
        if (storePanel != null)
        {
            storePanel.SetActive(true);
            
            // Ensure raycast target for blocker
            Image storeBg = storePanel.GetComponent<Image>();
            if (storeBg != null) storeBg.raycastTarget = true;
        }
        UpdateUI();
    }
    
    public void CloseStore()
    {
        if (storePanel != null)
        {
            storePanel.SetActive(false);
        }
    }
    
    private void OnStartLevelClicked()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.BeginLevel();
        }
    }
    
    private void UpdateUI()
    {
        if (playerStats == null) return;
        
        // Update Coins
        if (coinsText != null)
        {
            coinsText.text = $"Coins: {playerStats.coins}";
        }
        
        // 1. Ammo Pack
        if (ammoPackCostText != null)
            ammoPackCostText.text = ammoPackCost.ToString();
            
        // if (buyAmmoPackButton != null)
        //    buyAmmoPackButton.interactable = playerStats.coins >= ammoPackCost;
            
        // 2. Bigger Balls (Damage)
        if (playerStats.damageLevel >= 3) // Max level (Mega)
        {
            if (biggerBallsCostText != null) biggerBallsCostText.text = "MAX";
            if (buyBiggerBallsButton != null)
            {
                buyBiggerBallsButton.interactable = false;
                UpdateBiggerBallsIcon(3); // Show max level icon
            }
        }
        else
        {
            int currentDamageCost = GetNextProjectileCost();
            if (biggerBallsCostText != null)
                biggerBallsCostText.text = currentDamageCost.ToString();
                
            if (buyBiggerBallsButton != null)
            {
                // buyBiggerBallsButton.interactable = playerStats.coins >= currentDamageCost;
                // Show NEXT level sprite
                UpdateBiggerBallsIcon(playerStats.damageLevel + 1);
            }
        }
            
        // 3. New Cannon (Fire Rate / Double Barrel)
        if (playerStats.doubleBarrelUnlocked)
        {
            // Show Fire Rate Upgrade cost
            int currentCannonCost = newCannonBaseCost * playerStats.fireRateLevel;
            if (newCannonCostText != null)
                newCannonCostText.text = currentCannonCost.ToString();
        }
        else
        {
            // Show Unlock Cost
            if (newCannonCostText != null)
                newCannonCostText.text = "50";
        }
            
        // if (buyNewCannonButton != null)
        //    buyNewCannonButton.interactable = playerStats.coins >= currentCannonCost;
    }
    
    private void BuyAmmoPack()
    {
        if (playerStats.SpendCoins(ammoPackCost))
        {
            playerStats.UpgradeAmmoCapacity(ammoPackAmount);
            playerStats.ammo += ammoPackAmount;
            
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSuccess();
            UpdateUI();
        }
        else
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseFail();
        }
    }
    
    private void BuyBiggerBalls()
    {
        int cost = GetNextProjectileCost();
        if (playerStats.SpendCoins(cost))
        {
            playerStats.UpgradeDamage();
            
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSuccess();
            UpdateUI();
        }
        else
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseFail();
        }
    }

    private int GetNextProjectileCost()
    {
        // Find PlayerCannonController if not set
        if (cannonController == null) cannonController = FindFirstObjectByType<PlayerCannonController>();

        if (cannonController != null && cannonController.projectilePrefabs != null)
        {
            int nextLevelIndex = playerStats.damageLevel + 1;
            if (nextLevelIndex < cannonController.projectilePrefabs.Length)
            {
                GameObject nextPrefab = cannonController.projectilePrefabs[nextLevelIndex];
                if (nextPrefab != null)
                {
                    Projectile p = nextPrefab.GetComponent<Projectile>();
                    if (p != null)
                    {
                        return p.upgradeCost;
                    }
                }
            }
        }
        
        // Fallback if something is missing
        return biggerBallsBaseCost * playerStats.damageLevel;
    }
    
    private void BuyNewCannon()
    {
        // If already unlocked, maybe upgrade fire rate?
        // For now, let's assume "New Cannon" means unlocking the double barrel
        if (playerStats.doubleBarrelUnlocked)
        {
            // Already unlocked, maybe upgrade fire rate instead?
            int cost = newCannonBaseCost * playerStats.fireRateLevel;
            if (playerStats.SpendCoins(cost))
            {
                playerStats.UpgradeFireRate();
                if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSuccess();
                UpdateUI();
            }
            else
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseFail();
            }
            return;
        }

        // Unlock Double Barrel
        int unlockCost = 50; // Set a cost for the double barrel
        if (playerStats.SpendCoins(unlockCost))
        {
            playerStats.UnlockDoubleBarrel();
            
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSuccess();
            UpdateUI();
        }
        else
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseFail();
        }
    }

    private void SuppressDefaultClick(Button btn)
    {
        var soundComp = btn.GetComponent<UIButtonSounds>();
        if (soundComp != null)
        {
            soundComp.suppressDefaultClickSound = true;
        }
    }

    private void UpdateBiggerBallsIcon(int levelIndex)
    {
        if (buyBiggerBallsButton == null) return;
        
        // Find PlayerCannonController if not set
        if (cannonController == null) cannonController = FindFirstObjectByType<PlayerCannonController>();
        
        if (cannonController != null && cannonController.projectilePrefabs != null && levelIndex < cannonController.projectilePrefabs.Length)
        {
            GameObject prefab = cannonController.projectilePrefabs[levelIndex];
            if (prefab != null)
            {
                SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Image icon = buyBiggerBallsButton.transform.Find("Icon")?.GetComponent<Image>();
                    if (icon != null)
                    {
                        icon.sprite = sr.sprite;
                        icon.color = sr.color; // Apply tint if any
                    }
                }
            }
        }
    }
}
