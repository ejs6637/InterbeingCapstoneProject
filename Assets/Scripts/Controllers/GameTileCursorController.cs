using UnityEngine;

/// <summary>
/// Orientation setting for an inclined cursor
/// </summary>
public enum InclineOrientation
{
    DownToLeft,
    DownToRight
}

public class GameTileCursorController : MonoBehaviour
{
    public Sprite FlatCursorSprite;
    public Sprite SingleInclinedCursorSprite;
    public InclineOrientation SingleOrientation;
    public Sprite DoubleInclinedCursorSprite;
    public InclineOrientation DoubleOrientation;

    private SpriteRenderer CursorSpriteRenderer;
    private GameObject CurrentGameTile;
    private TileSurfaceOrientation CurrentInclineOrientation;
    private TileInclineRise CurrentInclineRise;

    private void Awake()
    {
        CursorSpriteRenderer = this.GetComponent<SpriteRenderer>();
        if (CursorSpriteRenderer == null)
            Debug.LogError("No SpriteRenderer attached to Game Tile Cursor!");

        // Ensure the cursor always sorts above tiles
        CursorSpriteRenderer.sortingOrder = 500;
    }

    void Update()
    {
        //Mouse moved
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (Camera.main == null)
            {
                Debug.LogError("Camera.main is missing a MainCamera tag!");
                return;
            }

            GameObject VisibleGameTile =
                GameTileFunctions.GetGameTileFromPositionalRaycast(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (VisibleGameTile != null && VisibleGameTile != CurrentGameTile)
            {
                UpdateCurrentGameTileCursor(VisibleGameTile);

                GameTileTracker gameTileTracker =
                    GameObject.FindGameObjectWithTag(Constants.TilemapTag)?.GetComponent<GameTileTracker>();

                if (gameTileTracker == null)
                    return;

                if (gameTileTracker.HighlightedPath.Count > 0 &&
                    CurrentGameTile != null &&
                    gameTileTracker.DestinationPathfindingMap.ContainsKey(CurrentGameTile))
                {
                    if (CurrentGameTile.GetComponent<GameTile>().OccupyingCharacter == null)
                    {
                        foreach (var tile in gameTileTracker.HighlightedPath)
                        {
                            if (tile != null)
                                GameTileFunctions.HighlightGameTile(tile, Color.cyan);
                        }
                    }

                    gameTileTracker.HighlightedPath.Clear();

                    GameObject recursiveGameTile = CurrentGameTile;
                    for (int i = 0; i < gameTileTracker.DestinationPathfindingMap.Count; i++)
                    {
                        GameTileFunctions.HighlightGameTile(recursiveGameTile, Color.yellow);
                        gameTileTracker.HighlightedPath.Add(recursiveGameTile);

                        if (gameTileTracker.DestinationPathfindingMap[recursiveGameTile] == null)
                            break;

                        recursiveGameTile = gameTileTracker.DestinationPathfindingMap[recursiveGameTile];
                    }
                }
            }
        }

        //Left Click
        if (Input.GetMouseButtonDown(0))
        {
            if (CurrentGameTile == null) return; // Safety fix

            GameTile currentGameTileComponent = CurrentGameTile.GetComponent<GameTile>();
            if (currentGameTileComponent == null) return;

            if (currentGameTileComponent.OccupyingCharacter != null &&
                currentGameTileComponent.OccupyingCharacter.GetComponent<CharacterGameData>().CharacterActive)
            {
                Debug.Log($"Character Found at X:{currentGameTileComponent.CellPositionX} " +
                    $"Y:{currentGameTileComponent.CellPositionY}, highlighting pathfinding returns");

                GameTileTracker gameTileTracker =
                    GameObject.FindGameObjectWithTag(Constants.TilemapTag)?.GetComponent<GameTileTracker>();

                if (gameTileTracker == null) return;

                if (gameTileTracker.DestinationPathfindingMap.Count == 0)
                {
                    CharacterGameData currentCharacterGameData =
                        currentGameTileComponent.OccupyingCharacter.GetComponent<CharacterGameData>();

                    gameTileTracker.DestinationPathfindingMap = GameTileFunctions.GetDestinationGameTiles(
                        gameTileTracker.GameTileDictionary,
                        new Vector2Int(currentGameTileComponent.CellPositionX, currentGameTileComponent.CellPositionY),
                        currentCharacterGameData.Movement,
                        currentCharacterGameData.VerticalJump,
                        currentCharacterGameData.HorizontalLeap,
                        false, false);

                    foreach (GameObject keyTile in gameTileTracker.DestinationPathfindingMap.Keys)
                    {
                        if (keyTile.GetComponent<GameTile>().OccupyingCharacter == null)
                            GameTileFunctions.HighlightGameTile(keyTile, Color.cyan);
                    }
                    GameTileFunctions.HighlightGameTile(CurrentGameTile, Color.yellow);
                    gameTileTracker.HighlightedPath.Add(CurrentGameTile);
                }
                else
                {
                    foreach (GameObject mappedGameTile in gameTileTracker.DestinationPathfindingMap.Keys)
                    {
                        GameTileFunctions.UnhighlightGameTile(mappedGameTile);
                    }
                    gameTileTracker.DestinationPathfindingMap.Clear();
                    gameTileTracker.HighlightedPath.Clear();
                }
            }
        }

        //Right Click
        if (Input.GetMouseButtonDown(1))
        {
            if (CurrentGameTile == null) return; // Safety fix

            GameTileTracker gameTileTracker =
                GameObject.FindGameObjectWithTag(Constants.TilemapTag)?.GetComponent<GameTileTracker>();

            if (gameTileTracker == null) return;

            if (gameTileTracker.HighlightedPath.Contains(CurrentGameTile) &&
                gameTileTracker.DestinationPathfindingMap.ContainsKey(CurrentGameTile) &&
                gameTileTracker.DestinationPathfindingMap[CurrentGameTile] != null)
            {
                GameObject recursiveGameTile = CurrentGameTile;
                for (int i = 0; i < gameTileTracker.DestinationPathfindingMap.Count; i++)
                {
                    GameTileFunctions.HighlightGameTile(recursiveGameTile, Color.yellow);
                    gameTileTracker.HighlightedPath.Add(recursiveGameTile);

                    if (gameTileTracker.DestinationPathfindingMap[recursiveGameTile] == null)
                        break;

                    recursiveGameTile = gameTileTracker.DestinationPathfindingMap[recursiveGameTile];
                }

                recursiveGameTile.GetComponent<GameTile>().OccupyingCharacter
                    .GetComponent<CharacterStateManager>().StartMoveSequence(CurrentGameTile);
            }
        }
    }

    void UpdateCurrentGameTileCursor(GameObject NewGameTile)
    {
        CurrentGameTile = NewGameTile;

        GameTile gameTileComponent = CurrentGameTile.GetComponent<GameTile>();
        if (gameTileComponent == null) return; // Extra safety

        this.transform.position = new Vector3(
            CurrentGameTile.transform.position.x,
            CurrentGameTile.transform.position.y,
            CurrentGameTile.transform.position.z - 0.75f);

        if (CurrentInclineRise != gameTileComponent.InclineRise)
        {
            switch (gameTileComponent.InclineRise)
            {
                case TileInclineRise.Zero:
                    CursorSpriteRenderer.sprite = FlatCursorSprite;
                    break;
                case TileInclineRise.Single:
                    CursorSpriteRenderer.sprite = SingleInclinedCursorSprite;
                    break;
                case TileInclineRise.Double:
                    CursorSpriteRenderer.sprite = DoubleInclinedCursorSprite;
                    break;
            }
            CurrentInclineRise = gameTileComponent.InclineRise;
        }

        if (CurrentInclineOrientation != gameTileComponent.SurfaceOrientation)
        {
            switch (gameTileComponent.SurfaceOrientation)
            {
                case TileSurfaceOrientation.Flat:
                    break;
                case TileSurfaceOrientation.InclinedDownToLeft:
                    CursorSpriteRenderer.flipX = (SingleOrientation != InclineOrientation.DownToLeft);
                    break;
                case TileSurfaceOrientation.InclinedDownToRight:
                    CursorSpriteRenderer.flipX = (SingleOrientation != InclineOrientation.DownToRight);
                    break;
            }
            CurrentInclineOrientation = gameTileComponent.SurfaceOrientation;
        }
    }
}
