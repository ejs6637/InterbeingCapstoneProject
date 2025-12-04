using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the bottom-right action menu for character turns
/// </summary>
public class ActionMenuUI : MonoBehaviour
{
    public static ActionMenuUI Instance;

    [Header("UI References")]
    public GameObject actionMenuPanel; // The panel containing all action buttons
    public Button attackButton;
    public Button waitButton;

    private CharacterStateManager currentCharacter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Optional: hide menu on start
        HideMenu();
    }

    private void OnEnable()
    {
        if (attackButton != null)
            attackButton.onClick.AddListener(OnAttackButton);

        if (waitButton != null)
            waitButton.onClick.AddListener(OnWaitButton);
    }

    private void OnDisable()
    {
        if (attackButton != null)
            attackButton.onClick.RemoveListener(OnAttackButton);

        if (waitButton != null)
            waitButton.onClick.RemoveListener(OnWaitButton);
    }

    /// <summary>
    /// Show the menu and assign the character that is currently acting
    /// </summary>
    /// <param name="character">The character whose turn it is</param>
    public void ShowMenu(CharacterStateManager character)
    {
        currentCharacter = character;

        if (actionMenuPanel != null)
            actionMenuPanel.SetActive(true);

        if (attackButton != null)
            attackButton.gameObject.SetActive(true);

        if (waitButton != null)
            waitButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide the menu and clear the current character reference
    /// </summary>
    public void HideMenu()
    {
        if (actionMenuPanel != null)
            actionMenuPanel.SetActive(false);

        if (attackButton != null)
            attackButton.gameObject.SetActive(false);

        if (waitButton != null)
            waitButton.gameObject.SetActive(false);

        currentCharacter = null;
    }

    /// <summary>
    /// Called when the Attack button is pressed
    /// </summary>
    private void OnAttackButton()
    {
        if (currentCharacter != null)
        {
            currentCharacter.PerformAttack();
            HideMenu();
        }
        else
        {
            Debug.LogWarning("ActionMenuUI: No current character assigned!");
        }
    }

    /// <summary>
    /// Called when the Wait button is pressed
    /// </summary>
    private void OnWaitButton()
    {
        if (currentCharacter != null)
        {
            currentCharacter.Wait();
            HideMenu();
        }
        else
        {
            Debug.LogWarning("ActionMenuUI: No current character assigned!");
        }
    }
}
