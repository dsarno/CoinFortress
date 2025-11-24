using UnityEngine;
using System.Collections;

public class ForceCanvasActive : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"ForceCanvasActive: Awake on {gameObject.name}");
    }

    private void Start()
    {
        Debug.Log($"ForceCanvasActive: Start on {gameObject.name}");
        StartCoroutine(KeepActive());
    }

    private IEnumerator KeepActive()
    {
        // For the first 2 seconds, force this object and its children to be active
        // and force the canvas to render
        
        Canvas c = GetComponent<Canvas>();
        
        for (int i = 0; i < 60; i++) // ~1-2 seconds
        {
            if (!gameObject.activeSelf)
            {
                Debug.LogWarning("ForceCanvasActive: Object was inactive! Reactivating...");
                gameObject.SetActive(true);
            }
            
            if (c != null && !c.enabled)
            {
                 Debug.LogWarning("ForceCanvasActive: Canvas component was disabled! Re-enabling...");
                 c.enabled = true;
            }
            
            yield return null;
        }
        
        Debug.Log("ForceCanvasActive: Finished enforcement.");
    }
}

