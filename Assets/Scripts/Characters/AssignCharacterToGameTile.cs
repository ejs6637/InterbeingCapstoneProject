using UnityEngine;
using UnityEngine.Tilemaps;

public class AssignCharacterToGameTile : MonoBehaviour
{
    public Tilemap gridTilemap; // Assign your Grid's Tilemap in the Inspector
    public Vector3Int cellPosition; // Assign the tile cell coordinate in Inspector
    public float zOffset = 0.25f; // Optional: render above the tile

    void Start()
    {
        if (gridTilemap == null)
        {
            Debug.LogError($"No Tilemap assigned for {gameObject.name}!");
            Destroy(this);
            return;
        }

        // Check if a tile exists at the chosen cell
        TileBase tile = gridTilemap.GetTile(cellPosition);
        if (tile == null)
        {
            Debug.LogError($"No Tile detected under {gameObject.name} at cell {cellPosition}");
            Destroy(this);
            return;
        }

        // Snap to the tile's world position
        Vector3 tileWorldPos = gridTilemap.GetCellCenterWorld(cellPosition);
        transform.position = tileWorldPos + new Vector3(0, 0, zOffset);

        // Assign the character to the GameTile object
        GameObject tileObj = GameTileTracker.Instance.GetTileObjectAtCell(cellPosition);
        if (tileObj != null)
        {
            GameTile tileComp = tileObj.GetComponent<GameTile>();
            if (tileComp != null)
                tileComp.OccupyingCharacter = this.gameObject;
        }

        Destroy(this); // Remove script after assignment
    }
}
