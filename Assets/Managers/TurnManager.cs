using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Player Prefabs")]
    public List<CharacterStateManager> playerPrefabs;    // Assign character prefabs
    public List<Transform> playerSpawnPoints;            // Empty GameObjects as spawn points

    [Header("Enemy Prefab")]
    public CharacterStateManager enemyPrefab;
    public Transform enemySpawnPoint;

    [Header("UI")]
    public List<GameObject> playerUIs;                  // One panel per player character
    public GameObject enemyUI;                           // Assign enemy UI panel
    public GameObject actionMenu;                        // Shared action menu
    public GameObject damagePopupPrefab;                 // Floating damage numbers

    [Header("Tilemap")]
    public Tilemap gridTilemap;                         // Assign your tilemap
    public Vector3 zOffset = new Vector3(0f, 0f, 0.25f);

    [HideInInspector]
    public List<CharacterStateManager> allCharacters = new List<CharacterStateManager>();
    [HideInInspector]
    public CharacterStateManager enemyCharacter;

    private int currentTurnIndex = 0;
    private CharacterStateManager currentCharacter;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (allCharacters == null) allCharacters = new List<CharacterStateManager>();
    }

    private IEnumerator Start()
    {
        // Wait until GameTileTracker exists
        while (GameTileTracker.Instance == null)
            yield return null;

        // Spawn characters safely
        SpawnCharactersAtAssignedPositions();

        // Begin first turn
        BeginPlayerTurn();
    }

    private void SpawnCharactersAtAssignedPositions()
    {
        allCharacters = new List<CharacterStateManager>();

        // Spawn Players
        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            if (playerPrefabs[i] == null || i >= playerSpawnPoints.Count)
            {
                Debug.LogError($"Missing prefab or spawn point for player index {i}");
                continue;
            }

            var player = Instantiate(playerPrefabs[i], playerSpawnPoints[i].position, Quaternion.identity);
            SnapCharacterToTile(player);
            allCharacters.Add(player);

            Debug.Log($"Spawned player {player.name} at {player.transform.position}");
        }

        // Spawn Enemy
        if (enemyPrefab != null && enemySpawnPoint != null)
        {
            enemyCharacter = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);
            SnapCharacterToTile(enemyCharacter);
            Debug.Log($"Spawned enemy {enemyCharacter.name} at {enemyCharacter.transform.position}");
        }
    }

   private void SnapCharacterToTile(CharacterStateManager character)
{
    if (character == null || GameTileTracker.Instance == null || gridTilemap == null) return;

    Vector3Int cellPos = gridTilemap.WorldToCell(character.transform.position);
    GameTile tile = GameTileTracker.Instance.GetTileAt(cellPos);

    if (tile != null)
    {
        // Snap position
        character.transform.position = tile.transform.position + zOffset;

        // Set MoveDestination to this tile
        character.MoveDestination = tile.CellPosition;
        Debug.Log($"{character.name} snapped to tile at {tile.CellPosition}");
    }
    else
    {
        Debug.LogWarning($"{character.name} could not find a tile at {character.transform.position} (cell {cellPos})");
        character.gameObject.SetActive(false); // optionally disable it
    }
}


    #region Player Turn
    public void BeginPlayerTurn()
    {
        currentTurnIndex = 0;

        // Hide enemy UI
        if (enemyUI != null)
            enemyUI.SetActive(false);

        // Hide all player UIs
        foreach (var ui in playerUIs)
            ui.SetActive(false);

        ShowCurrentPlayerUI();
    }

    private void ShowCurrentPlayerUI()
    {
        foreach (var ui in playerUIs)
            ui.SetActive(false);

        if (allCharacters.Count == 0) return;

        currentCharacter = allCharacters[currentTurnIndex];

        // Show only current character's UI
        if (currentTurnIndex < playerUIs.Count)
            playerUIs[currentTurnIndex].SetActive(true);

        if (actionMenu != null)
            actionMenu.SetActive(true);

        UpdateCharacterUI(currentCharacter, playerUIs[currentTurnIndex]);
    }

    private void UpdateCharacterUI(CharacterStateManager character, GameObject uiPanel)
    {
        if (character == null || character.CharacterData == null || uiPanel == null)
            return;

        var portrait = uiPanel.transform.Find("Portrait")?.GetComponent<UnityEngine.UI.Image>();
        var nameText = uiPanel.transform.Find("NameText")?.GetComponent<TMPro.TMP_Text>();
        var hpText = uiPanel.transform.Find("HPText")?.GetComponent<TMPro.TMP_Text>();

        if (portrait != null && character.CharacterData.CharacterSpriteRenderer != null)
            portrait.sprite = character.CharacterData.CharacterSpriteRenderer.sprite;

        if (nameText != null)
            nameText.text = character.CharacterData.CharacterName;

        if (hpText != null)
        {
            hpText.text = $"HP: {character.CharacterData.Health}/{character.CharacterData.MaxHealth}";
            hpText.color = (character.CharacterData.Health < character.CharacterData.MaxHealth * 0.3f) ? Color.red : Color.white;
        }
    }

    private GameObject GetUIPanelForCharacter(CharacterStateManager character)
    {
        if (character == enemyCharacter)
            return enemyUI;

        int index = allCharacters.IndexOf(character);
        if (index >= 0 && index < playerUIs.Count)
            return playerUIs[index];

        return null;
    }

    public void OnTileClicked(GameObject clickedTileObject)
    {
        if (clickedTileObject == null || currentCharacter == null)
            return;

        GameTile tile = clickedTileObject.GetComponent<GameTile>();
        if (tile == null) return;

        currentCharacter.transform.position = tile.transform.position + zOffset;
        currentCharacter.MoveDestination = tile.CellPosition;

        GameTileTracker.Instance.ClearHighlights();
        EndMovementPhase(currentCharacter);
    }



    public void OnMoveButton()
    {
        if (currentCharacter == null) return;

        // Make sure character is active and snapped
        if (!currentCharacter.CharacterData.CharacterActive)
            currentCharacter.CharacterData.CharacterActive = true;

        // Highlight tiles
        GameTileTracker.Instance.HighlightTilesForCharacter(currentCharacter);
        Debug.Log($"{currentCharacter.CharacterData.CharacterName} can now move");
    }



    public void ConfirmMove()
    {
        currentCharacter.CharacterData.CharacterActive = false;
        GameTileTracker.Instance.ClearHighlights();
        EndMovementPhase(currentCharacter);
    }

    public void CancelMove()
    {
        currentCharacter.CharacterData.CharacterActive = false;
        GameTileTracker.Instance.ClearHighlights();
        EndMovementPhase(currentCharacter);
    }

    public void OnFightButton()
    {
        int damage = Random.Range(10, 20);
        ApplyDamage(enemyCharacter, damage);

        EndActionPhase(currentCharacter);
    }

    public void OnWaitButton()
    {
        EndActionPhase(currentCharacter);
    }

    private void EndPlayerTurn()
    {
        currentTurnIndex++;
        if (currentTurnIndex >= allCharacters.Count)
            BeginEnemyTurn();
        else
            ShowCurrentPlayerUI();
    }
    #endregion

    #region Enemy Turn
    private void BeginEnemyTurn()
    {
        currentCharacter = enemyCharacter;

        // Hide all player UIs
        foreach (var ui in playerUIs)
            ui.SetActive(false);

        if (actionMenu != null)
            actionMenu.SetActive(false);

        if (enemyUI != null)
            enemyUI.SetActive(true);

        Debug.Log($"{enemyCharacter.CharacterData.CharacterName}'s turn started (Enemy)");
        Invoke(nameof(EnemyMove), 1f);
    }

    private void EnemyMove()
    {
        GameTileTracker.Instance.HighlightTilesForCharacter(enemyCharacter);
        Invoke(nameof(ClearEnemyHighlights), 1f);
    }

    private void ClearEnemyHighlights()
    {
        GameTileTracker.Instance.ClearHighlights();
        EnemyAction();
    }

    private void EnemyAction()
    {
        var adjacentPlayers = GameTileTracker.Instance.GetAdjacentEnemies(enemyCharacter);

        if (adjacentPlayers.Count > 0)
        {
            var target = adjacentPlayers[Random.Range(0, adjacentPlayers.Count)];
            ApplyDamage(target.GetComponent<CharacterStateManager>(), Random.Range(5, 15));
            Debug.Log($"{enemyCharacter.CharacterData.CharacterName} attacked {target.GetComponent<CharacterStateManager>().CharacterData.CharacterName}");
        }

        Invoke(nameof(EndEnemyTurn), 1f);
    }

    public void EndEnemyTurn()
    {
        BeginPlayerTurn();
    }
    #endregion

    #region Damage
    private void ShowDamage(CharacterGameData target, int damage)
    {
        if (damagePopupPrefab == null || target == null) return;

        GameObject popup = Instantiate(damagePopupPrefab, target.transform.position, Quaternion.identity);
        var dmgScript = popup.GetComponent<DamagePopup>();
        if (dmgScript != null) dmgScript.SetDamage(damage);
    }

    private void ApplyDamage(CharacterStateManager character, int damage)
    {
        if (character == null || character.CharacterData == null) return;

        character.CharacterData.Health -= damage;
        if (character.CharacterData.Health < 0)
            character.CharacterData.Health = 0;

        ShowDamage(character.CharacterData, damage);

        // Update UI immediately
        GameObject uiPanel = GetUIPanelForCharacter(character);
        UpdateCharacterUI(character, uiPanel);
    }
    #endregion

    #region Phase Methods
    public void EndMovementPhase(CharacterStateManager character)
    {
        Debug.Log($"{character.CharacterData.CharacterName} finished moving. Action phase ready.");
        if (actionMenu != null)
            actionMenu.SetActive(true);
    }

    public void EndActionPhase(CharacterStateManager character)
    {
        Debug.Log($"{character.CharacterData.CharacterName} finished action. Ending turn...");
        EndPlayerTurn();
    }
    #endregion
}
