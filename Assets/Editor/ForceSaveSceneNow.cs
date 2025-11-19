using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ForceSaveSceneNow : MonoBehaviour
{
    public static void Execute()
    {
        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"Scene {scene.name} saved!");
    }
}
