/// <summary>
/// Static master reference for constant project settings
/// </summary>
public static class Constants
{
    /// <summary>
    /// Tilemap Tag
    /// </summary>
    public static readonly string TilemapTag = "Tilemap";

    /// <summary>
    /// Game Tile Tag
    /// </summary>
    public static readonly string GameTileTag = "GameTile";

    /// <summary>
    /// The number of pixels in a tile single height unit
    /// </summary>
    public static readonly int PixelPerGameUnitHeight = 8;

    /// <summary>
    /// The number of game units a characteer sprite is treated as being
    /// This determines how many game units an occupied tile is treated as higher
    /// when calculating if an opposing unit can leap over it
    /// </summary>
    public static readonly int CharacterSpriteGameHeight = 3;

    /// <summary>
    /// The speed multiplied by deltaTime to determine how quickly a character
    /// will move between different game tiles during movement character states
    /// </summary>
    public static readonly float AnimationTravelSpeed = 1f;

    /// <summary>
    /// Animation Trigger for the Character Default Animation
    /// </summary>
    public static readonly string Default = "Default";

    /// <summary>
    /// Animation Trigger for the Character Idle Animation
    /// </summary>
    public static readonly string Idle = "Idle";

    /// <summary>
    /// Animation Trigger for the Character Crouch Animation
    /// </summary>
    public static readonly string Crouch = "Crouch";

    /// <summary>
    /// Animation Trigger for the Character Jump Animation
    /// </summary>
    public static readonly string Jump = "Jump";

    /// <summary>
    /// Animation Trigger for the Character Punch Animation
    /// </summary>
    public static readonly string Punch = "Punch";

    /// <summary>
    /// Animation Trigger for the Character SwordSwing Animation
    /// </summary>
    public static readonly string SwordSwing = "SwordSwing";

    /// <summary>
    /// Animation Trigger for the Character Injured Animation
    /// </summary>
    public static readonly string Injured = "Injured";

    /// <summary>
    /// Animation Trigger for the Character Downed Animation
    /// </summary>
    public static readonly string Downed = "Downed";

    /// <summary>
    /// Animation Trigger for the Character BackDefault Animation
    /// </summary>
    public static readonly string BackDefault = "BackDefault";

    /// <summary>
    /// Animation Trigger for the Character BackIdle Animation
    /// </summary>
    public static readonly string BackIdle = "BackIdle";

    /// <summary>
    /// Animation Trigger for the Character BackCrouch Animation
    /// </summary>
    public static readonly string BackCrouch = "BackCrouch";

    /// <summary>
    /// Animation Trigger for the Character BackJump Animation
    /// </summary>
    public static readonly string BackJump = "BackJump";

    /// <summary>
    /// Animation Trigger for the Character BackPunch Animation
    /// </summary>
    public static readonly string BackPunch = "BackPunch";

    /// <summary>
    /// Animation Trigger for the Character BackSwordSwing Animation
    /// </summary>
    public static readonly string BackSwordSwing = "BackSwordSwing";

    /// <summary>
    /// Animation Trigger for the Character BackInjured Animation
    /// </summary>
    public static readonly string BackInjured = "BackInjured";

    /// <summary>
    /// Animation Trigger for the Character Downed Animation
    /// </summary>
    public static readonly string BackDowned = "BackDowned";
}