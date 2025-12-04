using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public Sprite heroSprite;
    public Sprite villainSprite;

    private void Start()
    {
        dialogueUI.ShowDialogue("Hero", "We must hurry!", heroSprite, true);

        // Example to call the next line later (e.g., pressing a button)
        // dialogueUI.ShowDialogue("Villain", "You cannot escape!", villainSprite, false);
    }
}
