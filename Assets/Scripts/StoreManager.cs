using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
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
        }
        
        if (buyBiggerBallsButton != null)
        {
            buyBiggerBallsButton.onClick.RemoveAllListeners();
            buyBiggerBallsButton.onClick.AddListener(BuyBiggerBalls);
        }
        
        if (buyNewCannonButton != null)
        {
            buyNewCannonButton.onClick.RemoveAllListeners();
            buyNewCannonButton.onClick.AddListener(BuyNewCannon);
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
            
        if (buyAmmoPackButton != null)
            buyAmmoPackButton.interactable = playerStats.coins >= ammoPackCost;
            
        // 2. Bigger Balls (Damage)
        int currentDamageCost = biggerBallsBaseCost * playerStats.damageLevel;
        if (biggerBallsCostText != null)
            biggerBallsCostText.text = currentDamageCost.ToString();
            
        if (buyBiggerBallsButton != null)
            buyBiggerBallsButton.interactable = playerStats.coins >= currentDamageCost;
            
        // 3. New Cannon (Fire Rate)
        int currentCannonCost = newCannonBaseCost * playerStats.fireRateLevel;
        if (newCannonCostText != null)
            newCannonCostText.text = currentCannonCost.ToString();
            
        if (buyNewCannonButton != null)
            buyNewCannonButton.interactable = playerStats.coins >= currentCannonCost;
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
        int cost = biggerBallsBaseCost * playerStats.damageLevel;
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
    
    private void BuyNewCannon()
    {
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
    }
}
