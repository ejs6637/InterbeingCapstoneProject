using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Characters/Stats")]
public class CharacterStats : ScriptableObject
{
    public string characterName;

    [Header("Base Stats")]
    public int maxHealth = 100;
    public int attack = 10;
    public int defense = 5;
    public int speed = 5;
}
