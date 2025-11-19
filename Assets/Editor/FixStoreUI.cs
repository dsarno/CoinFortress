using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixStoreUI
{
    [MenuItem("Tools/Fix Store UI Input")]
    public static void FixStore()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found!");
            return;
        }
        
        // Find the store panel
        Transform storePanelTransform = canvas.transform.Find("Store Panel");
        if (storePanelTransform == null)
        {
            Debug.LogError("Store Panel not found!");
            return;
        }
        
        GameObject storePanel = storePanelTransform.gameObject;
        
        // Make sure store panel blocks raycasts
        Image storeBg = storePanel.GetComponent<Image>();
        if (storeBg != null)
        {
            storeBg.raycastTarget = true;
        }
        
        // Check if blocker already exists
        Transform existingBlocker = canvas.transform.Find("Store Root/Store Blocker");
        if (existingBlocker == null)
        {
            // Create store root to group blocker and panel
            GameObject storeRoot = new GameObject("Store Root");
            storeRoot.transform.SetParent(canvas.transform, false);
            RectTransform rootRect = storeRoot.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;
            rootRect.anchoredPosition = Vector2.zero;
            
            // Create fullscreen blocker
            GameObject blocker = new GameObject("Store Blocker");
            blocker.transform.SetParent(storeRoot.transform, false);
            RectTransform blockerRect = blocker.AddComponent<RectTransform>();
            blockerRect.anchorMin = Vector2.zero;
            blockerRect.anchorMax = Vector2.one;
            blockerRect.sizeDelta = Vector2.zero;
            blockerRect.anchoredPosition = Vector2.zero;
            Image blockerImage = blocker.AddComponent<Image>();
            blockerImage.color = new Color(0, 0, 0, 0.5f);
            blockerImage.raycastTarget = true;
            
            // Move store panel under root
            storePanel.transform.SetParent(storeRoot.transform, false);
            
            // Update StoreManager reference
            StoreManager storeManager = Object.FindFirstObjectByType<StoreManager>();
            if (storeManager != null)
            {
                storeManager.storePanel = storeRoot;
                EditorUtility.SetDirty(storeManager);
            }
            
            // Make sure store root is after HUD in hierarchy so it renders on top
            storeRoot.transform.SetAsLastSibling();
            
            Debug.Log("Store UI fixed! Added blocker to prevent click-through.");
        }
        else
        {
            Debug.Log("Store blocker already exists!");
        }
        
        // Save changes
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
