using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy AI Idle State
/// Handles decision making: move towards nearest player, attack if adjacent
/// </summary>
public class EnemyIdleState : CharacterBaseState
{
    public override void Start(CharacterStateManager StateManager)
    {
        // Immediately take AI turn when entering idle state
        TakeAITurn(StateManager);
    }

    public override void Update(CharacterStateManager StateManager)
    {
        // Nothing happens here, actions handled in Start
    }

    private void TakeAITurn(CharacterStateManager manager)
    {
        if (manager.CharacterData.IsDowned) return;

        // Highlight all tiles the enemy can move to
        manager.GameTileTracker.HighlightTilesForCharacter(manager);

        // Check if any player is adjacent
        List<GameObject> adjacentEnemies = manager.GameTileTracker.GetAdjacentEnemies(manager);
        if (adjacentEnemies.Count > 0)
        {
            // Attack first adjacent player
            CharacterFunctions.Attack(manager, adjacentEnemies[0]);
            manager.EndTurn();
            return;
        }

        // No adjacent enemy, move toward nearest player
        GameObject target = GetNearestPlayer(manager);
        if (target != null)
        {
            GameTile targetTile = target.GetComponent<CharacterStateManager>().GameTileTracker
                .GetTileAt(target.GetComponent<CharacterStateManager>().MoveDestination);

            if (targetTile != null)
            {
                // Highlight the path to the target
                manager.GameTileTracker.HighlightTilesForCharacter(manager);

                // Start AI movement sequence toward the target
                manager.StartMoveSequence(targetTile.gameObject);
            }
        }
        else
        {
            // No players found, end turn
            manager.EndTurn();
        }
    }

    /// <summary>
    /// Finds the nearest player character to move toward
    /// </summary>
    private GameObject GetNearestPlayer(CharacterStateManager manager)
    {
        float shortestDist = float.MaxValue;
        GameObject bestTarget = null;
        Vector2Int enemyPos2D = new Vector2Int(manager.MoveDestination.x, manager.MoveDestination.y);

        foreach (var tilePair in manager.GameTileTracker.GameTileDictionary)
        {
            GameTile tile = tilePair.Value.GetComponent<GameTile>();
            if (tile.OccupyingCharacter == null) continue;

            CharacterGameData character = tile.OccupyingCharacter.GetComponent<CharacterGameData>();
            if (character.Team == manager.CharacterData.Team) continue; // Only target opposing teams

            float dist = Vector2Int.Distance(enemyPos2D, tilePair.Key);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                bestTarget = tile.OccupyingCharacter;
            }
        }

        return bestTarget;
    }
}
