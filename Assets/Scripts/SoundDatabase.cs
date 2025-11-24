using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Game/Sound Database")]
public class SoundDatabase : ScriptableObject
{
    [Header("Music Tracks")]
    public List<MusicTrack> musicTracks = new List<MusicTrack>();
    
    [Header("Cannon Sounds")]
    public List<AudioClip> cannonFiringSounds = new List<AudioClip>();
    public List<AudioClip> enemyFiringSounds = new List<AudioClip>();
    
    [Header("Impact Sounds")]
    public List<ImpactSound> impactSounds = new List<ImpactSound>();
    
    [Header("UI Sounds")]
    public UISound uiSounds;
    
    /// <summary>
    /// Get a random cannon firing sound
    /// </summary>
    public AudioClip GetRandomCannonFireSound()
    {
        if (cannonFiringSounds.Count == 0) return null;
        return cannonFiringSounds[UnityEngine.Random.Range(0, cannonFiringSounds.Count)];
    }
    
    /// <summary>
    /// Get a random enemy firing sound
    /// </summary>
    public AudioClip GetRandomEnemyFireSound()
    {
        if (enemyFiringSounds.Count == 0) return null;
        return enemyFiringSounds[UnityEngine.Random.Range(0, enemyFiringSounds.Count)];
    }
    
    /// <summary>
    /// Get impact sound by block type
    /// </summary>
    public AudioClip GetImpactSound(BlockType blockType)
    {
        foreach (var impactSound in impactSounds)
        {
            if (impactSound.blockTypes.Contains(blockType))
            {
                if (impactSound.sounds.Count == 0) continue;
                return impactSound.sounds[UnityEngine.Random.Range(0, impactSound.sounds.Count)];
            }
        }
        return null;
    }
    
    /// <summary>
    /// Get music track by level number
    /// </summary>
    public AudioClip GetMusicForLevel(int levelNumber)
    {
        foreach (var track in musicTracks)
        {
            if (track.levelNumber == levelNumber)
                return track.musicClip;
        }
        // Return default track if available
        if (musicTracks.Count > 0)
            return musicTracks[0].musicClip;
        return null;
    }
}

[System.Serializable]
public class MusicTrack
{
    public string trackName;
    public int levelNumber;
    public AudioClip musicClip;
}

[System.Serializable]
public class ImpactSound
{
    public string soundName;
    public List<BlockType> blockTypes = new List<BlockType>();
    public List<AudioClip> sounds = new List<AudioClip>();
}

[System.Serializable]
public class UISound
{
    [Header("Button Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    public AudioClip coinHover;
    
    [Header("Purchase Sounds")]
    public AudioClip purchaseSuccess;
    public AudioClip purchaseFail;
    
    [Header("Game State Sounds")]
    public AudioClip levelStart;
    public AudioClip levelComplete;
    public AudioClip gameOver;
    
    [Header("Powerup Sounds")]
    public AudioClip shieldActivate;
    public AudioClip shieldBreak;
    public AudioClip ammoReload;
}
