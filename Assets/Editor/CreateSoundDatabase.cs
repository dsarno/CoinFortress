using UnityEngine;
using UnityEditor;

public class CreateSoundDatabase : MonoBehaviour
{
    public static void Execute()
    {
        // Create the SoundDatabase asset
        SoundDatabase soundDB = ScriptableObject.CreateInstance<SoundDatabase>();
        
        // Assign cannon firing sounds
        soundDB.cannonFiringSounds.Add(AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Canon/canon_shot_#1.wav"));
        soundDB.cannonFiringSounds.Add(AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Canon/canon_shot_#2.wav"));
        soundDB.cannonFiringSounds.Add(AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Canon/canon_shot_#3.wav"));
        soundDB.cannonFiringSounds.Add(AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Canon/canon_shot_#4.wav"));
        
        // Create impact sounds
        ImpactSound glassImpact = new ImpactSound
        {
            soundName = "Glass Impact",
            blockTypes = { BlockType.Window },
            sounds = { AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Canon/canon-impact-glass.wav") }
        };
        soundDB.impactSounds.Add(glassImpact);
        
        ImpactSound stoneImpact = new ImpactSound
        {
            soundName = "Stone Impact",
            blockTypes = { BlockType.Stone, BlockType.Iron, BlockType.Gold, BlockType.Diamond, BlockType.Silver, BlockType.Roof },
            sounds = { AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Canon/canon-impact-stone.wav") }
        };
        soundDB.impactSounds.Add(stoneImpact);
        
        // Create music track
        MusicTrack defaultTrack = new MusicTrack
        {
            trackName = "Fortress Break",
            levelNumber = 1,
            musicClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Music/Fortress_Break_Backing_Track.wav")
        };
        soundDB.musicTracks.Add(defaultTrack);
        
        // Initialize UI sounds structure
        soundDB.uiSounds = new UISound();
        
        // Save the asset
        AssetDatabase.CreateAsset(soundDB, "Assets/Data/SoundDatabase.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("SoundDatabase created at Assets/Data/SoundDatabase.asset");
    }
}
