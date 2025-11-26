using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    [Header("References")]
    public PlayerStats playerStats;
    public FortressSpawner fortressSpawner;
    public StoreManager storeManager;
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    
    [Header("Level Settings")]
    public int currentLevel = 1;
    public bool levelInProgress = false;
    public bool isGatheringPhase = false;
    public float gatheringTime = 5f;
    public float gatheringTimer = 0f;
    public bool hasTimeLimit = false;
    public float timeLimit = 30f;
    public float timeElapsed = 0f;
    
    [Header("Coin Spawning")]
    public GameObject coinPrefab; // You'll need to create this
    public int coinsPerLevel = 50;
    public float coinFountainDuration = 2f;
    public float coinSpawnRate = 0.1f;
    
    private int lastSecondTicked = -1;
    private Coroutine coinFountainCoroutine;
    
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
        RefreshReferences();
        // MainMenuController handles showing the menu on start
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshReferences();
    }

    private void RefreshReferences()
    {
        // Find PlayerStats
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        // Find FortressSpawner
        if (fortressSpawner == null)
        {
            fortressSpawner = FindFirstObjectByType<FortressSpawner>();
        }

        // Find StoreManager
        if (storeManager == null)
        {
            storeManager = FindFirstObjectByType<StoreManager>();
        }

        // Find UI Panels
        Transform canvasTrans = GameObject.Find("Canvas")?.transform;
        if (canvasTrans != null)
        {
            if (gameOverPanel == null)
            {
                Transform panelTrans = canvasTrans.Find("Game Over Panel");
                if (panelTrans != null) gameOverPanel = panelTrans.gameObject;
            }

            if (mainMenuPanel == null)
            {
                Transform panelTrans = canvasTrans.Find("HUD Panel/Main Menu Panel");
                if (panelTrans != null) mainMenuPanel = panelTrans.gameObject;
            }
        }

        if (gameOverPanel == null) 
        {
            Debug.LogError("LevelManager: Game Over Panel not found!");
        }
        else
        {
            // Dynamically hook up the Restart Button to ensure it works after scene reload
            Button restartBtn = gameOverPanel.transform.Find("Play Again Button")?.GetComponent<Button>();
            if (restartBtn != null)
            {
                restartBtn.onClick.RemoveAllListeners();
                restartBtn.onClick.AddListener(RestartGame);
                
                // Ensure sound component is attached
                if (restartBtn.GetComponent<UIButtonSounds>() == null)
                {
                    restartBtn.gameObject.AddComponent<UIButtonSounds>();
                }
            }
            else
            {
                Debug.LogWarning("LevelManager: Play Again Button not found in Game Over Panel!");
            }
        }

        if (mainMenuPanel == null) Debug.LogWarning("LevelManager: Main Menu Panel not found!");
        
        // Ensure EventSystem exists
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private void Update()
    {
        if (levelInProgress && hasTimeLimit)
        {
            timeElapsed += Time.deltaTime;
            float timeLeft = timeLimit - timeElapsed;

            if (timeLeft <= 5.0f && timeLeft > 0f)
            {
                int currentSecond = Mathf.CeilToInt(timeLeft);
                if (currentSecond != lastSecondTicked)
                {
                    if (SoundManager.Instance != null) SoundManager.Instance.PlayClockTick();
                    lastSecondTicked = currentSecond;
                }
            }

            if (timeElapsed >= timeLimit)
            {
                LevelFailed();
            }
        }
        else if (isGatheringPhase)
        {
            gatheringTimer -= Time.deltaTime;
            if (gatheringTimer <= 0f)
            {
                isGatheringPhase = false;
                PrepareNextLevel();
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
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

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
        lastSecondTicked = -1;
        
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
        if (fortressSpawner == null)
        {
            fortressSpawner = FindFirstObjectByType<FortressSpawner>();
        }

        if (fortressSpawner != null)
        {
            fortressSpawner.fortressRoot = null;
            fortressSpawner.SpawnFortress();
            Debug.Log("LevelManager: Fortress spawned.");
        }
        else
        {
            Debug.LogError("FortressSpawner is null and could not be found!");
        }
        
        levelInProgress = true;
        isGatheringPhase = false;
    }
    
    public void OnCoreDestroyed(Vector3 corePosition)
    {
        if (!levelInProgress) return;
        
        levelInProgress = false;
        isGatheringPhase = true;
        gatheringTimer = gatheringTime;
        
        // Play level complete sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLevelComplete();
        }
        
        // Start coin fountain
        if (coinFountainCoroutine != null) StopCoroutine(coinFountainCoroutine);
        coinFountainCoroutine = StartCoroutine(SpawnCoinFountain(corePosition));
    }
    
    private System.Collections.IEnumerator SpawnCoinFountain(Vector3 position)
    {
        // Removed automatic coin award. Coins must be collected physically.
        
        for (int i = 0; i < coinsPerLevel; i++)
        {
            if (coinPrefab != null)
            {
                GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity);
                CoinController cc = coin.GetComponent<CoinController>();
                if (cc != null)
                {
                    // Coins are now interactive and give money when collected
                    cc.givesCoins = true; 
                    // Auto-fade after a longer delay if not collected
                    cc.TriggerVisualEffect(3.0f); 
                }
            }
            yield return new WaitForSeconds(coinSpawnRate);
        }
    }
    
    private void PrepareNextLevel()
    {
        currentLevel++;
        
        // Stop spawning new coins
        if (coinFountainCoroutine != null)
        {
            StopCoroutine(coinFountainCoroutine);
            coinFountainCoroutine = null;
        }
        
        // Cleanup remaining coins
        CoinController[] coins = FindObjectsByType<CoinController>(FindObjectsSortMode.None);
        foreach (var coin in coins)
        {
            if (coin != null)
            {
                Destroy(coin.gameObject);
            }
        }

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
        // Reset progression
        LevelProgressionManager progressionManager = FindFirstObjectByType<LevelProgressionManager>();
        if (progressionManager != null)
        {
            progressionManager.ResetProgression();
        }

        // Reset player stats
        if (playerStats != null)
        {
            playerStats.ResetForNewGame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LevelFailed()
    {
        levelInProgress = false;
        Debug.Log("Level Failed: Time Limit Reached!");
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.FadeOutMusic(1.0f);
            SoundManager.Instance.PlayBooing();
            SoundManager.Instance.PlayGameOver();
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // Ensure it's on top
            gameOverPanel.transform.SetAsLastSibling();
            Debug.Log("LevelManager: Game Over Panel activated.");
        }
        else
        {
            Debug.LogError("LevelManager: Game Over Panel is null in LevelFailed!");
        }
    }

    private void RestartLevel()
    {
        // Reload current level setup
        BeginLevel();
    }
}
