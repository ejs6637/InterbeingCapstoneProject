using UnityEngine;

[System.Serializable]
public class CharacterData : MonoBehaviour
{
    public string characterName;
    public Sprite portrait;
    public int maxHP;
    public int currentHP;
    public int Movement = 3; // tiles per turn

    public bool isDowned => currentHP <= 0;

    [HideInInspector] public bool hasMoved = false;
    [HideInInspector] public bool turnEnded = false;
    [HideInInspector] public bool EnableMovementInput = false;

    public GameObject characterUI;
    public GameObject actionMenu;
    public Transform damagePopupParent;

    // Add this reference to link with CharacterStateManager
    public CharacterStateManager stateManager; // assign in inspector
}
