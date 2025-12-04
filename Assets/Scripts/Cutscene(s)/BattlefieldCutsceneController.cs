using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattlefieldCutsceneController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject[] playerHUDs;
    public GameObject[] enemyHUDs;
    public GameObject[] actionPanels;

    [Header("Character References")]
    public GameObject[] characters;
    public DialogueUI dialogueUI;

    [Header("Cutscene Image")]
    public Image cutsceneImage;
    public float shakeMagnitude = 5f;
    public float shakeDuration = 0.5f;

    [Header("Flash Settings")]
    public Image flashImage;
    public float flashDuration = 0.2f;

    [Header("Tile Cursor")]
    public GameObject tileCursorPrefab;

    [Header("Audio")]
    public AudioSource cutsceneAudio;

    [Header("Timing")]
    public float delayBeforeShake = 0.5f;

    [Header("Tilemap/Grid References")]
    public GameObject tilemapGameObject;
    public GameObject gridGameObject;

    private Vector3 originalImagePos;

    private void Start()
    {
        // Hide gameplay UI and characters at start
        SetUIActive(playerHUDs, false);
        SetUIActive(enemyHUDs, false);
        SetUIActive(actionPanels, false);
        SetGameObjectsActive(characters, false);

        if (dialogueUI)
            dialogueUI.dialoguePanel.gameObject.SetActive(false);

        if (tileCursorPrefab)
            tileCursorPrefab.SetActive(false);

        // Prepare cutscene image
        if (cutsceneImage)
        {
            cutsceneImage.gameObject.SetActive(true);
            originalImagePos = cutsceneImage.rectTransform.localPosition;
        }

        if (flashImage)
            flashImage.gameObject.SetActive(false);

        // Hide system cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Start the cutscene
        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        // Optional delay
        yield return new WaitForSeconds(delayBeforeShake);

        // Play cutscene audio
        if (cutsceneAudio != null)
            cutsceneAudio.Play();

        // Shake the cutscene image
        if (cutsceneImage != null)
            yield return StartCoroutine(ShakeImage());

        // Stop audio
        if (cutsceneAudio != null)
            cutsceneAudio.Stop();

        // Flash white
        if (flashImage != null)
            yield return StartCoroutine(FlashWhite());

        // Hide cutscene image
        if (cutsceneImage != null)
            cutsceneImage.gameObject.SetActive(false);

        // Re-enable tilemap/grid
        if (tilemapGameObject != null)
            tilemapGameObject.SetActive(true);
        if (gridGameObject != null)
            gridGameObject.SetActive(true);

        // Start dialogue if available
        if (dialogueUI != null)
        {
            // Ensure dialogue panel is active
            dialogueUI.dialoguePanel.gameObject.SetActive(true);

            bool dialogueFinished = false;
            dialogueUI.OnLineFinished += () => dialogueFinished = true;

            dialogueUI.StartDialogue();

            // Wait until dialogue is finished
            while (!dialogueFinished)
                yield return null;
        }

        // After cutscene and dialogue: show characters, cursor, and begin player turn
        OnCutsceneFinished();
    }

    private void OnCutsceneFinished()
    {
        SetGameObjectsActive(characters, true);

        // Show tile cursor
        if (tileCursorPrefab != null)
            tileCursorPrefab.SetActive(true);

        // Show gameplay UI if needed
        SetUIActive(playerHUDs, true);
        SetUIActive(actionPanels, true);

        // Restore cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Begin player turn
        if (TurnManager.Instance != null)
            TurnManager.Instance.BeginPlayerTurn();

        // Remove dialogue listener
        if (dialogueUI != null)
            dialogueUI.OnLineFinished = null;
    }

    private IEnumerator ShakeImage()
    {
        float timer = 0f;
        float frequency = 3f;
        Vector3 originalPos = cutsceneImage.rectTransform.anchoredPosition;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            float offsetX = Mathf.Sin(timer * frequency * Mathf.PI * 2f) * shakeMagnitude;
            cutsceneImage.rectTransform.anchoredPosition = originalPos + new Vector3(offsetX, 0f, 0f);
            yield return null;
        }

        cutsceneImage.rectTransform.anchoredPosition = originalPos;
    }

    private IEnumerator FlashWhite()
    {
        flashImage.gameObject.SetActive(true);
        Color color = flashImage.color;
        color.a = 0f;
        flashImage.color = color;

        // Fade in
        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / flashDuration);
            flashImage.color = color;
            yield return null;
        }

        // Fade out
        timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / flashDuration);
            flashImage.color = color;
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }

    private void SetUIActive(GameObject[] uiArray, bool state)
    {
        if (uiArray == null) return;
        foreach (var ui in uiArray)
        {
            if (ui != null)
                ui.SetActive(state);
        }
    }

    private void SetGameObjectsActive(GameObject[] objects, bool state)
    {
        if (objects == null) return;
        foreach (var obj in objects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}
