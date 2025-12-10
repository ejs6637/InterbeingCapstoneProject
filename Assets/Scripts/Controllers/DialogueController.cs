using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public DialogueLine[] lines;

    public System.Action OnConversationComplete;

    private int currentLineIndex = -1;
    private bool canContinue = false;

    public void StartConversation()
    {
        currentLineIndex = -1;
        dialogueUI.StartDialogue();
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= lines.Length)
        {
            EndConversation();
            return;
        }

        DialogueLine line = lines[currentLineIndex];
        dialogueUI.ShowDialogue(line.speakerName, line.text, line.portrait, line.isLeftSide);

        dialogueUI.OnLineFinished = () => canContinue = true;
        canContinue = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canContinue)
                ShowNextLine();
            else
                dialogueUI.ContinueOrSkip();
        }
    }

    private void EndConversation()
    {
        dialogueUI.EndDialogue();
        OnConversationComplete?.Invoke();
    }
}
