using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnTileClicked(gameObject);
    }
}
