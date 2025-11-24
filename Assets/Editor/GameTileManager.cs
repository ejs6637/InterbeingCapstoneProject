using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Tool for spawning GameTiles from a Tilemap and assigning highlight sprites, colliders, and data
/// </summary>
public class GameTileManager : ScriptableWizard
{
    public Tilemap TilemapComponent;
    public List<GameTileType> GameTileTypes = new List<GameTileType>();

    [MenuItem("Tools/Game Tile Manager")]
    static void GameTileManagerWizard()
    {
        DisplayWizard<GameTileManager>("Game Tile Manager", "Destroy Game Tiles", "Regenerate Game Tiles");
    }

    private void Awake()
    {
        // Load Tilemap
        GameObject tilemapObj = GameObject.FindGameObjectWithTag(Constants.TilemapTag);
        if (tilemapObj == null)
        {
            Debug.LogError("No Tilemap found with the TilemapTag.");
            return;
        }
        TilemapComponent = tilemapObj.GetComponent<Tilemap>();

        // Ensure GameTileTracker exists
        if (tilemapObj.GetComponent<GameTileTracker>() == null)
            tilemapObj.AddComponent<GameTileTracker>();

        // Load GameTileTypes from assets
        string[] guids = AssetDatabase.FindAssets("t:GameTileType");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameTileTypes.Add(AssetDatabase.LoadAssetAtPath<GameTileType>(path));
        }
    }

    void OnWizardOtherButton() => RegenerateTiles();
    void OnWizardCreate() => DestroyGameTiles();

    private void RegenerateTiles()
    {
        DestroyGameTiles();
        CreateGameTiles();
    }

    private void CreateGameTiles()
    {
        if (TilemapComponent == null || GameTileTypes.Count == 0)
        {
            Debug.LogError("Tilemap or GameTileTypes missing.");
            return;
        }

        // Map TileBase -> GameTileType
        var tileMapping = new Dictionary<TileBase, GameTileType>();
        foreach (var type in GameTileTypes)
        {
            if (type.GameTileBase == null)
            {
                Debug.LogError($"GameTileType {type.name} has no TileBase assigned.");
                return;
            }
            tileMapping[type.GameTileBase] = type;
        }

        // Compress bounds
        TilemapComponent.CompressBounds();
        var bounds = TilemapComponent.cellBounds;

        // Iterate tiles
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int z = bounds.zMax - 1; z >= bounds.zMin; z--) // top-down
                {
                    Vector3Int cellPos = new Vector3Int(x, y, z);
                    TileBase tileBase = TilemapComponent.GetTile(cellPos);
                    if (tileBase == null) continue;
                    if (!tileMapping.ContainsKey(tileBase))
                    {
                        Debug.LogError($"No GameTileType for tile at {cellPos}");
                        DestroyGameTiles();
                        return;
                    }

                    GameTileType tileType = tileMapping[tileBase];

                    // Create Tile GameObject
                    GameObject tileGO = new GameObject($"GameTile X:{x} Y:{y} Z:{z}");
                    tileGO.tag = Constants.GameTileTag;
                    tileGO.transform.parent = TilemapComponent.transform;

                    GameTile gameTile = tileGO.AddComponent<GameTile>();
                    gameTile.GameTileType = tileType;
                    gameTile.CellPositionX = x;
                    gameTile.CellPositionY = y;
                    gameTile.CellPositionZ = z;

                    // Set world position
                    Vector3 worldPos = TilemapComponent.CellToWorld(cellPos);
                    tileGO.transform.position = new Vector3(
                        worldPos.x + TilemapComponent.orientationMatrix[0, 3],
                        worldPos.y + TilemapComponent.orientationMatrix[1, 3],
                        worldPos.z + TilemapComponent.orientationMatrix[2, 3]);

                    // Collider
                    PolygonCollider2D collider = tileGO.AddComponent<PolygonCollider2D>();
                    var sprite = TilemapComponent.GetSprite(cellPos);
                    collider.pathCount = sprite.GetPhysicsShapeCount();
                    List<Vector2> points = new List<Vector2>();
                    for (int i = 0; i < collider.pathCount; i++)
                    {
                        sprite.GetPhysicsShape(i, points);
                        // Flip X if rotated
                        if (TilemapComponent.GetTransformMatrix(cellPos).rotation.eulerAngles.y >= 180)
                        {
                            for (int j = 0; j < points.Count; j++)
                                points[j] = new Vector2(-points[j].x, points[j].y);
                        }
                        collider.SetPath(i, points);
                    }

                    // Highlight as child
                    GameObject highlightGO = new GameObject("Highlight");
                    highlightGO.transform.parent = tileGO.transform;
                    highlightGO.transform.localPosition = Vector3.zero;

                    SpriteRenderer highlightRenderer = highlightGO.AddComponent<SpriteRenderer>();
                    highlightRenderer.sprite = tileType.GameTileHighlight;
                    highlightRenderer.enabled = false;
                    highlightRenderer.sortingLayerName = "Tiles"; // Make sure your tile layer exists
                    highlightRenderer.sortingOrder = 10; // Above base tiles
                    highlightRenderer.spriteSortPoint = SpriteSortPoint.Pivot;

                    // Optional: flip for inclined tiles
                    if (gameTile.SurfaceOrientation == TileSurfaceOrientation.InclinedDownToRight)
                        highlightRenderer.flipX = true;

                    // Only create one tile per cell
                    break;
                }
            }
        }
    }

    private void DestroyGameTiles()
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag(Constants.GameTileTag))
            DestroyImmediate(obj);
    }
}
