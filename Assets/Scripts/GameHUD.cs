using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    
    [Header("UI Elements")]
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI ammoTierText;
    public TextMeshProUGUI timerText;
    
    private void Start()
    {
        if (playerStats != null)
        {
            // Subscribe to events
            playerStats.onAmmoChanged.AddListener(UpdateHUD);
            playerStats.onCoinsChanged.AddListener(UpdateHUD);
            playerStats.onStatsChanged.AddListener(UpdateHUD);
        }
        
        UpdateHUD();
    }
    
    private void Update()
    {
        // Update every frame for real-time feedback
        UpdateHUD();
    }
    
    private void UpdateHUD()
    {
        if (playerStats == null) return;
        
        // Ammo
        if (ammoText != null)
        {
            if (playerStats.ammo > 0)
            {
                ammoText.text = $"Ammo: {playerStats.ammo}/{playerStats.maxAmmo}";
                ammoText.color = Color.white;
            }
            else
            {
                ammoText.text = "Ammo: OUT! (Weak shots only)";
                ammoText.color = Color.red;
            }
        }
        
        // Coins
        if (coinsText != null)
        {
            coinsText.text = $"Coins: {playerStats.coins}";
        }
        
        // Health
        if (healthText != null)
        {
            healthText.text = $"HP: {playerStats.currentHP}/{playerStats.maxHP}";
            
            // Color code health
            float healthPercent = (float)playerStats.currentHP / playerStats.maxHP;
            if (healthPercent > 0.5f)
                healthText.color = Color.green;
            else if (healthPercent > 0.25f)
                healthText.color = Color.yellow;
            else
                healthText.color = Color.red;
        }
        
        // Shield
        if (shieldText != null)
        {
            if (playerStats.shieldUnlocked)
            {
                shieldText.text = $"Shield: {playerStats.shieldCurrentHP}/{playerStats.shieldMaxHP}";
                shieldText.gameObject.SetActive(true);
                
                // Color code shield
                if (playerStats.shieldCurrentHP > 0)
                    shieldText.color = Color.cyan;
                else
                    shieldText.color = Color.gray;
            }
            else
            {
                shieldText.gameObject.SetActive(false);
            }
        }
        
        // Ammo Tier
        if (ammoTierText != null)
        {
            string[] tierNames = { "Standard", "Heavy", "Explosive" };
            string[] tierColors = { "#FFFFFF", "#FFD700", "#FF4500" };
            
            ammoTierText.text = $"Ammo: <color={tierColors[playerStats.ammoTier]}>{tierNames[playerStats.ammoTier]}</color>";
        }

        // Timer
        if (timerText != null && LevelManager.Instance != null)
        {
            if (LevelManager.Instance.isGatheringPhase)
            {
                // Show gathering countdown
                float timeLeft = Mathf.Max(0, LevelManager.Instance.gatheringTimer);
                timerText.text = $"GATHER: {timeLeft:0.0}";
                timerText.color = Color.yellow;
                
                if (!timerText.gameObject.activeSelf) 
                    timerText.gameObject.SetActive(true);
            }
            else if (LevelManager.Instance.levelInProgress && LevelManager.Instance.hasTimeLimit)
            {
                // Show level timer
                float timeLeft = Mathf.Max(0, LevelManager.Instance.timeLimit - LevelManager.Instance.timeElapsed);
                int minutes = Mathf.FloorToInt(timeLeft / 60F);
                int seconds = Mathf.FloorToInt(timeLeft - minutes * 60);
                timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
                
                // Ensure it's active
                if (!timerText.gameObject.activeSelf) 
                    timerText.gameObject.SetActive(true);
                
                if (timeLeft <= 10f)
                {
                    timerText.color = Color.red;
                }
                else
                {
                    timerText.color = Color.white;
                }
            }
            else
            {
                // Hide if not needed
                if (timerText.gameObject.activeSelf)
                    timerText.gameObject.SetActive(false);
            }
        }
    }
    
    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.onAmmoChanged.RemoveListener(UpdateHUD);
            playerStats.onCoinsChanged.RemoveListener(UpdateHUD);
            playerStats.onStatsChanged.RemoveListener(UpdateHUD);
        }
    }
}
