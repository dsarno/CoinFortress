using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button startGameButton;
    
    private void Start()
    {
        // Setup button listener
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }
        
        // Show main menu on start
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }
    
    private void OnStartGameClicked()
    {
        Debug.Log("Start Game button clicked!");
        
        // Hide main menu
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        
        // Find and start level via LevelManager
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.StartFirstLevel();
        }
        else
        {
            Debug.LogError("LevelManager not found!");
        }
    }
}
