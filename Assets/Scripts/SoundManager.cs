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
        
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;
        
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
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
    public void PlayCannonFireSound()
    {
        if (soundDatabase == null) return;
        
        AudioClip clip = soundDatabase.GetRandomCannonFireSound();
        if (clip != null)
        {
            PlaySFX(clip);
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
            PlaySFXAtPoint(clip, position);
        }
    }
    
    /// <summary>
    /// Play a generic SFX clip
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volumeMultiplier);
    }
    
    /// <summary>
    /// Play SFX at a specific world position (or just play it non-spatially for 2D games)
    /// </summary>
    public void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: Tried to play null audio clip at point");
            return;
        }
        
        // For 2D games, just play the sound non-spatially using the main SFX source
        // This ensures sounds are always audible regardless of camera distance
        PlaySFX(clip, volumeMultiplier);
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
    }
    
    public void PlayGameOver()
    {
        if (soundDatabase?.uiSounds?.gameOver != null)
            PlayUISound(soundDatabase.uiSounds.gameOver);
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
