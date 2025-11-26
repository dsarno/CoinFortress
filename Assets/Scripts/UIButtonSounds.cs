using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Add this component to any button to automatically play click and hover sounds
/// </summary>
[RequireComponent(typeof(Button))]
public class UIButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only play hover sound if button is interactable
        if (button != null && button.interactable && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonHover();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // Only play click sound if button is interactable
        // AND if we haven't explicitly suppressed it (StoreManager handles its own click sounds for success/fail)
        if (button != null && button.interactable && SoundManager.Instance != null)
        {
            // Check if this button is handled by StoreManager logic which plays its own sounds
            // We can check if the button has a listener that calls StoreManager methods, but that's complex.
            // Instead, let's just check if this is one of the store buttons by name or tag, 
            // OR we can add a public flag to suppress default click.
            
            if (!suppressDefaultClickSound)
            {
                SoundManager.Instance.PlayButtonClick();
            }
        }
    }

    public bool suppressDefaultClickSound = false;
}
