# Sound System Guide

## Overview
The game now has a comprehensive sound management system with centralized sound storage and easy-to-use playback methods.

## Components

### 1. SoundDatabase (ScriptableObject)
**Location:** `Assets/Data/SoundDatabase.asset`

This is the central repository for all game sounds, organized into categories:

#### Music Tracks
- Level-specific background music
- Each track has a level number assignment
- Currently configured:
  - Level 1: "Fortress Break Backing Track"

#### Cannon Sounds
- **Player Cannon Firing:** 4 variations (`canon_shot_#1-4.wav`)
- **Enemy Firing:** Can be added separately for enemy turrets

#### Impact Sounds
- **Glass Impact:** For Window blocks
- **Stone Impact:** For Stone, Iron, Gold, Core, and Roof blocks
- Multiple variations can be added per impact type

#### UI Sounds
- Button clicks and hovers
- Purchase success/fail
- Level start/complete/game over
- Shield activate/break
- Ammo reload

### 2. SoundManager (Singleton)
**Location:** Attached to GameManager in the scene

Manages all sound playback with separate audio channels:
- **Music Source:** For background music (loops by default)
- **SFX Source:** For game sound effects
- **UI Source:** For UI feedback sounds

#### Volume Control
- Master Volume (affects all sounds)
- Music Volume
- SFX Volume
- UI Volume

All volumes can be adjusted independently.

## Usage

### Adding Sounds to the Database

1. Open `Assets/Data/SoundDatabase.asset` in the Inspector
2. Add sounds to the appropriate category:
   - **Music Tracks:** Add track name, level number, and AudioClip
   - **Cannon Sounds:** Add AudioClips to the list (random selection on fire)
   - **Impact Sounds:** Create entries with block types and sound clips
   - **UI Sounds:** Assign specific UI event sounds

### Playing Sounds in Code

```csharp
// Music
SoundManager.Instance.PlayLevelMusic(levelNumber);
SoundManager.Instance.PlayMusic(audioClip);
SoundManager.Instance.StopMusic();
SoundManager.Instance.FadeOutMusic(duration);

// Cannon & Combat SFX
SoundManager.Instance.PlayCannonFireSound();
SoundManager.Instance.PlayEnemyFireSound();
SoundManager.Instance.PlayImpactSound(blockType, position);

// Generic SFX
SoundManager.Instance.PlaySFX(audioClip);
SoundManager.Instance.PlaySFXAtPoint(audioClip, position);

// UI Sounds
SoundManager.Instance.PlayButtonClick();
SoundManager.Instance.PlayPurchaseSuccess();
SoundManager.Instance.PlayPurchaseFail();
SoundManager.Instance.PlayLevelStart();
SoundManager.Instance.PlayLevelComplete();
SoundManager.Instance.PlayShieldActivate();
SoundManager.Instance.PlayShieldBreak();

// Volume Control
SoundManager.Instance.SetMasterVolume(0.8f);
SoundManager.Instance.SetMusicVolume(0.7f);
SoundManager.Instance.SetSFXVolume(0.9f);
```

## Current Integrations

### Projectile Script
- Plays impact sounds when hitting blocks
- Sound varies based on block type (glass vs stone)

### PlayerCannonController
- Plays cannon firing sound for both normal and weak shots
- Random variation from 4 different firing sounds

### LevelManager
- Plays level start sound when beginning a level
- Starts appropriate music track for each level
- Plays level complete sound when core is destroyed

### StoreManager
- Purchase success sound for successful purchases
- Purchase fail sound when not enough coins
- Shield activate sound when unlocking shield

## Extending the System

### Adding New Sound Categories
1. Add new lists/fields to `SoundDatabase.cs`
2. Add corresponding playback methods to `SoundManager.cs`
3. Update the database asset in the Inspector

### Adding Block-Specific Impact Sounds
1. Open `SoundDatabase.asset`
2. Create a new `Impact Sound` entry
3. Assign the block types that should use this sound
4. Add one or more AudioClips (random selection)

### Adding Level-Specific Music
1. Open `SoundDatabase.asset`
2. Add a new entry to `Music Tracks`
3. Set the level number and assign the AudioClip
4. Music will automatically play when that level starts

### Adding Enemy Sounds
Enemy turrets can use `SoundManager.Instance.PlayEnemyFireSound()` instead of the player cannon sound for variety.

## Sound File Locations
- **Cannon Sounds:** `Assets/Sound/Canon/`
- **Music:** `Assets/Sound/Music/`
- **UI Sounds:** (To be added as needed)

## Important Notes for 2D Games
- **Non-Spatial Audio:** All sounds play as 2D (non-spatial) audio, meaning they're always audible regardless of camera distance
- This is intentional for 2D games where spatial positioning doesn't make sense
- If you want spatial audio for a 3D game, modify `PlaySFXAtPoint()` to use `AudioSource.PlayClipAtPoint()`

## Tips
- The SoundManager persists between scene loads (DontDestroyOnLoad)
- Multiple sounds in a category are randomly selected for variety
- All sounds respect the volume settings
- Volume settings could be saved to PlayerPrefs for persistence
- Button click sounds are triggered via EventTrigger components
