using UnityEngine;
using UnityEngine.Tilemaps;

public class AssignCharacterToGameTile : MonoBehaviour
{
    public Tilemap gridTilemap; // Assign in Inspector

    void Start()
    {
        Vector3Int cellPos = gridTilemap.WorldToCell(transform.position);
        TileBase tile = gridTilemap.GetTile(cellPos);

        if (tile != null)
        {
            GameObject tileObj = GameTileTracker.Instance.GetTileObjectAtCell(cellPos);

            if (tileObj != null)
            {
                transform.position = CharacterFunctions.GetCharacterPositionOnGameTile(tileObj);
                tileObj.GetComponent<GameTile>().OccupyingCharacter = this.gameObject;
            }
            else
            {
                Debug.LogError($"Tile exists but no GameTile object found at cell {cellPos}");
            }
        }
        else
        {
            Debug.LogError($"No Tile detected under {gameObject.name} at {cellPos}");
        }

        Destroy(this);
    }
}
