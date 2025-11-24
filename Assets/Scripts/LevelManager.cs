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
        Debug.Log("LevelManager: StartFirstLevel called.");

        // Called from main menu "Start Game" button
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Debug.Log("LevelManager: Main Menu hidden.");
        }
        else
        {
             Debug.LogWarning("LevelManager: MainMenuPanel reference is null! Trying to find it...");
             // Last ditch attempt to find it to hide it
             var panel = GameObject.Find("Main Menu Panel");
             if (panel != null) panel.SetActive(false);
        }
        
        // Skip the store for the first level, just start playing
        BeginLevel();
    }
    
    private System.Collections.IEnumerator BeginLevelCoroutine()
    {
        Debug.Log("BeginLevel() called!");
        
        // Wait briefly for button click sound to play
        yield return new WaitForSeconds(0.15f);
        
        // Called when player clicks "Start Level" button in store
        if (storeManager != null)
        {
            storeManager.CloseStore();
            Debug.Log("Store closed");
        }
        else
        {
            Debug.LogWarning("StoreManager is null!");
        }
        
        // Play level start sound and music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLevelStart();
            // Delay music start slightly so level start sound is heard
            yield return new WaitForSeconds(0.2f);
            SoundManager.Instance.PlayLevelMusic(currentLevel);
        }
        
        // Refill ammo for new level
        if (playerStats != null)
        {
            playerStats.RefillForNewLevel();
            Debug.Log($"Ammo refilled to {playerStats.ammo}");
        }
        else
        {
            Debug.LogWarning("PlayerStats is null!");
        }
        
        // Spawn fortress (it handles clearing internally)
        if (fortressSpawner != null)
        {
            // Clear the fortress root reference so spawner creates a fresh one
            fortressSpawner.fortressRoot = null;
            
            fortressSpawner.SpawnFortress();
            Debug.Log("Fortress spawned");
        }
        else
        {
            Debug.LogWarning("FortressSpawner is null!");
        }
        
        levelInProgress = true;
        Debug.Log($"Level {currentLevel} started! levelInProgress = {levelInProgress}");
    }
    
    public void OnCoreDestroyed(Vector3 corePosition)
    {
        if (!levelInProgress) return;
        
        Debug.Log("Core destroyed! Level complete!");
        levelInProgress = false;
        
        // Play level complete sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLevelComplete();
        }
        
        // Start coin fountain
        StartCoroutine(SpawnCoinFountain(corePosition));
        
        // Wait a bit then open store for next level
        Invoke(nameof(PrepareNextLevel), coinFountainDuration + 1f);
    }
    
    private System.Collections.IEnumerator SpawnCoinFountain(Vector3 position)
    {
        // Award coins directly for now (can add coin pickup objects later)
        if (playerStats != null)
        {
            playerStats.AddCoins(coinsPerLevel);
        }
        
        Debug.Log($"Coin fountain! +{coinsPerLevel} coins!");
        
        // TODO: Spawn actual coin particles/objects
        // For now, just wait
        yield return new WaitForSeconds(coinFountainDuration);
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
        
        // Load next level configuration if LevelProgressionManager exists
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
}
