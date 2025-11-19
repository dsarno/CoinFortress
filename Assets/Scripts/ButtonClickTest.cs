using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple test script to verify button clicks are working.
/// Attach this to any button to test if clicks are being registered.
/// </summary>
public class ButtonClickTest : MonoBehaviour
{
    private Button button;
    
    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }
    
    private void OnClick()
    {
        Debug.Log($"★★★ BUTTON CLICKED: {gameObject.name} ★★★");
    }
}
