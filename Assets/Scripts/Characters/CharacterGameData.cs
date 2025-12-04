using UnityEngine;
using UnityEngine.U2D.Animation;

/// <summary>
/// Direction the Character is facing from the camera's perspective - front is facing towards camera, back is facing away from it
/// left is the camera's left, and right is the camera's right
/// </summary>
public enum CharacterDirectionFacing
{
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}

/// <summary>
/// Unit Type: Player units are controlled by the player, enemies and allies by AI, though allies can be moved through
/// </summary>
public enum CharacterTeam
{
    Player,
    Enemy,
    Ally
}

/// <summary>
/// Component of a Character Game Object that stores all relevant game data for APIs
/// </summary>
public class CharacterGameData : MonoBehaviour
{
    [Header("Graphic References")]
    //Animation Data
    public SpriteRenderer CharacterSpriteRenderer;
    public Animator CharacterAnimator;
    public SpriteLibrary CharacterSpriteLibrary;
    public SpriteLibraryAsset FrontSpriteLibraryAsset;
    public SpriteLibraryAsset BackSpriteLibraryAsset;
    public AnimationCurve LeapingCurve;

    [Space(10)]
    [Header("Character Data")]
    //Team the Character is controlled by
    public CharacterTeam Team;
    //Character Direction
    public CharacterDirectionFacing DirectionFaced;
    //Character's Names
    public string CharacterName;
    //Character Animator's Current State
    public string AnimatorState = Constants.Idle;

    //State to determine whether the character can act during this turn/action
    public bool CharacterActive = false;

    //Number of Game Tiles that can be traversed in one move
    public int Movement;
    //Number of game height units that can be jumped up
    public int VerticalJump;
    //Number of game tiles that can be leapt across
    public int HorizontalLeap;

    [Header("Combat Data")]
    public int MaxHealth = 10;       // Maximum health
    public int Health = 10;          // Current health
    public int AttackPower = 1;      // Damage dealt per attack

    [HideInInspector] public bool IsInjured = false; // Becomes true when health < 2/3
    [HideInInspector] public bool IsDowned = false;  // Becomes true when health <= 0

}