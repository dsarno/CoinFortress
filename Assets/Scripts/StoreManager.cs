using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public GameObject storePanel;
    public TextMeshProUGUI coinsText;
    
    [Header("Ammo Section")]
    public Button buyAmmoPack;
    public TextMeshProUGUI ammoPackCostText;
    public Button upgradeAmmoTier;
    public TextMeshProUGUI ammoTierCostText;
    public TextMeshProUGUI currentAmmoTierText;
    
    [Header("Cannon Section")]
    public Button upgradeDamage;
    public TextMeshProUGUI damageCostText;
    public TextMeshProUGUI damageCurrentText;
    public Button upgradeFireRate;
    public TextMeshProUGUI fireRateCostText;
    public TextMeshProUGUI fireRateCurrentText;
    
    [Header("Shield Section")]
    public Button unlockShield;
    public TextMeshProUGUI unlockShieldCostText;
    public Button upgradeShieldHP;
    public TextMeshProUGUI shieldHPCostText;
    public TextMeshProUGUI shieldCurrentText;
    public Button repairShield;
    public TextMeshProUGUI repairShieldCostText;
    
    [Header("Pricing")]
    public int ammoPackCost = 10;
    public int ammoPackAmount = 5;
    public int ammoTierBaseCost = 50;
    public int damageUpgradeCost = 20;
    public int fireRateUpgradeCost = 25;
    public int shieldUnlockCost = 100;
    public int shieldHPUpgradeCost = 30;
    public int shieldHPUpgradeAmount = 3;
    public int shieldRepairCost = 15;
    
    private void Start()
    {
        // Setup button listeners
        if (buyAmmoPack != null) buyAmmoPack.onClick.AddListener(BuyAmmoPack);
        if (upgradeAmmoTier != null) upgradeAmmoTier.onClick.AddListener(UpgradeAmmoTier);
        if (upgradeDamage != null) upgradeDamage.onClick.AddListener(UpgradeDamage);
        if (upgradeFireRate != null) upgradeFireRate.onClick.AddListener(UpgradeFireRate);
        if (unlockShield != null) unlockShield.onClick.AddListener(UnlockShield);
        if (upgradeShieldHP != null) upgradeShieldHP.onClick.AddListener(UpgradeShieldHP);
        if (repairShield != null) repairShield.onClick.AddListener(RepairShield);
        
        // Hide store initially
        if (storePanel != null)
        {
            storePanel.SetActive(false);
        }
        
        UpdateUI();
    }
    
    public void OpenStore()
    {
        if (storePanel != null)
        {
            storePanel.SetActive(true);
            
            // Make sure the store blocks raycasts
            UnityEngine.UI.Image storeBg = storePanel.GetComponent<UnityEngine.UI.Image>();
            if (storeBg != null)
            {
                storeBg.raycastTarget = true;
            }
        }
        UpdateUI();
    }
    
    public void CloseStore()
    {
        Debug.Log("CloseStore() called");
        if (storePanel != null)
        {
            storePanel.SetActive(false);
            Debug.Log("Store panel set to inactive");
        }
        else
        {
            Debug.LogWarning("storePanel is null in CloseStore()!");
        }
    }
    
    private void UpdateUI()
    {
        if (playerStats == null) return;
        
        // Update coins display
        if (coinsText != null)
        {
            coinsText.text = $"Coins: {playerStats.coins}";
        }
        
        // Ammo section
        if (ammoPackCostText != null)
            ammoPackCostText.text = $"Cost: {ammoPackCost}";
        
        int tierCost = ammoTierBaseCost * (playerStats.ammoTier + 1);
        if (ammoTierCostText != null)
            ammoTierCostText.text = $"Cost: {tierCost}";
        
        if (currentAmmoTierText != null)
        {
            string[] tierNames = { "Standard", "Heavy", "Explosive" };
            currentAmmoTierText.text = $"Current: {tierNames[playerStats.ammoTier]}";
        }
        
        // Cannon section
        int damageCost = damageUpgradeCost * playerStats.damageLevel;
        if (damageCostText != null)
            damageCostText.text = $"Cost: {damageCost}";
        if (damageCurrentText != null)
            damageCurrentText.text = $"Damage: {playerStats.damage}";
        
        int fireRateCost = fireRateUpgradeCost * playerStats.fireRateLevel;
        if (fireRateCostText != null)
            fireRateCostText.text = $"Cost: {fireRateCost}";
        if (fireRateCurrentText != null)
            fireRateCurrentText.text = $"Cooldown: {playerStats.fireCooldown:F2}s";
        
        // Shield section
        if (unlockShieldCostText != null)
            unlockShieldCostText.text = $"Cost: {shieldUnlockCost}";
        if (shieldHPCostText != null)
            shieldHPCostText.text = $"Cost: {shieldHPUpgradeCost}";
        if (repairShieldCostText != null)
            repairShieldCostText.text = $"Cost: {shieldRepairCost}";
        if (shieldCurrentText != null)
            shieldCurrentText.text = $"Shield: {playerStats.shieldCurrentHP}/{playerStats.shieldMaxHP}";
        
        // Button interactability
        if (buyAmmoPack != null) buyAmmoPack.interactable = playerStats.coins >= ammoPackCost;
        if (upgradeAmmoTier != null) upgradeAmmoTier.interactable = playerStats.coins >= tierCost && playerStats.ammoTier < 2;
        if (upgradeDamage != null) upgradeDamage.interactable = playerStats.coins >= damageCost;
        if (upgradeFireRate != null) upgradeFireRate.interactable = playerStats.coins >= fireRateCost;
        if (unlockShield != null) unlockShield.interactable = playerStats.coins >= shieldUnlockCost && !playerStats.shieldUnlocked;
        if (upgradeShieldHP != null) upgradeShieldHP.interactable = playerStats.coins >= shieldHPUpgradeCost && playerStats.shieldUnlocked;
        if (repairShield != null) repairShield.interactable = playerStats.coins >= shieldRepairCost && 
                                     playerStats.shieldUnlocked && 
                                     playerStats.shieldCurrentHP < playerStats.shieldMaxHP;
        
        // Hide/show shield buttons
        if (unlockShield != null)
            unlockShield.gameObject.SetActive(!playerStats.shieldUnlocked);
        if (upgradeShieldHP != null)
            upgradeShieldHP.gameObject.SetActive(playerStats.shieldUnlocked);
        if (repairShield != null)
            repairShield.gameObject.SetActive(playerStats.shieldUnlocked);
    }
    
    private void BuyAmmoPack()
    {
        if (playerStats.SpendCoins(ammoPackCost))
        {
            playerStats.UpgradeAmmoCapacity(ammoPackAmount);
            playerStats.ammo += ammoPackAmount; // Also add to current ammo
            Debug.Log($"Bought ammo pack! +{ammoPackAmount} ammo");
            
            // Play purchase success sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseSuccess();
            }
            
            UpdateUI();
        }
        else
        {
            // Play purchase fail sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseFail();
            }
        }
    }
    
    private void UpgradeAmmoTier()
    {
        int cost = ammoTierBaseCost * (playerStats.ammoTier + 1);
        if (playerStats.SpendCoins(cost))
        {
            playerStats.UpgradeAmmoTier();
            Debug.Log($"Upgraded ammo to tier {playerStats.ammoTier}!");
            
            // Play purchase success sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseSuccess();
            }
            
            UpdateUI();
        }
        else
        {
            // Play purchase fail sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseFail();
            }
        }
    }
    
    private void UpgradeDamage()
    {
        int cost = damageUpgradeCost * playerStats.damageLevel;
        if (playerStats.SpendCoins(cost))
        {
            playerStats.UpgradeDamage();
            Debug.Log($"Upgraded damage to {playerStats.damage}!");
            
            // Play purchase success sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseSuccess();
            }
            
            UpdateUI();
        }
        else
        {
            // Play purchase fail sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseFail();
            }
        }
    }
    
    private void UpgradeFireRate()
    {
        int cost = fireRateUpgradeCost * playerStats.fireRateLevel;
        if (playerStats.SpendCoins(cost))
        {
            playerStats.UpgradeFireRate();
            Debug.Log($"Upgraded fire rate! Cooldown: {playerStats.fireCooldown:F2}s");
            
            // Play purchase success sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseSuccess();
            }
            
            UpdateUI();
        }
        else
        {
            // Play purchase fail sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseFail();
            }
        }
    }
    
    private void UnlockShield()
    {
        if (playerStats.SpendCoins(shieldUnlockCost))
        {
            playerStats.UnlockShield();
            Debug.Log("Shield unlocked!");
            
            // Play shield activate and purchase success sounds
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayShieldActivate();
                SoundManager.Instance.PlayPurchaseSuccess();
            }
            
            UpdateUI();
        }
        else
        {
            // Play purchase fail sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseFail();
            }
        }
    }
    
    private void UpgradeShieldHP()
    {
        if (playerStats.SpendCoins(shieldHPUpgradeCost))
        {
            playerStats.UpgradeShieldMaxHP(shieldHPUpgradeAmount);
            Debug.Log($"Upgraded shield max HP to {playerStats.shieldMaxHP}!");
            
            // Play purchase success sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseSuccess();
            }
            
            UpdateUI();
        }
        else
        {
            // Play purchase fail sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseFail();
            }
        }
    }
    
    private void RepairShield()
    {
        if (playerStats.SpendCoins(shieldRepairCost))
        {
            playerStats.RepairShield();
            Debug.Log("Shield repaired!");
            
            // Play purchase success sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseSuccess();
            }
            
            UpdateUI();
        }
        else
        {
            // Play purchase fail sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPurchaseFail();
            }
        }
    }
}
