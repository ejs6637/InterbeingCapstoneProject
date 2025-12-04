using System;
using UnityEngine;

/// <summary>
/// Useful Static Functions revolving around Characters
/// </summary>
public static class CharacterFunctions
{
    public static void ChangeAnimationState(string newAnimationState, CharacterGameData characterGameData)
    {
        characterGameData.CharacterAnimator.SetTrigger(newAnimationState);
    }

    public static void ChangeOrientation(CharacterDirectionFacing newOrientation, CharacterGameData characterGameData)
    {
        switch (newOrientation)
        {
            case CharacterDirectionFacing.FrontLeft:
                characterGameData.CharacterSpriteLibrary.spriteLibraryAsset = characterGameData.FrontSpriteLibraryAsset;
                characterGameData.CharacterSpriteRenderer.flipX = false;
                break;
            case CharacterDirectionFacing.FrontRight:
                characterGameData.CharacterSpriteLibrary.spriteLibraryAsset = characterGameData.FrontSpriteLibraryAsset;
                characterGameData.CharacterSpriteRenderer.flipX = true;
                break;
            case CharacterDirectionFacing.BackRight:
                characterGameData.CharacterSpriteLibrary.spriteLibraryAsset = characterGameData.BackSpriteLibraryAsset;
                characterGameData.CharacterSpriteRenderer.flipX = false;
                break;
            case CharacterDirectionFacing.BackLeft:
                characterGameData.CharacterSpriteLibrary.spriteLibraryAsset = characterGameData.BackSpriteLibraryAsset;
                characterGameData.CharacterSpriteRenderer.flipX = true;
                break;
            default:
                Debug.LogError($"Invalid Character Orientation");
                return;
        }

        characterGameData.DirectionFaced = newOrientation;
    }

    public static CharacterDirectionFacing DetermineOrientation(Vector2Int originCoordinates, Vector2Int destinationCoordinates)
    {
        if (destinationCoordinates.x - originCoordinates.x < 0 &&
            Math.Abs(destinationCoordinates.x - originCoordinates.x) >= Math.Abs(destinationCoordinates.y - originCoordinates.y))
            return CharacterDirectionFacing.FrontLeft;

        if (destinationCoordinates.y - originCoordinates.y < 0 &&
            Math.Abs(destinationCoordinates.x - originCoordinates.x) <= Math.Abs(destinationCoordinates.y - originCoordinates.y))
            return CharacterDirectionFacing.FrontRight;

        if (destinationCoordinates.y - originCoordinates.y > 0 &&
            Math.Abs(destinationCoordinates.x - originCoordinates.x) <= Math.Abs(destinationCoordinates.y - originCoordinates.y))
            return CharacterDirectionFacing.BackLeft;

        return CharacterDirectionFacing.BackRight;
    }

    public static Vector3 GetCharacterPositionOnGameTile(GameObject GameTile)
    {
        GameTile gameTileComponent = GameTile.GetComponent<GameTile>();

        return new Vector3(
            GameTile.transform.position.x,
            GameTile.transform.position.y - ((float)gameTileComponent.InclineGameHeight / Constants.PixelPerGameUnitHeight),
            GameTile.transform.position.z - ((float)gameTileComponent.InclineGameHeight / 2) + 0.75f);
    }

    // Battle
    public static void Attack(CharacterStateManager attacker, GameObject target)
    {
        if (target == null) return;

        CharacterGameData targetData = target.GetComponent<CharacterGameData>();
        if (targetData == null)
        {
            Debug.LogWarning("Target does not have CharacterGameData!");
            return;
        }

        targetData.Health -= attacker.CharacterData.AttackPower;

        ChangeAnimationState(Constants.SwordSwing, attacker.CharacterData);

        if (targetData.Health <= 0)
        {
            targetData.IsDowned = true;
            ChangeAnimationState(Constants.Downed, targetData);
        }
        else if (targetData.Health <= targetData.MaxHealth * 2f / 3f)
        {
            targetData.IsInjured = true;
            ChangeAnimationState(Constants.Injured, targetData);
        }
    }

    public static GameObject FindAdjacentEnemy(CharacterStateManager character)
    {
        Vector3Int pos = character.MoveDestination;
        GameTileTracker tracker = character.GameTileTracker;

        GameTile[] adjacentTiles = new GameTile[]
        {
            tracker.GetTileAt(pos + new Vector3Int(0, 1, 0)),
            tracker.GetTileAt(pos + new Vector3Int(0, -1, 0)),
            tracker.GetTileAt(pos + new Vector3Int(1, 0, 0)),
            tracker.GetTileAt(pos + new Vector3Int(-1, 0, 0))
        };

        foreach (var tile in adjacentTiles)
        {
            if (tile != null && tile.OccupyingCharacter != null)
            {
                CharacterGameData data = tile.OccupyingCharacter.GetComponent<CharacterGameData>();
                if (data.Team != character.CharacterData.Team)
                    return tile.OccupyingCharacter;
            }
        }

        return null;
    }
}
