using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Manages level progression and loading different level configurations
/// </summary>
public class LevelProgressionManager : MonoBehaviour
{
    [Header("Level Sequence")]
    public List<LevelConfig> levelSequence = new List<LevelConfig>();
    public bool randomizeLevels = false;
    
    [Header("Current Level")]
    public int currentLevelIndex = 0;
    
    [Header("References")]
    public LevelManager levelManager;
    public FortressSpawner fortressSpawner;
    public SpriteRenderer backgroundRenderer;
    public Camera mainCamera;
    
    private void Start()
    {
        if (levelSequence.Count == 0)
        {
            Debug.LogWarning("No levels in sequence! Add level configs to the list.");
        }
        RefreshReferences();
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
        if (fortressSpawner == null) fortressSpawner = FindFirstObjectByType<FortressSpawner>();
        if (levelManager == null) levelManager = FindFirstObjectByType<LevelManager>();
        if (backgroundRenderer == null) 
        {
            GameObject bg = GameObject.Find("Background");
            if (bg != null) backgroundRenderer = bg.GetComponent<SpriteRenderer>();
        }
        if (mainCamera == null) mainCamera = Camera.main;
    }
    
    public LevelConfig GetCurrentLevel()
    {
        if (levelSequence.Count == 0) return null;
        
        if (currentLevelIndex < 0 || currentLevelIndex >= levelSequence.Count)
        {
            currentLevelIndex = 0;
        }
        
        return levelSequence[currentLevelIndex];
    }
    
    public LevelConfig GetNextLevel()
    {
        if (levelSequence.Count == 0) return null;
        
        if (randomizeLevels)
        {
            currentLevelIndex = Random.Range(0, levelSequence.Count);
        }
        else
        {
            currentLevelIndex++;
            
            // Loop back to start if we've completed all levels
            if (currentLevelIndex >= levelSequence.Count)
            {
                currentLevelIndex = 0;
                Debug.Log("Completed all levels! Looping back to start.");
            }
        }
        
        return GetCurrentLevel();
    }
    
    public void LoadLevel(LevelConfig config)
    {
        if (config == null)
        {
            Debug.LogError("Cannot load null level config!");
            return;
        }
        
        // Ensure references are set
        if (fortressSpawner == null) fortressSpawner = FindFirstObjectByType<FortressSpawner>();
        if (levelManager == null) levelManager = FindFirstObjectByType<LevelManager>();
        if (backgroundRenderer == null) 
        {
            GameObject bg = GameObject.Find("Background");
            if (bg != null) backgroundRenderer = bg.GetComponent<SpriteRenderer>();
        }
        if (mainCamera == null) mainCamera = Camera.main;

        // Update level manager
        if (levelManager != null)
        {
            levelManager.currentLevel = config.levelNumber;
            levelManager.coinsPerLevel = config.coinsOnComplete;
        }
        
        // Update fortress spawner
        if (fortressSpawner != null)
        {
            if (config.fortressLayout != null)
            {
                fortressSpawner.layout = config.fortressLayout;
            }
            else
            {
                Debug.LogWarning($"Level Config '{config.levelName}' has no Fortress Layout assigned! Using default.");
            }
            
            // Update spawn position if spawn point exists
            if (fortressSpawner.spawnPoint != null)
            {
                fortressSpawner.spawnPoint.position = config.fortressSpawnPosition;
                Debug.Log($"LevelProgressionManager: Updated spawn point to {config.fortressSpawnPosition} for level {config.levelName}");
            }
            else
            {
                Debug.LogWarning("LevelProgressionManager: FortressSpawner has no spawn point assigned!");
            }
        }
        else
        {
            Debug.LogError("LevelProgressionManager: FortressSpawner not found!");
        }
        
        // Update background
        if (config.backgroundPrefab != null)
        {
            // Destroy old background if it exists (assuming it's a child of a container or we track it)
            // For now, let's assume we replace the object with the SpriteRenderer
            if (backgroundRenderer != null)
            {
                Destroy(backgroundRenderer.gameObject);
                backgroundRenderer = null;
            }
            
            // Instantiate new background
            GameObject bgObject = Instantiate(config.backgroundPrefab, config.backgroundPosition, config.backgroundRotation);
            bgObject.transform.localScale = config.backgroundScale;
            bgObject.name = "Background";
            backgroundRenderer = bgObject.GetComponent<SpriteRenderer>();
        }
        else if (backgroundRenderer != null)
        {
            if (config.backgroundSprite != null)
            {
                backgroundRenderer.sprite = config.backgroundSprite;
            }
            // Only apply color if it's not white (default), otherwise it tints the sprite too dark
            if (config.backgroundColor != Color.white && config.backgroundColor.a > 0)
            {
                backgroundRenderer.color = config.backgroundColor;
            }
            else
            {
                backgroundRenderer.color = Color.white;
            }
            
            // Apply transform settings to existing background
            backgroundRenderer.transform.position = config.backgroundPosition;
            backgroundRenderer.transform.rotation = config.backgroundRotation;
            backgroundRenderer.transform.localScale = config.backgroundScale;
        }
        
        // Update camera background color
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = config.skyColor;
        }

        // Update Player Position
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.transform.position = config.playerSpawnPosition;
        }
        
        // Apply difficulty settings
        ApplyDifficultySettings(config);
        
        // TODO: Spawn enemies based on config.enemySpawns
        
    }
    
    private void ApplyDifficultySettings(LevelConfig config)
    {
        // Update player stats based on difficulty
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            // Can modify starting resources based on difficulty
            // playerStats.maxAmmo = config.startingAmmo;
            // playerStats.ammo = config.startingAmmo;
        }
        
        // Update physics based on level modifiers
        if (config.hasGravity)
        {
            Physics2D.gravity = new Vector2(config.windForce, -9.81f);
        }
        else
        {
            Physics2D.gravity = new Vector2(config.windForce, 0f);
        }
    }
    
    public void StartNextLevel()
    {
        LevelConfig nextLevel = GetNextLevel();
        LoadLevel(nextLevel);
    }

    public void ResetProgression()
    {
        currentLevelIndex = 0;
    }
}
