using UnityEngine;
using UnityEditor;

public class AssignWinSounds : MonoBehaviour
{
    public static void Assign()
    {
        SoundDatabase db = AssetDatabase.LoadAssetAtPath<SoundDatabase>("Assets/Data/SoundDatabase.asset");
        if (db != null)
        {
            db.uiSounds.cheerLevelWin = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Level sounds/cheer-level-win.wav");
            db.uiSounds.levelSuccess = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sound/Level sounds/level-success.wav");
            EditorUtility.SetDirty(db);
            Debug.Log("Assigned win sounds to database.");
        }
        else
        {
            Debug.LogError("SoundDatabase not found!");
        }
    }
}
