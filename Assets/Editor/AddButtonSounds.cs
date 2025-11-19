using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class AddButtonSounds : MonoBehaviour
{
    public static void Execute()
    {
        // Find all buttons in the Store Panel
        GameObject storePanel = GameObject.Find("Canvas/Store Root/Store Panel");
        if (storePanel == null)
        {
            Debug.LogError("Store Panel not found!");
            return;
        }
        
        Button[] buttons = storePanel.GetComponentsInChildren<Button>(true);
        int count = 0;
        
        foreach (Button button in buttons)
        {
            // Add event trigger for button click sound
            var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
            
            // Check if we already have a PointerClick entry
            bool hasClickSound = false;
            foreach (var entry in eventTrigger.triggers)
            {
                if (entry.eventID == UnityEngine.EventSystems.EventTriggerType.PointerClick)
                {
                    hasClickSound = true;
                    break;
                }
            }
            
            if (!hasClickSound)
            {
                // Add PointerClick trigger
                var clickEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                clickEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
                clickEntry.callback.AddListener((data) => { 
                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlayButtonClick();
                    }
                });
                eventTrigger.triggers.Add(clickEntry);
                count++;
                Debug.Log($"Added click sound to: {button.gameObject.name}");
            }
        }
        
        Debug.Log($"Added button click sounds to {count} buttons");
        EditorUtility.SetDirty(storePanel);
    }
}
