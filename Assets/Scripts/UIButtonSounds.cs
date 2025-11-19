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
        if (button != null && button.interactable && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
    }
}
