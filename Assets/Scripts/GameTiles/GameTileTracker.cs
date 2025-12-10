using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to the Tilemap by the Game Tile Manager
/// Tracks tiles, pathfinding, and highlights for movement & actions
/// </summary>
/// 
[DefaultExecutionOrder(-100)]
public class GameTileTracker : MonoBehaviour
{
    public static GameTileTracker Instance;

    public Dictionary<Vector2Int, GameObject> GameTileDictionary = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<GameObject, GameObject> DestinationPathfindingMap = new Dictionary<GameObject, GameObject>();
    public HashSet<GameObject> HighlightedPath = new HashSet<GameObject>();
    public List<GameTile> allGameTiles = new List<GameTile>();

    [Header("Highlight Settings")]
    public GameObject HighlightPrefab; // Assign in Inspector
    public string HighlightSortingLayer = "Highlights"; // Sorting layer for highlights
    public int SortingOrderAboveTile = 1; // relative to tile's order

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
            Instance = this;
        else
        {
            return;
        }

        // Register all tiles
        GameObject[] gameTiles = GameObject.FindGameObjectsWithTag(Constants.GameTileTag);
        foreach (GameObject gameTile in gameTiles)
        {
            GameTile tileComp = gameTile.GetComponent<GameTile>();
            Vector2Int key = new Vector2Int(tileComp.CellPositionX, tileComp.CellPositionY);
            if (!GameTileDictionary.ContainsKey(key))
                GameTileDictionary.Add(key, gameTile);
        }

        {
            allGameTiles.Clear();
            GameTile[] tiles = FindObjectsOfType<GameTile>();

            foreach (var tile in tiles)
                allGameTiles.Add(tile);
        }


    }

    /// <summary>
    /// Highlights all tiles a character can move to based on its movement
    /// </summary>
    public void HighlightTilesForCharacter(CharacterStateManager character)
    {
        ClearHighlights();

        Vector3Int origin = character.MoveDestination;
        int movement = character.CharacterData.Movement;

        foreach (var tilePair in GameTileDictionary)
        {
            Vector2Int tilePos2D = tilePair.Key;
            int distance = Mathf.Abs(tilePos2D.x - origin.x) + Mathf.Abs(tilePos2D.y - origin.y);

            if (distance <= movement)
            {
                HighlightTile(tilePair.Value);
            }
        }
    }

    private void HighlightTile(GameObject tile)
    {
        if (tile == null || HighlightPrefab == null) return;

        // Instantiate highlight on top of tile
        GameObject highlight = Instantiate(
            HighlightPrefab,
            tile.transform.position,
            Quaternion.identity,
            tile.transform); // parented to tile

        // Set sorting layer/order
        SpriteRenderer sr = highlight.GetComponent<SpriteRenderer>();
        SpriteRenderer tileSR = tile.GetComponent<SpriteRenderer>();
        if (sr != null && tileSR != null)
        {
            sr.sortingLayerName = HighlightSortingLayer;
            sr.sortingOrder = tileSR.sortingOrder + SortingOrderAboveTile;
        }

        HighlightedPath.Add(highlight);
    }

    /// <summary>
    /// Removes all highlights
    /// </summary>
    public void ClearHighlights()
    {
        foreach (var highlight in HighlightedPath)
        {
            if (highlight != null)
                Destroy(highlight);
        }
        HighlightedPath.Clear();
    }

    /// <summary>
    /// Returns the GameTile at a given grid position (Vector3Int)
    /// </summary>
    public GameTile GetTileAt(Vector3Int position)
    {
        GameTileDictionary.TryGetValue(new Vector2Int(position.x, position.y), out GameObject tile);
        return tile?.GetComponent<GameTile>();
    }

        /// <summary>
    /// Finds adjacent enemy characters (cardinal directions)
    /// </summary>
    public List<GameObject> GetAdjacentEnemies(CharacterStateManager character)
    {
        Vector3Int pos = character.MoveDestination;
        List<GameObject> enemies = new List<GameObject>();

        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0,1,0),
            new Vector3Int(0,-1,0),
            new Vector3Int(1,0,0),
            new Vector3Int(-1,0,0)
        };

        foreach (Vector3Int dir in directions)
        {
            GameTile tile = GetTileAt(pos + dir);
            if (tile != null && tile.OccupyingCharacter != null)
            {
                CharacterGameData data = tile.OccupyingCharacter.GetComponent<CharacterGameData>();
                // Only add characters that are on a different team
                if (data.Team != character.CharacterData.Team)
                    enemies.Add(tile.OccupyingCharacter);
            }
        }

        return enemies;
    }

    public GameObject GetTileObjectAtCell(Vector3Int cellPos)
    {
        foreach (var tile in allGameTiles)
        {
            Vector3Int tileCellPos = tile.CellPosition;

            if (tileCellPos == cellPos)
            {
                return tile.gameObject;
            }
        }

        return null;
    }

}
