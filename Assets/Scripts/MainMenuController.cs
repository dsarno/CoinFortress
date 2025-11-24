using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button startGameButton;
    
    private void Awake()
    {
        Debug.Log($"MainMenuController: Awake called on {gameObject.name}. ActiveSelf: {gameObject.activeSelf}");
    }

    private void OnEnable()
    {
        Debug.Log($"MainMenuController: OnEnable called on {gameObject.name}");
    }

    private void Start()
    {
        Debug.Log("MainMenuController: Start called");

        // Robustness check: Find references if missing
        if (mainMenuPanel == null)
        {
            Debug.LogWarning("MainMenuController: mainMenuPanel reference is missing! Attempting to find by name...");
            // Try to find by name assuming standard hierarchy
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
            startGameButton.onClick.RemoveAllListeners(); // Prevent double subscription
            startGameButton.onClick.AddListener(OnStartGameClicked);
            Debug.Log($"MainMenuController: Subscribed to button {startGameButton.name}");
        }
        else
        {
            Debug.LogError("MainMenuController: Could not find Start Game Button!");
        }
        
        // Show main menu on start
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            Debug.Log("MainMenuController: Set Main Menu Panel to Active");
        }
        else
        {
            Debug.LogError("MainMenuController: Could not find Main Menu Panel!");
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
