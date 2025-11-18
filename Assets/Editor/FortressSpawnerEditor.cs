using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FortressSpawner))]
public class FortressSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        FortressSpawner spawner = (FortressSpawner)target;
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Spawn Fortress", GUILayout.Height(30)))
        {
            spawner.SpawnFortress();
        }
        
        if (GUILayout.Button("Clear Fortress", GUILayout.Height(30)))
        {
            spawner.ClearFortress();
        }
    }
}