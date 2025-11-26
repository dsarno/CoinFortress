using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Sound Database")]
    public SoundDatabase soundDatabase;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float uiVolume = 0.6f;

    [Header("Pitch Settings")]
    [Range(0.1f, 2f)] public float minPitch = 0.8f;
    [Range(0.1f, 2f)] public float maxPitch = 1.2f;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Setup audio sources if not assigned
        SetupAudioSources();
    }
    
    private void SetupAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        
        if (uiSource == null)
        {
            uiSource = gameObject.AddComponent<AudioSource>();
            uiSource.playOnAwake = false;
        }
        
        UpdateVolumes();
    }
    
    private void UpdateVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
        if (uiSource != null)
            uiSource.volume = uiVolume * masterVolume;
    }
    
    #region Music
    
    /// <summary>
    /// Play music track for a specific level
    /// </summary>
    public void PlayLevelMusic(int levelNumber)
    {
        if (soundDatabase == null) return;
        
        AudioClip musicClip = soundDatabase.GetMusicForLevel(levelNumber);
        if (musicClip != null)
        {
            PlayMusic(musicClip);
        }
    }
    
    /// <summary>
    /// Play a specific music clip
    /// </summary>
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;
        
        // Stop any active fade out
        StopAllCoroutines();
        musicSource.volume = musicVolume * masterVolume;
        
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;
        
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlayIntroMusic()
    {
        if (soundDatabase != null && soundDatabase.uiSounds != null && soundDatabase.uiSounds.introMusic != null)
        {
            PlayMusic(soundDatabase.uiSounds.introMusic);
        }
    }
    
    /// <summary>
    /// Stop music
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    
    /// <summary>
    /// Fade out music
    /// </summary>
    public void FadeOutMusic(float duration = 1f)
    {
        StartCoroutine(FadeOutMusicCoroutine(duration));
    }
    
    private IEnumerator FadeOutMusicCoroutine(float duration)
    {
        if (musicSource == null) yield break;
        
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }
        
        musicSource.Stop();
        musicSource.volume = startVolume;
    }
    
    #endregion
    
    #region SFX
    
    /// <summary>
    /// Play a cannon firing sound
    /// </summary>
    public void PlayCannonFireSound(float volumeMultiplier = 1f)
    {
        if (soundDatabase == null) return;
        
        AudioClip clip = soundDatabase.GetRandomCannonFireSound();
        if (clip != null)
        {
            PlaySFX(clip, volumeMultiplier, true);
        }
    }
    
    /// <summary>
    /// Play an enemy firing sound
    /// </summary>
    public void PlayEnemyFireSound()
    {
        if (soundDatabase == null) return;
        AudioClip clip = soundDatabase.GetRandomEnemyFireSound();
        PlaySFX(clip);
    }
    
    /// <summary>
    /// Play impact sound based on block type
    /// </summary>
    public void PlayImpactSound(BlockType blockType, Vector3 position)
    {
        if (soundDatabase == null) return;
        
        AudioClip clip = soundDatabase.GetImpactSound(blockType);
        if (clip != null)
        {
            PlaySFXAtPoint(clip, position, 1f, true);
        }
    }
    
    /// <summary>
    /// Play a generic SFX clip
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f, bool varyPitch = false)
    {
        if (sfxSource == null || clip == null) return;
        
        if (varyPitch)
        {
            sfxSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            sfxSource.pitch = 1f;
        }
        
        sfxSource.PlayOneShot(clip, volumeMultiplier);
        
        // Reset pitch after a short delay or just leave it? 
        // PlayOneShot uses the source's current pitch. 
        // If we change it back immediately, it might affect the playing sound? 
        // No, PlayOneShot fires and forgets, but it uses the source's settings at the moment of firing.
        // However, changing pitch on the source affects ALL sounds playing on that source.
        // Ideally we'd use a pool of audio sources, but for now we'll just reset it next frame or keep it varied.
        // Let's reset it to 1.0 in Update or just leave it random since it's the main SFX channel.
    }
    
    /// <summary>
    /// Play SFX at a specific world position (or just play it non-spatially for 2D games)
    /// </summary>
    public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volumeMultiplier = 1f, bool varyPitch = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: Tried to play null audio clip at point");
            return;
        }
        
        // For 2D games, just play the sound non-spatially using the main SFX source
        // This ensures sounds are always audible regardless of camera distance
        PlaySFX(clip, volumeMultiplier, varyPitch);
    }
    
    #endregion
    
    #region UI Sounds
    
    public void PlayButtonClick()
    {
        // Stop any playing UI sounds first (like hover) so click is immediate
        if (uiSource != null)
        {
            uiSource.Stop();
        }
        
        if (soundDatabase?.uiSounds?.buttonClick != null)
            PlayUISound(soundDatabase.uiSounds.buttonClick);
    }
    
    public void PlayButtonHover()
    {
        if (soundDatabase?.uiSounds?.buttonHover != null)
            PlayUISound(soundDatabase.uiSounds.buttonHover);
    }
    
    public void PlayPurchaseSuccess()
    {
        if (soundDatabase?.uiSounds?.purchaseSuccess != null)
            PlayUISound(soundDatabase.uiSounds.purchaseSuccess);
    }

    public void PlayCoinHover()
    {
        // Use a higher pitch for coin hover to distinguish it
        if (uiSource != null)
        {
            uiSource.pitch = 1.5f;
            // Prefer dedicated coin hover sound, fallback to button hover
            AudioClip clip = soundDatabase?.uiSounds?.coinHover;
            if (clip == null) clip = soundDatabase?.uiSounds?.buttonHover;
            
            if (clip != null)
                uiSource.PlayOneShot(clip, 0.5f);
            
            StartCoroutine(ResetUIPitch());
        }
    }

    private IEnumerator ResetUIPitch()
    {
        yield return new WaitForSeconds(0.1f);
        if (uiSource != null) uiSource.pitch = 1.0f;
    }
    
    public void PlayPurchaseFail()
    {
        if (soundDatabase?.uiSounds?.purchaseFail != null)
            PlayUISound(soundDatabase.uiSounds.purchaseFail);
    }
    
    public void PlayLevelStart()
    {
        if (soundDatabase?.uiSounds?.levelStart != null)
            PlayUISound(soundDatabase.uiSounds.levelStart);
    }
    
    public void PlayLevelComplete()
    {
        if (soundDatabase?.uiSounds?.levelComplete != null)
            PlayUISound(soundDatabase.uiSounds.levelComplete);
            
        // Play additional win sounds if available
        if (soundDatabase?.uiSounds?.cheerLevelWin != null)
            PlaySFX(soundDatabase.uiSounds.cheerLevelWin, 1f, false);
            
        if (soundDatabase?.uiSounds?.levelSuccess != null)
            PlaySFX(soundDatabase.uiSounds.levelSuccess, 1f, false);
    }

    public void PlayGameWin()
    {
        if (soundDatabase?.uiSounds?.victoryHorns != null)
            PlaySFX(soundDatabase.uiSounds.victoryHorns, 1f, false);
    }
    
    public void PlayGameOver()
    {
        if (soundDatabase?.uiSounds?.gameOver != null)
            PlayUISound(soundDatabase.uiSounds.gameOver);
    }

    public void PlayClockTick()
    {
        if (soundDatabase?.uiSounds?.clockTick != null)
            PlaySFX(soundDatabase.uiSounds.clockTick, 1f, false);
    }

    public void PlayBooing()
    {
        if (soundDatabase?.uiSounds?.booing != null)
            PlaySFX(soundDatabase.uiSounds.booing, 1f, false);
    }
    
    public void PlayShieldActivate()
    {
        if (soundDatabase?.uiSounds?.shieldActivate != null)
            PlayUISound(soundDatabase.uiSounds.shieldActivate);
    }
    
    public void PlayShieldBreak()
    {
        if (soundDatabase?.uiSounds?.shieldBreak != null)
            PlayUISound(soundDatabase.uiSounds.shieldBreak);
    }
    
    public void PlayAmmoReload()
    {
        if (soundDatabase?.uiSounds?.ammoReload != null)
            PlayUISound(soundDatabase.uiSounds.ammoReload);
    }
    
    private void PlayUISound(AudioClip clip)
    {
        if (uiSource == null || clip == null) return;
        
        // Use Play instead of PlayOneShot for immediate playback
        // This ensures the sound starts immediately without waiting
        uiSource.clip = clip;
        uiSource.Play();
    }
    
    #endregion
    
    #region Volume Control
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    #endregion
}
