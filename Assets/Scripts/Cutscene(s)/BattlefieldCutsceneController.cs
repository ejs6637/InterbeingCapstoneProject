using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattlefieldCutsceneController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject[] playerHUDs;
    public GameObject[] enemyHUDs;
    public GameObject[] actionPanels;

    [Header("Characters")]
    public GameObject[] playerCharacters;
    public GameObject[] enemyCharacters; // NEW
    public DialogueController dialogueController;

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

    private void Start()
    {
        // Hide everything during cutscene
        SetUIActive(playerHUDs, false);
        SetUIActive(enemyHUDs, false);
        SetUIActive(actionPanels, false);
        HideCharacters();
        tileCursorPrefab?.SetActive(false);
        tilemapGameObject?.SetActive(false);
        gridGameObject?.SetActive(false);

        cutsceneImage?.gameObject.SetActive(true);
        flashImage?.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        yield return new WaitForSeconds(delayBeforeShake);

        cutsceneAudio?.Play();
        if (cutsceneImage != null) yield return StartCoroutine(ShakeImage());
        cutsceneAudio?.Stop();

        if (flashImage != null) yield return StartCoroutine(FlashWhite());

        if (cutsceneImage != null) cutsceneImage.gameObject.SetActive(false);

        // Dialogue sequence
        if (dialogueController != null)
        {
            bool dialogueFinished = false;
            dialogueController.OnConversationComplete += () => dialogueFinished = true;

            dialogueController.StartConversation();

            while (!dialogueFinished)
                yield return null;
        }

        // Reveal gameplay again
        RestoreCharacters();
        SetUIActive(playerHUDs, true);
        SetUIActive(enemyHUDs, true);
        SetUIActive(actionPanels, true);
        tileCursorPrefab?.SetActive(true);
        tilemapGameObject?.SetActive(true);
        gridGameObject?.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        TurnManager.Instance?.BeginPlayerTurn();
    }

    private IEnumerator ShakeImage()
    {
        Vector3 originalPos = cutsceneImage.rectTransform.anchoredPosition;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            float offsetX = Mathf.Sin(timer * 40f) * shakeMagnitude;
            cutsceneImage.rectTransform.anchoredPosition = originalPos + new Vector3(offsetX, 0, 0);
            yield return null;
        }

        cutsceneImage.rectTransform.anchoredPosition = originalPos;
    }

    private IEnumerator FlashWhite()
    {
        flashImage.gameObject.SetActive(true);
        Color c = flashImage.color;
        float timer = 0f;

        // fade in
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, timer / flashDuration);
            flashImage.color = c;
            yield return null;
        }

        // fade out
        timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, timer / flashDuration);
            flashImage.color = c;
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }

    private void SetUIActive(GameObject[] arr, bool state)
    {
        foreach (var go in arr)
            if (go != null) go.SetActive(state);
    }

    private void SetGameObjectsActive(GameObject[] arr, bool state)
    {
        foreach (var go in arr)
            if (go != null) go.SetActive(state);
    }

    #region Character Visibility Fix

    private Vector3[] playerOriginalPositions;
    private Vector3[] enemyOriginalPositions;

    private void HideCharacters()
    {
        // Save original positions
        playerOriginalPositions = new Vector3[playerCharacters.Length];
        enemyOriginalPositions = new Vector3[enemyCharacters.Length];

        for (int i = 0; i < playerCharacters.Length; i++)
        {
            if (playerCharacters[i] != null)
            {
                playerOriginalPositions[i] = playerCharacters[i].transform.position;
                playerCharacters[i].transform.position += new Vector3(0, -5000f, 0);
            }
        }

        for (int i = 0; i < enemyCharacters.Length; i++)
        {
            if (enemyCharacters[i] != null)
            {
                enemyOriginalPositions[i] = enemyCharacters[i].transform.position;
                enemyCharacters[i].transform.position += new Vector3(0, -5000f, 0);
            }
        }
    }

    private void RestoreCharacters()
    {
        // Return characters to original positions
        for (int i = 0; i < playerCharacters.Length; i++)
        {
            if (playerCharacters[i] != null)
                playerCharacters[i].transform.position = playerOriginalPositions[i];
        }

        for (int i = 0; i < enemyCharacters.Length; i++)
        {
            if (enemyCharacters[i] != null)
                enemyCharacters[i].transform.position = enemyOriginalPositions[i];
        }
    }

    #endregion

}
