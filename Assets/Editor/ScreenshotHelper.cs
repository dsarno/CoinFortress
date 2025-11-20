using UnityEngine;
using UnityEditor;

public class ScreenshotHelper : MonoBehaviour
{
    public static void EnableStore()
    {
        GameObject store = GameObject.Find("Canvas/Store Root");
        if (store != null)
        {
            store.SetActive(true);
            // Disable Main Menu to see Store clearly
            GameObject menu = GameObject.Find("Canvas/Main Menu Panel");
            if (menu != null) menu.SetActive(false);
        }
    }
}
