using UnityEngine;

public class DontDestroyTilemap : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
