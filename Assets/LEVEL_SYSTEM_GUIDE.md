
# Level Configuration System

## Overview

The game now has a complete level configuration system using ScriptableObjects. This allows you to design levels with:
- Custom fortress layouts
- Background visuals
- Difficulty settings
- Enemy placements
- Reward amounts
- Special modifiers (wind, time limits, etc.)

---

## Core Components

### 1. LevelConfig (ScriptableObject)
**File:** `Assets/Scripts/LevelConfig.cs`

Stores all settings for a single level:
- **Level Info:** Number, name, description
- **Fortress:** Layout, spawn position
- **Visual:** Background sprite, colors
- **Difficulty:** Level rating, starting ammo, enemy fire rate
- **Rewards:** Coins on completion, bonus coins
- **Enemies:** Array of enemy spawn data
- **Modifiers:** Gravity, wind, time limits

### 2. LevelProgressionManager
**File:** `Assets/Scripts/LevelProgressionManager.cs`

Manages:
- Sequence of levels
- Loading level configurations
- Progressing to next level
- Optional random level selection
- Applying level settings to game

### 3. EnemySpawnData
**Part of:** `LevelConfig.cs`

Defines enemy placements:
- **Enemy Type:** Static turret, moving turret, floating enemy, boss
- **Position:** Where to spawn
- **Stats:** Fire rate, health, damage
- **Movement:** Move points and speed (for moving enemies)

---

## Creating Levels

### Quick Start (Using Menu)

1. **Tools → Create Level Configs → Create All Levels**
   - Creates 3 example levels instantly
   - Saved to `Assets/Levels/`

2. **Individual levels:**
   - Tools → Create Level Configs → Level 1 (Tutorial)
   - Tools → Create Level Configs → Level 2 (Challenge)
   - Tools → Create Level Configs → Level 3 (Boss)

### Manual Creation

1. **Right-click in Project**
2. **Create → Fortress → Level Config**
3. **Name it** (e.g., "Level_04_Chaos")
4. **Configure in Inspector:**

**Level Info:**
```
Level Number: 4
Level Name: "Chaos Castle"
Description: "Multiple enemy turrets and limited time!"
```

**Fortress:**
```
Fortress Layout: [Drag your FortressLayout asset]
Fortress Spawn Position: (12, -11)
```

**Visual:**
```
Background Sprite: [Optional custom background]
Background Color: (0.2, 0.2, 0.3, 1)
Sky Color: (0.4, 0.3, 0.5, 1)
```

**Difficulty:**
```
Difficulty Level: 6 (1-10 scale)
Starting Ammo: 10
Enemy Fire Rate: 1.5s (lower = faster)
Enable Enemy Turrets: ✓
```

**Rewards:**
```
Coins On Complete: 100
Bonus Coins For Perfect: 50
```

**Enemy Configuration:**
```
Enemy Spawns: (size 3)
  Element 0:
    Enemy Type: Static Turret
    Spawn Position: (10, 5)
    Fire Rate: 2
    Health: 3
    Damage: 1
  Element 1:
    Enemy Type: Moving Turret
    Spawn Position: (0, 8)
    Moves: ✓
    Move Start: (-5, 8)
    Move End: (5, 8)
    Move Speed: 2
    Fire Rate: 1.5
    Health: 5
    Damage: 1
  Element 2:
    Enemy Type: Boss Turret
    Spawn Position: (0, 12)
    Fire Rate: 1
    Health: 15
    Damage: 2
```

**Level Modifiers:**
```
Has Gravity: ✓
Wind Force: 0.5 (horizontal force on projectiles)
Has Time Limit: ✓
Time Limit Seconds: 120
```

---

## Setting Up Level Sequence

### Option 1: Using GameManager

1. **Select GameManager** in hierarchy
2. **Add Component → Level Progression Manager**
3. **Configure:**
   - **Level Sequence:**
     - Size: 3 (or however many levels)
     - Element 0: Level_01_Tutorial
     - Element 1: Level_02_Challenge
     - Element 2: Level_03_Boss
   - **Randomize Levels:** ☐ (sequential) or ✓ (random)
   - **References:**
     - Level Manager: [Drag from GameManager]
     - Fortress Spawner: [Drag from scene]
     - Background Renderer: [Drag Background object]
     - Main Camera: [Drag Main Camera]

### Option 2: Standalone GameObject

1. **Create empty GameObject** → Name: "Level System"
2. **Add Component → Level Progression Manager**
3. **Configure as above**

---

## How It Works

### Level Flow:

1. **Game Start:**
   - LevelProgressionManager loads first level config
   - Applies fortress layout, background, difficulty
   - Sets coin rewards

2. **Player Clicks "START LEVEL":**
   - LevelManager.BeginLevel() called
   - Fortress spawned with current layout
   - Enemies spawned (if implemented)
   - Level settings applied

3. **Player Destroys Treasure Chest:**
   - FortressBlock detects TreasureChest destroyed
   - Calls LevelManager.OnCoreDestroyed()
   - Awards coins
   - Waits ~3 seconds

4. **PrepareNextLevel():**
   - LevelProgressionManager.StartNextLevel()
   - Loads next config in sequence
   - Clears old fortress
   - Opens store for upgrades

5. **Repeat:**
   - Loop back to step 2

---

## Win Condition

**Treasure Chest is the primary win condition:**

```csharp
// In FortressBlock.cs
if (blockType == BlockType.TreasureChest || blockType == BlockType.Core)
{
    TriggerLevelComplete();
}
```

- Destroying the **Treasure Chest** completes the level
- Also works with **Core** block type (for backwards compatibility)

---

## Example Levels

### Level 1: Tutorial
- **Difficulty:** 1/10
- **Ammo:** 15 (generous)
- **Enemies:** None
- **Coins:** 50 (+25 bonus)
- **Goal:** Learn mechanics

### Level 2: Challenge
- **Difficulty:** 3/10
- **Ammo:** 10 (standard)
- **Enemies:** 2 static turrets
- **Coins:** 75 (+50 bonus)
- **Goal:** Deal with enemy fire

### Level 3: Boss
- **Difficulty:** 5/10
- **Ammo:** 12
- **Enemies:** 1 boss + 2 turrets
- **Coins:** 100 (+75 bonus)
- **Modifiers:** Wind (0.5), Time limit (180s)
- **Goal:** High pressure challenge

---

## Difficulty Scaling

**Difficulty Level (1-10):**
- **1-2:** Tutorial (no/few enemies, lots of ammo)
- **3-4:** Easy (basic enemies, standard ammo)
- **5-6:** Medium (multiple enemies, limited ammo)
- **7-8:** Hard (fast enemies, tough fortress, modifiers)
- **9-10:** Extreme (boss fights, severe limits)

**Difficulty affects:**
- Number of enemies
- Enemy stats (HP, damage, fire rate)
- Starting ammo
- Wind/gravity modifiers
- Time limits
- Coin rewards (higher difficulty = more coins)

---

## Enemy Types (For Future Implementation)

### Static Turret
- Fixed position
- Shoots at intervals
- Basic enemy

### Moving Turret
- Moves between two points
- Makes it harder to hit
- Continues firing while moving

### Floating Enemy
- Free-form movement
- Can patrol or track player
- More unpredictable

### Boss Turret
- High health
- High damage
- Fast fire rate
- Usually single, powerful enemy

---

## Level Modifiers

### Gravity
- `hasGravity: true` - Normal physics arc
- `hasGravity: false` - Straight shots

### Wind
- `windForce: 0` - No wind
- `windForce: 0.5` - Light breeze (slight push)
- `windForce: 1-2` - Strong wind (major effect)
- Negative values push left, positive push right

### Time Limit
- `hasTimeLimit: false` - No limit
- `hasTimeLimit: true` - Must complete in time
- `timeLimitSeconds: 120` - 2 minutes

---

## Tips for Level Design

### Fortress Layouts
- Start easy, build difficulty
- Place treasure chest strategically (not too easy, not impossible)
- Use windows for visual variety (weaker blocks)
- Roof blocks for protection
- Iron/Gold blocks for tough sections

### Enemy Placement
- Don't block all shots immediately
- Mix static and moving enemies
- Place bosses in strategic positions
- Consider player's shooting angle

### Difficulty Progression
- Level 1-3: Tutorial, learn mechanics
- Level 4-6: Introduce challenge
- Level 7-9: Test skill
- Level 10+: Expert territory

### Coin Rewards
- Base coins: 50 * difficulty level
- Bonus coins: 25 * difficulty level
- Keeps rewards meaningful as difficulty increases

---

## Integration Checklist

To fully integrate the level system:

- [x] LevelConfig ScriptableObject created
- [x] LevelProgressionManager created
- [x] Treasure chest triggers level complete
- [x] LevelManager updated for progression
- [ ] Create 3-5 level configs
- [ ] Add LevelProgressionManager to scene
- [ ] Assign level sequence
- [ ] Test level progression
- [ ] Implement enemy spawning (future)
- [ ] Add time limit UI (future)
- [ ] Add wind visual indicator (future)

---

## Quick Commands

| Menu Command | Action |
|--------------|--------|
| Tools → Create Level Configs → Create All Levels | Generate 3 example levels |
| Tools → Create Level Configs → Level 1 | Create tutorial level |
| Tools → Create Level Configs → Level 2 | Create challenge level |
| Tools → Create Level Configs → Level 3 | Create boss level |
| Right-click → Create → Fortress → Level Config | Manual level creation |

---

## File Organization

```
Assets/
├── Levels/                      # Level config files
│   ├── Level_01_Tutorial.asset
│   ├── Level_02_Challenge.asset
│   └── Level_03_Boss.asset
├── Scripts/
│   ├── LevelConfig.cs           # SO definition
│   ├── LevelProgressionManager.cs
│   └── FortressBlock.cs         # Updated with treasure chest win
└── Editor/
    └── CreateLevelConfigs.cs    # Quick creation tools
```

---

## Next Steps

1. **Create example levels:**
   ```
   Tools → Create Level Configs → Create All Levels
   ```

2. **Add to scene:**
   - Add LevelProgressionManager component
   - Assign level sequence
   - Connect references

3. **Test:**
   - Play game
   - Click Start Level
   - Destroy treasure chest
   - Verify level 2 loads

4. **Expand:**
   - Create more layouts with Fortress Editor
   - Design 10+ levels
   - Add enemy AI (future feature)
   - Implement time limit UI

---

**Your level system is ready! Create diverse, challenging levels with full control over every aspect of the gameplay.**
