using UnityEngine;
using UnityEditor;

public class RunUISetup : MonoBehaviour
{
    public static void Execute()
    {
        UISetupHelper helper = FindFirstObjectByType<UISetupHelper>();
        if (helper != null)
        {
            helper.SetupHUD();
            Debug.Log("Ran SetupHUD from helper.");
        }
        else
        {
            Debug.LogWarning("UISetupHelper not found in scene. Creating temporary one.");
            GameObject go = new GameObject("TempUISetup");
            helper = go.AddComponent<UISetupHelper>();
            helper.SetupHUD();
            DestroyImmediate(go);
        }
    }
}
