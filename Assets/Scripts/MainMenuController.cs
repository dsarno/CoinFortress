using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button startGameButton;
    
    private void Start()
    {
        // Robustness check: Find references if missing
        if (mainMenuPanel == null)
        {
            Debug.LogWarning("MainMenuController: mainMenuPanel reference is missing! Attempting to find by name...");
            Transform canvasTransform = FindFirstObjectByType<Canvas>()?.transform;
            if (canvasTransform != null)
            {
                Transform panelTrans = canvasTransform.Find("Main Menu Panel");
                if (panelTrans != null) mainMenuPanel = panelTrans.gameObject;
            }
        }

        if (startGameButton == null && mainMenuPanel != null)
        {
            Debug.LogWarning("MainMenuController: startGameButton reference is missing! Searching in panel...");
            startGameButton = mainMenuPanel.GetComponentInChildren<Button>();
        }

        // Setup button listener
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }
        else
        {
            Debug.LogError("MainMenuController: Could not find Start Game Button!");
        }
        
        // Show main menu on start
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("MainMenuController: Could not find Main Menu Panel!");
        }
    }
    
    private void OnStartGameClicked()
    {
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
