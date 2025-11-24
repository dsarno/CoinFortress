using UnityEngine;
using UnityEditor;
using System.IO;

public class AssignCoinHoverSound
{
    [MenuItem("Tools/Assign Coin Hover Sound")]
    public static void AssignSound()
    {
        SoundDatabase database = AssetDatabase.LoadAssetAtPath<SoundDatabase>("Assets/Data/SoundDatabase.asset");
        if (database == null)
        {
            Debug.LogError("SoundDatabase not found at Assets/Data/SoundDatabase.asset");
            return;
        }
        
        // Try to find a suitable sound
        string[] guids = AssetDatabase.FindAssets("video_game_UI_button_#3", new[] { "Assets/Sound/UI" });
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            
            if (clip != null)
            {
                database.uiSounds.coinHover = clip;
                // Also ensure buttonHover is set if missing
                if (database.uiSounds.buttonHover == null)
                {
                    database.uiSounds.buttonHover = clip;
                }
                
                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();
                Debug.Log($"Assigned {clip.name} to coinHover in SoundDatabase");
            }
            else
            {
                Debug.LogError("Could not load audio clip");
            }
        }
        else
        {
            Debug.LogError("Could not find video_game_UI_button_#3 in Assets/Sound/UI");
        }
    }
}
