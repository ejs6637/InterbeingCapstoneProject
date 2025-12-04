using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{
    [HideInInspector] public CharacterGameData CharacterData;

    [Header("UI")]
    public GameObject CharacterUI; // assign this in the Inspector

    protected CharacterBaseState CharacterState;
    public CharacterIdleState Idle = new CharacterIdleState();
    public CharacterWalkState Walk = new CharacterWalkState();
    public CharacterLeapState Leap = new CharacterLeapState();
    public CharacterDownedState Downed = new CharacterDownedState();

    [HideInInspector] public GameTileTracker GameTileTracker;
    [HideInInspector] public Vector3Int MoveOrigin;
    [HideInInspector] public Vector3Int MoveDestination;
    [HideInInspector] public Vector3 MoveDestinationPosition;
    [HideInInspector] public float DestinationZMultiplier;
    public LinkedList<GameObject> RemainingDestinationWaypoints = new LinkedList<GameObject>();

    protected virtual void Start()
    {
        GameTileTracker = GameObject.FindGameObjectWithTag(Constants.TilemapTag)
            .GetComponent<GameTileTracker>();
        CharacterData = GetComponent<CharacterGameData>();
        CharacterState = Idle;
        CharacterState.Start(this);

        // Hide UI at start; TurnManager will show only active character's UI
        if (CharacterUI != null)
            CharacterUI.SetActive(false);
    }

    void Update()
    {
        if (CharacterState != null)
            CharacterState.Update(this);
    }


    // turns
    public virtual void StartTurn()
    {
        if (CharacterData.IsDowned)
        {
            EndTurn();
            return;
        }

        // Enable this character's UI when their turn starts
        if (CharacterUI != null)
            CharacterUI.SetActive(true);

        UpdateIdleSprite();
        HighlightMovableTiles();
        EnableMovementInput();
    }

    public virtual void EndTurn()
    {
        // Hide this character's UI when their turn ends
        if (CharacterUI != null)
            CharacterUI.SetActive(false);

        ClearHighlights();
        TurnManager.Instance.EndActionPhase(this);
    }

    public virtual void EndMovement()
    {
        DisableMovementInput();
        TurnManager.Instance.EndMovementPhase(this);
    }

    public virtual void StartActionPhase()
    {
        ActionMenuUI.Instance.ShowMenu(this);
    }

    public void SetActionPhase()
    {
        if (CharacterData.Team == CharacterTeam.Player)
            CharacterData.CharacterActive = true; // enable player action
    }

    public void PerformAttack()
    {
        GameObject enemy = CharacterFunctions.FindAdjacentEnemy(this);
        if (enemy != null)
            CharacterFunctions.Attack(this, enemy);
        EndTurn();
    }

    public void Wait()
    {
        EndTurn();
    }

    // movement
    public void ChangeState(CharacterBaseState NewState)
    {
        CharacterState = NewState;
        CharacterState.Start(this);
    }

    public void StartMoveSequence(GameObject DestinationGameTileObject)
    {
        GameObject TileToEnqueue = DestinationGameTileObject;
        for (int i = 0; i < CharacterData.Movement + 1; i++)
        {
            if (GameTileTracker.DestinationPathfindingMap[TileToEnqueue] == null) break;
            RemainingDestinationWaypoints.AddFirst(TileToEnqueue);
            TileToEnqueue = GameTileTracker.DestinationPathfindingMap[TileToEnqueue];
        }

        GameTile FirstOrigin = TileToEnqueue.GetComponent<GameTile>();
        FirstOrigin.OccupyingCharacter = null;
        DestinationGameTileObject.GetComponent<GameTile>().OccupyingCharacter = gameObject;

        MoveDestination = new Vector3Int(FirstOrigin.CellPositionX, FirstOrigin.CellPositionY, FirstOrigin.CellPositionZ);
        MoveToNextWaypoint();
    }

    public void MoveToNextWaypoint()
    {
        if (RemainingDestinationWaypoints.Count == 0)
        {
            UpdateIdleSprite();
            ChangeState(Idle);
            return;
        }

        GameTile NextDestination = RemainingDestinationWaypoints.First.Value.GetComponent<GameTile>();
        MoveOrigin = MoveDestination;
        MoveDestination = new Vector3Int(NextDestination.CellPositionX, NextDestination.CellPositionY, NextDestination.CellPositionZ);
        MoveDestinationPosition = CharacterFunctions.GetCharacterPositionOnGameTile(RemainingDestinationWaypoints.First.Value);
        DestinationZMultiplier = 1 + Mathf.Abs(transform.position.z - MoveDestinationPosition.z);

        RemainingDestinationWaypoints.RemoveFirst();

        if (Mathf.Abs(MoveDestination.x - MoveOrigin.x) > 1 || Mathf.Abs(MoveDestination.y - MoveOrigin.y) > 1)
            ChangeState(Leap);
        else
        {
            switch (MoveDestination.z - MoveOrigin.z)
            {
                case > 1: ChangeState(Leap); break;
                case <= 1 and >= -1: ChangeState(Walk); break;
                case < -1: ChangeState(Leap); break;
            }
        }
    }

    // misc.
    private void UpdateIdleSprite()
    {
        if (CharacterData.IsDowned)
            ChangeState(Downed);
        else if (CharacterData.IsInjured)
            CharacterFunctions.ChangeAnimationState(Constants.Injured, CharacterData);
        else
            CharacterFunctions.ChangeAnimationState(Constants.Idle, CharacterData);
    }

    private void HighlightMovableTiles()
    {
        GameTileTracker.HighlightTilesForCharacter(this);
    }

    private void ClearHighlights()
    {
        GameTileTracker.ClearHighlights();
    }

    private void EnableMovementInput()
    {
        GameTileTracker.HighlightTilesForCharacter(this);
    }

    private void DisableMovementInput()
    {
        GameTileTracker.ClearHighlights();
    }
}
