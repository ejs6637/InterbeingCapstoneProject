using UnityEngine;

[CreateAssetMenu(fileName = "CharacterUIPreset", menuName = "UI/CharacterUISettings")]
public class CharacterUISettings : ScriptableObject
{
    [Header("UI References")]
    public GameObject CharacterUIPanel;   // Panel for the character
    public GameObject ActionMenu;         // Shared action menu
    public GameObject DamagePopupPrefab;  // Floating damage numbers prefab
}
