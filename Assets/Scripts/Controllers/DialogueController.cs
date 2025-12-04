using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public DialogueLine[] lines;

    private int currentLineIndex = -1;

    void Start()
    {
        StartConversation();
    }

    public void StartConversation()
    {
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

        dialogueUI.OnLineFinished = () =>
        {
            // Enable player input to continue once typing is done
            canContinue = true;
        };

        canContinue = false;
    }

    private bool canContinue = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canContinue)
                ShowNextLine();
            else
                dialogueUI.ContinueOrSkip();
        }
    }

    void EndConversation()
    {
        dialogueUI.EndDialogue();
        Debug.Log("Dialogue finished!");
        // Trigger gameplay UI now if needed
        // TurnManager.Instance.BeginPlayerTurn();
    }
}
