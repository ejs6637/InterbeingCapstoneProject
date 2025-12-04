using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea] public string text;
    public Sprite portrait;
    public bool isLeftSide = true;
}
