using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to automatically create the store UI and HUD
/// Attach this to an empty GameObject and click "Setup UI" in the inspector
/// </summary>
public class UISetupHelper : MonoBehaviour
{
    public PlayerStats playerStats;
    public StoreManager storeManager;
    public LevelManager levelManager;
    
    [ContextMenu("Setup Complete UI")]
    public void SetupCompleteUI()
    {
        SetupHUD();
        SetupStoreUI();
        Debug.Log("UI setup complete!");
    }
    
    [ContextMenu("Setup HUD Only")]
    public void SetupHUD()
    {
        // Find or create Canvas
        Canvas canvas = FindOrCreateCanvas();
        
        // Create HUD Panel
        GameObject hudPanel = new GameObject("HUD Panel");
        hudPanel.transform.SetParent(canvas.transform, false);
        RectTransform hudRect = hudPanel.AddComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0, 0);
        hudRect.anchorMax = new Vector2(1, 1);
        hudRect.sizeDelta = Vector2.zero;
        
        // Create HUD texts (top-left corner)
        CreateHUDText(hudPanel.transform, "Ammo Text", new Vector2(10, -10), TextAnchor.UpperLeft);
        CreateHUDText(hudPanel.transform, "Health Text", new Vector2(10, -40), TextAnchor.UpperLeft);
        CreateHUDText(hudPanel.transform, "Shield Text", new Vector2(10, -70), TextAnchor.UpperLeft);
        
        // Create coins and tier (top-right corner)
        CreateHUDText(hudPanel.transform, "Coins Text", new Vector2(-10, -10), TextAnchor.UpperRight);
        CreateHUDText(hudPanel.transform, "Ammo Tier Text", new Vector2(-10, -40), TextAnchor.UpperRight);
        
        // Add GameHUD component
        GameHUD hud = hudPanel.AddComponent<GameHUD>();
        hud.playerStats = playerStats;
        hud.ammoText = hudPanel.transform.Find("Ammo Text").GetComponent<TextMeshProUGUI>();
        hud.healthText = hudPanel.transform.Find("Health Text").GetComponent<TextMeshProUGUI>();
        hud.shieldText = hudPanel.transform.Find("Shield Text").GetComponent<TextMeshProUGUI>();
        hud.coinsText = hudPanel.transform.Find("Coins Text").GetComponent<TextMeshProUGUI>();
        hud.ammoTierText = hudPanel.transform.Find("Ammo Tier Text").GetComponent<TextMeshProUGUI>();
        
        Debug.Log("HUD created!");
    }
    
    [ContextMenu("Setup Store UI Only")]
    public void SetupStoreUI()
    {
        Canvas canvas = FindOrCreateCanvas();
        
        // Create fullscreen blocker behind store
        GameObject blocker = new GameObject("Store Blocker");
        blocker.transform.SetParent(canvas.transform, false);
        RectTransform blockerRect = blocker.AddComponent<RectTransform>();
        blockerRect.anchorMin = Vector2.zero;
        blockerRect.anchorMax = Vector2.one;
        blockerRect.sizeDelta = Vector2.zero;
        Image blockerImage = blocker.AddComponent<Image>();
        blockerImage.color = new Color(0, 0, 0, 0.5f); // Semi-transparent black
        blockerImage.raycastTarget = true;
        
        // Create Store Panel
        GameObject storePanel = new GameObject("Store Panel");
        storePanel.transform.SetParent(canvas.transform, false);
        RectTransform storeRect = storePanel.AddComponent<RectTransform>();
        storeRect.anchorMin = new Vector2(0.5f, 0.5f);
        storeRect.anchorMax = new Vector2(0.5f, 0.5f);
        storeRect.sizeDelta = new Vector2(600, 700);
        
        // Add background
        Image storeBg = storePanel.AddComponent<Image>();
        storeBg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        storeBg.raycastTarget = true;
        
        // Parent blocker and store panel together
        GameObject storeRoot = new GameObject("Store Root");
        storeRoot.transform.SetParent(canvas.transform, false);
        RectTransform rootRect = storeRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.sizeDelta = Vector2.zero;
        
        blocker.transform.SetParent(storeRoot.transform, false);
        storePanel.transform.SetParent(storeRoot.transform, false);
        
        // Title
        CreateStoreText(storePanel.transform, "Store Title", new Vector2(0, 320), "UPGRADE STORE", 32);
        
        // Coins display
        GameObject coinsDisplay = CreateStoreText(storePanel.transform, "Coins Display", new Vector2(0, 270), "Coins: 0", 24);
        
        // Start Level Button
        GameObject startBtn = CreateStoreButton(storePanel.transform, "Start Level Button", new Vector2(0, -320), "START LEVEL");
        
        // Ammo Section
        CreateStoreText(storePanel.transform, "Ammo Section Title", new Vector2(0, 200), "=== AMMO ===", 20);
        GameObject currentAmmoTier = CreateStoreText(storePanel.transform, "Current Ammo Tier", new Vector2(0, 170), "Current: Standard", 16);
        GameObject buyAmmoPack = CreateStoreButton(storePanel.transform, "Buy Ammo Pack", new Vector2(0, 130), "Buy Ammo Pack (+5) - Cost: 10");
        GameObject upgradeAmmoTier = CreateStoreButton(storePanel.transform, "Upgrade Ammo Tier", new Vector2(0, 90), "Upgrade Ammo Tier - Cost: 50");
        
        // Cannon Section
        CreateStoreText(storePanel.transform, "Cannon Section Title", new Vector2(0, 40), "=== CANNON ===", 20);
        GameObject damageDisplay = CreateStoreText(storePanel.transform, "Damage Current", new Vector2(-150, 10), "Damage: 1", 14);
        GameObject upgradeDamage = CreateStoreButton(storePanel.transform, "Upgrade Damage", new Vector2(-150, -20), "Upgrade - Cost: 20", new Vector2(250, 40));
        GameObject fireRateDisplay = CreateStoreText(storePanel.transform, "Fire Rate Current", new Vector2(150, 10), "Cooldown: 0.5s", 14);
        GameObject upgradeFireRate = CreateStoreButton(storePanel.transform, "Upgrade Fire Rate", new Vector2(150, -20), "Upgrade - Cost: 25", new Vector2(250, 40));
        
        // Shield Section
        CreateStoreText(storePanel.transform, "Shield Section Title", new Vector2(0, -80), "=== SHIELD ===", 20);
        GameObject shieldDisplay = CreateStoreText(storePanel.transform, "Shield Current", new Vector2(0, -110), "Shield: 0/0", 14);
        GameObject unlockShield = CreateStoreButton(storePanel.transform, "Unlock Shield", new Vector2(-100, -140), "Unlock - Cost: 100", new Vector2(180, 40));
        GameObject upgradeShieldHP = CreateStoreButton(storePanel.transform, "Upgrade Shield HP", new Vector2(100, -140), "Upgrade HP - Cost: 30", new Vector2(180, 40));
        GameObject repairShield = CreateStoreButton(storePanel.transform, "Repair Shield", new Vector2(0, -190), "Repair Shield - Cost: 15", new Vector2(280, 40));
        
        // Setup StoreManager
        if (storeManager == null)
        {
            storeManager = canvas.gameObject.AddComponent<StoreManager>();
        }
        
        storeManager.playerStats = playerStats;
        storeManager.storePanel = storeRoot; // Use root instead of just panel
        storeManager.coinsText = coinsDisplay.GetComponent<TextMeshProUGUI>();
        
        // Ammo buttons
        storeManager.buyAmmoPack = buyAmmoPack.GetComponent<Button>();
        storeManager.ammoPackCostText = buyAmmoPack.GetComponentInChildren<TextMeshProUGUI>();
        storeManager.upgradeAmmoTier = upgradeAmmoTier.GetComponent<Button>();
        storeManager.ammoTierCostText = upgradeAmmoTier.GetComponentInChildren<TextMeshProUGUI>();
        storeManager.currentAmmoTierText = currentAmmoTier.GetComponent<TextMeshProUGUI>();
        
        // Cannon buttons
        storeManager.upgradeDamage = upgradeDamage.GetComponent<Button>();
        storeManager.damageCostText = upgradeDamage.GetComponentInChildren<TextMeshProUGUI>();
        storeManager.damageCurrentText = damageDisplay.GetComponent<TextMeshProUGUI>();
        storeManager.upgradeFireRate = upgradeFireRate.GetComponent<Button>();
        storeManager.fireRateCostText = upgradeFireRate.GetComponentInChildren<TextMeshProUGUI>();
        storeManager.fireRateCurrentText = fireRateDisplay.GetComponent<TextMeshProUGUI>();
        
        // Shield buttons
        storeManager.unlockShield = unlockShield.GetComponent<Button>();
        storeManager.unlockShieldCostText = unlockShield.GetComponentInChildren<TextMeshProUGUI>();
        storeManager.upgradeShieldHP = upgradeShieldHP.GetComponent<Button>();
        storeManager.shieldHPCostText = upgradeShieldHP.GetComponentInChildren<TextMeshProUGUI>();
        storeManager.shieldCurrentText = shieldDisplay.GetComponent<TextMeshProUGUI>();
        storeManager.repairShield = repairShield.GetComponent<Button>();
        storeManager.repairShieldCostText = repairShield.GetComponentInChildren<TextMeshProUGUI>();
        
        // Hook up Start Level button to LevelManager
        if (levelManager != null)
        {
            startBtn.GetComponent<Button>().onClick.AddListener(levelManager.BeginLevel);
        }
        
        Debug.Log("Store UI created!");
    }
    
    private Canvas FindOrCreateCanvas()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Add EventSystem if not present
            if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        return canvas;
    }
    
    private GameObject CreateHUDText(Transform parent, string name, Vector2 position, TextAnchor alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        
        // Set anchors based on alignment
        if (alignment == TextAnchor.UpperLeft)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
        }
        else if (alignment == TextAnchor.UpperRight)
        {
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
        }
        
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 30);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = name;
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = alignment == TextAnchor.UpperLeft ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        
        return textObj;
    }
    
    private GameObject CreateStoreText(Transform parent, string name, Vector2 position, string content, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(500, 40);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        return textObj;
    }
    
    private GameObject CreateStoreButton(Transform parent, string name, Vector2 position, string label, Vector2? customSize = null)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = customSize ?? new Vector2(400, 40);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.4f, 0.6f);
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.4f, 0.6f);
        colors.highlightedColor = new Color(0.3f, 0.5f, 0.7f);
        colors.pressedColor = new Color(0.15f, 0.35f, 0.55f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f);
        button.colors = colors;
        
        // Add text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        
        return buttonObj;
    }
}
