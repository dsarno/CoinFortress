# Fortress Game - Ammo & Upgrade System Setup Guide

## Quick Setup (5 minutes)

### 1. Create Level Manager GameObject

1. In your SampleScene hierarchy, create a new empty GameObject
2. Name it "GameManager"
3. Add the following components:
   - `LevelManager`
   - `UISetupHelper`

### 2. Setup References

In the Inspector for GameManager:

**UISetupHelper:**
- Drag the `Player` GameObject to `Player Stats` field
- Drag the `FortressSpawner` GameObject to the Level Manager's `Fortress Spawner` field

**LevelManager:**
- The script will auto-find references, but you can manually set:
  - Player Stats (from Player GameObject)
  - Fortress Spawner (from FortressSpawner GameObject)
  - Store Manager (will be created automatically)

### 3. Auto-Generate UI

1. Select the GameManager GameObject
2. In the Inspector, find the `UISetupHelper` component
3. Right-click on the component header → **"Setup Complete UI"**
   
   OR use the individual options:
   - "Setup HUD Only" - Creates the in-game ammo/health display
   - "Setup Store UI Only" - Creates the upgrade shop

4. The script will automatically create:
   - Canvas with EventSystem
   - HUD (top-left: ammo, health, shield | top-right: coins, ammo tier)
   - Store Panel (centered, with all upgrade buttons)

### 4. Create Shield GameObject (Optional - if you want visual shield)

1. Under the `Player` GameObject, create a new GameObject
2. Name it "Shield"
3. Add components:
   - `Sprite Renderer` (assign a shield sprite or use a simple colored square)
   - `Box Collider 2D` (set as trigger)
   - `Shield` script
4. Position it slightly in front of the cannon (e.g., x: 0.5, y: 0)
5. Set the layer or use tags to prevent friendly fire

### 5. Configure Player

Make sure the `Player` GameObject has:
- `PlayerStats` component (should already be there)
- `PlayerCannonController` (should already be there)

The PlayerStats will now have these fields visible in Inspector:
- **Combat:** damage, fireCooldown
- **Ammo:** ammo (10), maxAmmo (10), ammoTier (0)
- **Weak Fallback Shot:** weakShotDamage (1), weakShotCooldown (2)
- **Health:** maxHP, currentHP
- **Shield:** shieldUnlocked (false), shieldMaxHP (5), shieldCurrentHP (0)
- **Economy:** coins (100 for testing)

---

## How It Works

### Core Loop

1. **Store Opens** at start
   - Spend coins on upgrades
   - Click "START LEVEL" when ready

2. **Level Plays**
   - Limited ammo (shown in HUD)
   - Fire at fortress with mouse aim + left click
   - When out of ammo → weak fallback shots (slower, less damage)
   - Shield blocks enemy fire (if unlocked)

3. **Core Destroyed**
   - Earn coins (currently 50 per level)
   - Store opens again for next level

### Ammo System

- **Standard (Tier 0):** Base damage
- **Heavy (Tier 1):** +50% damage
- **Explosive (Tier 2):** +50% damage + splash damage to nearby blocks

Ammo is consumed on each shot. When ammo reaches 0, you can still fire weak shots to avoid soft-lock.

### Upgrades Available

**Ammo Section:**
- Buy Ammo Pack: +5 max ammo capacity (10 coins)
- Upgrade Ammo Tier: Better shells (50+ coins, increases per tier)

**Cannon Section:**
- Upgrade Damage: +1 damage per level (cost scales: 20, 40, 60...)
- Upgrade Fire Rate: Faster shooting (cost scales: 25, 50, 75...)

**Shield Section:**
- Unlock Shield: One-time purchase (100 coins)
- Upgrade Shield HP: +3 max shield HP (30 coins)
- Repair Shield: Restore to full HP (15 coins)

---

## Testing the System

1. **Play the scene**
2. Store should open immediately
3. You start with 100 coins (for testing)
4. Try buying upgrades
5. Click "START LEVEL"
6. Fortress should spawn
7. Fire at the fortress (watch ammo count decrease)
8. Destroy the core to complete level
9. Store opens again with earned coins

---

## Customization

### Adjust Costs
Edit these in the StoreManager component:
- ammoPackCost, ammoTierBaseCost
- damageUpgradeCost, fireRateUpgradeCost
- shieldUnlockCost, shieldHPUpgradeCost, shieldRepairCost

### Adjust Starting Stats
Edit PlayerStats component:
- coins (start with more/less for testing)
- ammo, maxAmmo (starting ammo capacity)
- damage (starting damage)
- fireCooldown (starting fire rate)

### Adjust Rewards
Edit LevelManager component:
- coinsPerLevel (how many coins earned per level)

---

## Troubleshooting

**"UI not showing"**
- Make sure Canvas is in ScreenSpace-Overlay mode
- Check that EventSystem exists in scene

**"Store buttons don't work"**
- Verify StoreManager references are set
- Check that EventSystem is in the scene

**"Ammo not decreasing"**
- Make sure PlayerStats is on the Player GameObject
- Check that PlayerCannonController is referencing the correct PlayerStats

**"Shield not working"**
- Ensure Shield GameObject has Shield.cs script
- Check collider is set to trigger
- Make sure enemy projectiles have the tag "EnemyProjectile"

**"Core doesn't trigger level end"**
- Verify LevelManager exists in scene
- Check that the core block has BlockType.Core

---

## Next Steps

1. Add coin pickup objects with fountain VFX (currently coins are awarded directly)
2. Create enemy turrets that fire back at player
3. Add sound effects for shooting, impacts, purchases
4. Add particle effects for explosions (especially Tier 2 ammo)
5. Create different fortress layouts for higher levels
6. Add difficulty scaling (stronger fortresses at higher levels)
7. Add sprite variations for different ammo tiers
8. Polish the UI with better graphics and animations

---

## File Reference

**New Scripts:**
- `Assets/Scripts/Shield.cs` - Shield behavior and collision
- `Assets/Scripts/StoreManager.cs` - Upgrade shop UI and logic
- `Assets/Scripts/LevelManager.cs` - Level flow and coin rewards
- `Assets/Scripts/GameHUD.cs` - In-game stat display
- `Assets/Scripts/UISetupHelper.cs` - Automatic UI generation

**Modified Scripts:**
- `Assets/Scripts/PlayerStats.cs` - Added ammo, shield, upgrades
- `Assets/Scripts/PlayerCannonController.cs` - Ammo consumption, weak shots
- `Assets/Scripts/Projectile.cs` - Ammo tiers, splash damage
- `Assets/Scripts/FortressBlock.cs` - LevelManager integration
