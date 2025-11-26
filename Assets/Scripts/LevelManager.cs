using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    [Header("References")]
    public PlayerStats playerStats;
    public FortressSpawner fortressSpawner;
    public StoreManager storeManager;
    public GameObject mainMenuPanel;
    
    [Header("Level Settings")]
    public int currentLevel = 1;
    public bool levelInProgress = false;
    public bool hasTimeLimit = false;
    public float timeLimit = 30f;
    public float timeElapsed = 0f;
    
    [Header("Coin Spawning")]
    public GameObject coinPrefab; // You'll need to create this
    public int coinsPerLevel = 50;
    public float coinFountainDuration = 2f;
    public float coinSpawnRate = 0.1f;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // MainMenuController handles showing the menu on start
    }

    private void Update()
    {
        if (levelInProgress && hasTimeLimit)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= timeLimit)
            {
                LevelFailed();
            }
        }
    }
    
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        
        if (storeManager != null)
        {
            storeManager.CloseStore();
        }
        
        levelInProgress = false;
    }
    
    public void StartLevel()
    {
        if (storeManager != null)
        {
            storeManager.OpenStore();
        }
        
        levelInProgress = false;
    }
    
    public void BeginLevel()
    {
        // Start the level with a small delay to allow button click sound to play
        StartCoroutine(BeginLevelCoroutine());
    }
    
    public void StartFirstLevel()
    {
        // Called from main menu "Start Game" button
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        else
        {
             Debug.LogWarning("LevelManager: MainMenuPanel reference is null! Trying to find it...");
             var panel = GameObject.Find("Main Menu Panel");
             if (panel != null) panel.SetActive(false);
        }
        
        BeginLevel();
    }
    
    private System.Collections.IEnumerator BeginLevelCoroutine()
    {
        // Wait briefly for button click sound to play
        yield return new WaitForSeconds(0.15f);
        
        // Close store if open
        if (storeManager != null)
        {
            storeManager.CloseStore();
        }
        else
        {
            Debug.LogWarning("StoreManager is null!");
        }
        
        // Load level config from progression manager
        LevelProgressionManager progressionManager = FindFirstObjectByType<LevelProgressionManager>();
        if (progressionManager != null)
        {
            progressionManager.LoadLevel(progressionManager.GetCurrentLevel());
            
            // Apply time limit from config
            LevelConfig config = progressionManager.GetCurrentLevel();
            if (config != null)
            {
                hasTimeLimit = config.hasTimeLimit;
                timeLimit = config.timeLimitSeconds;
            }
            else
            {
                // Default fallback
                hasTimeLimit = true;
                timeLimit = 30f;
            }
        }
        else
        {
            Debug.LogWarning("LevelProgressionManager is null!");
            // Default fallback
            hasTimeLimit = true;
            timeLimit = 30f;
        }
        
        timeElapsed = 0f;
        
        // Play level start sound and music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLevelStart();
            yield return new WaitForSeconds(0.2f);
            SoundManager.Instance.PlayLevelMusic(currentLevel);
        }
        
        // Refill ammo for new level
        if (playerStats != null)
        {
            playerStats.RefillForNewLevel();
        }
        else
        {
            Debug.LogWarning("PlayerStats is null!");
        }
        
        // Spawn fortress
        if (fortressSpawner != null)
        {
            fortressSpawner.fortressRoot = null;
            fortressSpawner.SpawnFortress();
        }
        else
        {
            fortressSpawner = FindFirstObjectByType<FortressSpawner>();
            if (fortressSpawner != null)
            {
                fortressSpawner.fortressRoot = null;
                fortressSpawner.SpawnFortress();
            }
            else
            {
                Debug.LogWarning("FortressSpawner is null and could not be found!");
            }
        }
        
        levelInProgress = true;
    }
    
    public void OnCoreDestroyed(Vector3 corePosition)
    {
        if (!levelInProgress) return;
        
        levelInProgress = false;
        
        // Play level complete sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLevelComplete();
        }
        
        // Start coin fountain
        StartCoroutine(SpawnCoinFountain(corePosition));
        
        // Open store after 2 seconds
        Invoke(nameof(PrepareNextLevel), 2.0f);
    }
    
    private System.Collections.IEnumerator SpawnCoinFountain(Vector3 position)
    {
        // Award coins directly for now (can add coin pickup objects later)
        if (playerStats != null)
        {
            playerStats.AddCoins(coinsPerLevel);
        }
        
        for (int i = 0; i < coinsPerLevel; i++)
        {
            if (coinPrefab != null)
            {
                GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity);
                CoinController cc = coin.GetComponent<CoinController>();
                if (cc != null)
                {
                    cc.TriggerVisualEffect(1.0f); // Wait 1 second before starting fade
                    cc.givesCoins = false; // Already awarded by LevelManager
                }
            }
            yield return new WaitForSeconds(coinSpawnRate);
        }
    }
    
    private void PrepareNextLevel()
    {
        currentLevel++;
        
        // Clear old fortress
        GameObject fortressRoot = GameObject.Find("FortressRoot");
        if (fortressRoot != null)
        {
            Destroy(fortressRoot);
        }
        
        // Get next level from progression manager
        LevelProgressionManager progressionManager = FindFirstObjectByType<LevelProgressionManager>();
        if (progressionManager != null)
        {
            progressionManager.StartNextLevel();
        }
        
        // Open store
        StartLevel();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LevelFailed()
    {
        levelInProgress = false;
        Debug.Log("Level Failed: Time Limit Reached!");
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGameOver();
        }
        
        // Show game over UI or restart level
        // For now, just restart the level after a delay
        Invoke(nameof(RestartLevel), 2f);
    }

    private void RestartLevel()
    {
        // Reload current level setup
        BeginLevel();
    }
}
