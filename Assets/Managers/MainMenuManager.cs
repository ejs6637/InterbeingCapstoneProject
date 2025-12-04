using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas mainMenuCanvas;
    public Canvas settingsCanvas;
    public Canvas extrasCanvas; // NEW: Extras Menu canvas

    private void Start()
    {
        // Safety check
        if (mainMenuCanvas == null || settingsCanvas == null || extrasCanvas == null)
        {
            Debug.LogError("MenuManager: Canvas references not assigned!");
            return;
        }

        // Start with main menu visible, others hidden
        mainMenuCanvas.gameObject.SetActive(true);
        settingsCanvas.gameObject.SetActive(false);
        extrasCanvas.gameObject.SetActive(false); // Ensure extras hidden at start
    }

    // Called when Play button is pressed
    public void PlayGame()
    {
        SceneTransition transition = FindObjectOfType<SceneTransition>();

        if (transition != null)
        {
            transition.TransitionToScene("InterbeingBattlefield");
        }
        else
        {
            Debug.LogError("No SceneTransition found in the scene! Loading directly.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("InterbeingBattlefield");
        }
    }


    // Called when Settings button is pressed
    public void OpenSettings()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(true);
    }

    // Called when Back button in settings is pressed
    public void CloseSettings()
    {
        settingsCanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }

    // NEW: Called when Extras button is pressed
    public void OpenExtras()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        extrasCanvas.gameObject.SetActive(true);
    }

    // NEW: Called when Back button in extras is pressed
    public void CloseExtras()
    {
        extrasCanvas.gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }

    // Called when Quit button is pressed
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed in editor"); // Only visible in editor
    }
}
