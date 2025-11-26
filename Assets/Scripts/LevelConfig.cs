using UnityEngine;

/// <summary>
/// ScriptableObject that defines all settings for a level
/// </summary>
[CreateAssetMenu(fileName = "Level_01", menuName = "Fortress/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";
    [TextArea(2, 4)]
    public string levelDescription = "Destroy the treasure chest to win!";
    
    [Header("Fortress")]
    public FortressLayout fortressLayout;
    public Vector2 fortressSpawnPosition = new Vector2(12, -11);
    public Vector3 playerSpawnPosition = new Vector3(-12, -3, 0);
    
    [Header("Visual")]
    public Sprite backgroundSprite;
    public GameObject backgroundPrefab;
    public Color backgroundColor = Color.white;
    public Color skyColor = new Color(0.4f, 0.3f, 0.5f);
    
    [Header("Background Transform")]
    public Vector3 backgroundPosition = Vector3.zero;
    public Vector3 backgroundScale = Vector3.one;
    public Quaternion backgroundRotation = Quaternion.identity;
    
    [Header("Difficulty")]
    [Range(1, 10)]
    public int difficultyLevel = 1;
    public int startingAmmo = 10;
    public float enemyFireRate = 2f; // Seconds between enemy shots
    public bool enableEnemyTurrets = false;
    
    [Header("Rewards")]
    public int coinsOnComplete = 50;
    public int bonusCoinsForPerfect = 25; // Extra coins if player takes no damage
    
    [Header("Enemy Configuration")]
    public EnemySpawnData[] enemySpawns;
    
    [Header("Level Modifiers")]
    public bool hasGravity = true;
    public float windForce = 0f; // Horizontal wind affecting projectiles
    public bool hasTimeLimit = true;
    public float timeLimitSeconds = 30f;
}

/// <summary>
/// Defines where enemies should spawn and their behavior
/// </summary>
[System.Serializable]
public class EnemySpawnData
{
    public enum EnemyType
    {
        StaticTurret,      // Fixed position turret
        MovingTurret,      // Turret that moves back and forth
        FloatingEnemy,     // Floating enemy that moves around
        BossTurret         // Powerful boss turret
    }
    
    public EnemyType enemyType = EnemyType.StaticTurret;
    public Vector2 spawnPosition;
    public float fireRate = 3f;
    public int health = 3;
    public int damage = 1;
    
    [Header("Movement (for moving enemies)")]
    public bool moves = false;
    public Vector2 moveStart;
    public Vector2 moveEnd;
    public float moveSpeed = 2f;
}
