using UnityEngine;
using UnityEditor;

public class CreateLevelConfigs
{
    [MenuItem("Tools/Create Level Configs/Level 1 (Tutorial)")]
    public static void CreateLevel1()
    {
        LevelConfig level = ScriptableObject.CreateInstance<LevelConfig>();
        
        level.levelNumber = 1;
        level.levelName = "Tutorial Fortress";
        level.levelDescription = "Destroy the treasure chest to win! Use limited ammo wisely.";
        
        // Use existing layout
        level.fortressLayout = AssetDatabase.LoadAssetAtPath<FortressLayout>("Assets/BricksAndWindows.asset");
        level.fortressSpawnPosition = new Vector2(12, -11);
        
        level.difficultyLevel = 1;
        level.startingAmmo = 15; // Extra ammo for tutorial
        level.coinsOnComplete = 50;
        level.bonusCoinsForPerfect = 25;
        
        level.enableEnemyTurrets = false; // No enemies in tutorial
        level.enemyFireRate = 3f;
        
        level.hasGravity = true;
        level.windForce = 0f;
        level.hasTimeLimit = false;
        
        AssetDatabase.CreateAsset(level, "Assets/Levels/Level_01_Tutorial.asset");
        AssetDatabase.SaveAssets();
        
        Debug.Log("Created Level 1 config at Assets/Levels/Level_01_Tutorial.asset");
    }
    
    [MenuItem("Tools/Create Level Configs/Level 2 (Challenge)")]
    public static void CreateLevel2()
    {
        LevelConfig level = ScriptableObject.CreateInstance<LevelConfig>();
        
        level.levelNumber = 2;
        level.levelName = "Fortified Castle";
        level.levelDescription = "The fortress is stronger. Watch out for enemy turrets!";
        
        level.fortressLayout = AssetDatabase.LoadAssetAtPath<FortressLayout>("Assets/BricksAndWindows.asset");
        level.fortressSpawnPosition = new Vector2(12, -11);
        
        level.difficultyLevel = 3;
        level.startingAmmo = 10;
        level.coinsOnComplete = 75;
        level.bonusCoinsForPerfect = 50;
        
        level.enableEnemyTurrets = true;
        level.enemyFireRate = 2f; // Faster enemy fire
        
        // Add enemy turrets
        level.enemySpawns = new EnemySpawnData[2];
        level.enemySpawns[0] = new EnemySpawnData
        {
            enemyType = EnemySpawnData.EnemyType.StaticTurret,
            spawnPosition = new Vector2(10, 0),
            fireRate = 2f,
            health = 3,
            damage = 1
        };
        level.enemySpawns[1] = new EnemySpawnData
        {
            enemyType = EnemySpawnData.EnemyType.StaticTurret,
            spawnPosition = new Vector2(-5, 5),
            fireRate = 2.5f,
            health = 3,
            damage = 1
        };
        
        level.hasGravity = true;
        level.windForce = 0f;
        level.hasTimeLimit = false;
        
        AssetDatabase.CreateAsset(level, "Assets/Levels/Level_02_Challenge.asset");
        AssetDatabase.SaveAssets();
        
        Debug.Log("Created Level 2 config at Assets/Levels/Level_02_Challenge.asset");
    }
    
    [MenuItem("Tools/Create Level Configs/Level 3 (Boss)")]
    public static void CreateLevel3()
    {
        LevelConfig level = ScriptableObject.CreateInstance<LevelConfig>();
        
        level.levelNumber = 3;
        level.levelName = "Dragon's Keep";
        level.levelDescription = "Face the mighty fortress with a boss turret!";
        
        level.fortressLayout = AssetDatabase.LoadAssetAtPath<FortressLayout>("Assets/BricksAndWindows.asset");
        level.fortressSpawnPosition = new Vector2(12, -11);
        
        level.difficultyLevel = 5;
        level.startingAmmo = 12;
        level.coinsOnComplete = 100;
        level.bonusCoinsForPerfect = 75;
        
        level.enableEnemyTurrets = true;
        level.enemyFireRate = 1.5f; // Very fast enemy fire
        
        // Add boss and minions
        level.enemySpawns = new EnemySpawnData[3];
        level.enemySpawns[0] = new EnemySpawnData
        {
            enemyType = EnemySpawnData.EnemyType.BossTurret,
            spawnPosition = new Vector2(0, 10),
            fireRate = 1f,
            health = 10,
            damage = 2
        };
        level.enemySpawns[1] = new EnemySpawnData
        {
            enemyType = EnemySpawnData.EnemyType.StaticTurret,
            spawnPosition = new Vector2(8, 3),
            fireRate = 2f,
            health = 3,
            damage = 1
        };
        level.enemySpawns[2] = new EnemySpawnData
        {
            enemyType = EnemySpawnData.EnemyType.StaticTurret,
            spawnPosition = new Vector2(-8, 3),
            fireRate = 2f,
            health = 3,
            damage = 1
        };
        
        level.hasGravity = true;
        level.windForce = 1f; // Wind adds challenge
        level.hasTimeLimit = true;
        level.timeLimitSeconds = 180f; // 3 minutes
        
        AssetDatabase.CreateAsset(level, "Assets/Levels/Level_03_Boss.asset");
        AssetDatabase.SaveAssets();
        
        Debug.Log("Created Level 3 config at Assets/Levels/Level_03_Boss.asset");
    }
    
    [MenuItem("Tools/Create Level Configs/Create All Levels")]
    public static void CreateAllLevels()
    {
        // Create Levels folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Levels"))
        {
            AssetDatabase.CreateFolder("Assets", "Levels");
        }
        
        CreateLevel1();
        CreateLevel2();
        CreateLevel3();
        
        EditorUtility.DisplayDialog("Levels Created", 
            "Created 3 level configs:\n" +
            "- Level 1: Tutorial\n" +
            "- Level 2: Challenge\n" +
            "- Level 3: Boss\n\n" +
            "Find them in Assets/Levels/", 
            "OK");
    }
}
