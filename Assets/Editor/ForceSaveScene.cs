using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ForceSaveScene
{
    [MenuItem("Tools/Force Save Scene")]
    public static void Save()
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✓ Scene saved: {activeScene.name}");
    }
}
