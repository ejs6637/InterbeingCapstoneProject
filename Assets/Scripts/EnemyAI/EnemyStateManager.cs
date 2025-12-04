using UnityEngine;

/// <summary>
/// Enemy version of the CharacterStateManager.
/// Uses AI to make decisions automatically instead of player input.
/// </summary>
public class EnemyStateManager : CharacterStateManager
{
    public EnemyIdleState EnemyIdle = new EnemyIdleState();

    protected override void Start()
    {
        base.Start();
        CharacterState = EnemyIdle;
        CharacterState.Start(this);
    }

    /// <summary>
    /// Called when this enemy's turn starts
    /// </summary>
    public override void StartTurn()
    {
        if (CharacterData.IsDowned)
        {
            EndTurn();
            return;
        }

        // Switch to AI decision state
        ChangeState(EnemyIdle);
    }

    /// <summary>
    /// Called when enemy movement phase ends
    /// </summary>
    public override void EndMovement()
    {
        // In the new TurnManager, ending movement triggers the enemy turn to end automatically
        TurnManager.Instance.EndEnemyTurn();
    }

    /// <summary>
    /// Called when the action phase starts
    /// </summary>
    public override void StartActionPhase()
    {
        // Skip player action menu; AI handles action automatically
        ChangeState(EnemyIdle);
    }

    /// <summary>
    /// Called when this enemy's turn ends
    /// </summary>
    public override void EndTurn()
    {
        // Calls the public EndEnemyTurn() in TurnManager
        TurnManager.Instance.EndEnemyTurn();
    }
}
