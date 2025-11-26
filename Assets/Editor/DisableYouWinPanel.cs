using UnityEngine;
using UnityEditor;

public class DisableYouWinPanel
{
    public static void Execute()
    {
        GameObject panel = GameObject.Find("You Win Panel");
        if (panel != null)
        {
            panel.SetActive(false);
            Debug.Log("Disabled You Win Panel");
        }
        else
        {
            Debug.LogError("Could not find You Win Panel");
        }
    }
}
