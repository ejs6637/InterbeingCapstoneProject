using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    public Image leftPortrait;
    public Image rightPortrait;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public CanvasGroup dialoguePanel;

    [Header("Typing Settings")]
    public float typeSpeed = 0.03f;

    [Header("Opacity")]
    public float activeAlpha = 1f;
    public float inactiveAlpha = 0.3f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string fullText;

    public System.Action OnLineFinished;
    public bool dialogueFinished = false; // Added

    void Start()
    {
        dialoguePanel.alpha = 0f;
    }

    public void StartDialogue()
    {
        ShowPanel(true);
    }

    public void EndDialogue()
    {
        ShowPanel(false);
        dialogueFinished = true; // Signals cutscene that dialogue is done
    }

    public void ShowDialogue(string speakerName, string text, Sprite portrait, bool isLeftSide)
    {
        dialogueFinished = false; // reset each new line

        nameText.text = speakerName;
        fullText = text;
        dialogueText.text = "";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = true;

        if (isLeftSide)
        {
            leftPortrait.sprite = portrait;
            SetOpacity(leftPortrait, activeAlpha);
            SetOpacity(rightPortrait, inactiveAlpha);
        }
        else
        {
            rightPortrait.sprite = portrait;
            SetOpacity(rightPortrait, activeAlpha);
            SetOpacity(leftPortrait, inactiveAlpha);
        }

        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        OnLineFinished?.Invoke();
    }

    public void ContinueOrSkip()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = fullText;
            isTyping = false;
            OnLineFinished?.Invoke();
        }
    }

    private void ShowPanel(bool show)
    {
        dialoguePanel.alpha = show ? 1 : 0;
        dialoguePanel.blocksRaycasts = show;
    }

    private void SetOpacity(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
