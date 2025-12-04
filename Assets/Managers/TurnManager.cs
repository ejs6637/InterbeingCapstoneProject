using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Characters")]
    public List<CharacterStateManager> allCharacters; // player + enemy characters
    public CharacterStateManager enemyCharacter;      // single enemy

    [Header("UI")]
    public List<GameObject> playerUIs;        // One panel per player character
    public GameObject actionMenu;             // Shared action menu
    public GameObject damagePopupPrefab;      // Prefab for floating damage numbers

    private int currentTurnIndex = 0;
    private CharacterStateManager currentCharacter;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        BeginPlayerTurn();
    }

    #region Player Turn
    public void BeginPlayerTurn()
    {
        currentTurnIndex = 0;

        // Hide enemy UI
        if (enemyCharacter != null && enemyCharacter.CharacterUI != null)
            enemyCharacter.CharacterUI.SetActive(false);

        // Hide all player UIs first
        foreach (var ui in playerUIs)
            ui.SetActive(false);

        ShowCurrentPlayerUI();
        Debug.Log($"{currentCharacter.CharacterData.name}'s turn started (Player)");
    }


    void ShowCurrentPlayerUI()
    {
        // Hide all character UIs
        foreach (var ui in playerUIs)
            ui.SetActive(false);

        // Set current character
        currentCharacter = allCharacters[currentTurnIndex];

        // Show only current character's UI
        playerUIs[currentTurnIndex].SetActive(true);

        // Show action menu for player input
        if (actionMenu != null)
            actionMenu.SetActive(true);

        UpdateCharacterUI(currentCharacter);
    }

    void UpdateCharacterUI(CharacterStateManager character)
    {
        if (character == null || character.CharacterData == null)
        {
            Debug.LogError("Character or CharacterData is null!");
            return;
        }

        var ui = playerUIs.Count > currentTurnIndex ? playerUIs[currentTurnIndex] : null;
        if (ui == null)
        {
            Debug.LogError("Player UI panel not assigned for index " + currentTurnIndex);
            return;
        }

        var portrait = ui.transform.Find("Portrait")?.GetComponent<UnityEngine.UI.Image>();
        var nameText = ui.transform.Find("NameText")?.GetComponent<TMPro.TMP_Text>();
        var hpText = ui.transform.Find("HPText")?.GetComponent<TMPro.TMP_Text>();

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


    public void OnMoveButton()
    {
        if (currentCharacter.CharacterData.CharacterActive) return;

        currentCharacter.CharacterData.CharacterActive = true;
        // Enable movement input for this character
        GameTileTracker.Instance.HighlightTilesForCharacter(currentCharacter);
        Debug.Log($"{currentCharacter.CharacterData.name} can now move");
    }

    public void ConfirmMove()
    {
        currentCharacter.CharacterData.CharacterActive = false;
        GameTileTracker.Instance.ClearHighlights();
        Debug.Log($"{currentCharacter.CharacterData.name} finished movement");
    }

    public void CancelMove()
    {
        currentCharacter.CharacterData.CharacterActive = false;
        GameTileTracker.Instance.ClearHighlights();
        Debug.Log($"{currentCharacter.CharacterData.name} canceled movement");
    }

    public void OnFightButton()
    {
        int damage = Random.Range(10, 20);

        enemyCharacter.CharacterData.Health -= damage;
        if (enemyCharacter.CharacterData.Health < 0) enemyCharacter.CharacterData.Health = 0;

        ShowDamage(enemyCharacter.CharacterData, damage);
        UpdateCharacterUI(enemyCharacter);

        EndPlayerTurn();
    }

    public void OnWaitButton()
    {
        Debug.Log($"{currentCharacter.CharacterData.name} chose WAIT");
        EndPlayerTurn();
    }

    void EndPlayerTurn()
    {
        playerUIs[currentTurnIndex].SetActive(true);

        currentTurnIndex++;
        if (currentTurnIndex >= allCharacters.Count)
            BeginEnemyTurn();
        else
            ShowCurrentPlayerUI();
    }
    #endregion

    #region Enemy Turn
    void BeginEnemyTurn()
    {
        currentCharacter = enemyCharacter;

        // Hide all player UIs
        foreach (var ui in playerUIs)
        {
            if (ui != null)
                ui.SetActive(false);
        }

        // Hide the shared action menu
        if (actionMenu != null)
            actionMenu.SetActive(false);

        // No need to hide enemy UI if you don’t have one
        // Just proceed with enemy AI actions
        Debug.Log($"{enemyCharacter.CharacterData.CharacterName}'s turn started (Enemy)");

        Invoke(nameof(EnemyMove), 1f);
    }



    void EnemyMove()
    {
        GameTileTracker.Instance.HighlightTilesForCharacter(enemyCharacter);
        // TODO: implement AI pathing
        // Only clear highlights after movement is done
        Invoke(nameof(ClearEnemyHighlights), 1f);
    }

    void ClearEnemyHighlights()
    {
        GameTileTracker.Instance.ClearHighlights();
        EnemyAction();
    }


    void EnemyAction()
    {
        var adjacentPlayers = GameTileTracker.Instance.GetAdjacentEnemies(enemyCharacter);

        if (adjacentPlayers.Count > 0)
        {
            var target = adjacentPlayers[Random.Range(0, adjacentPlayers.Count)];
            var targetData = target.GetComponent<CharacterStateManager>().CharacterData;

            int damage = Random.Range(5, 15);
            targetData.Health -= damage;
            if (targetData.Health < 0) targetData.Health = 0;

            ShowDamage(targetData, damage);
            Debug.Log($"{enemyCharacter.CharacterData.name} attacked {targetData.name}");
        }

        Invoke(nameof(EndEnemyTurn), 1f);
    }

    public void EndEnemyTurn()
    {
        Debug.Log($"{enemyCharacter.CharacterData.name}'s turn ended");
        BeginPlayerTurn();
    }
    #endregion

    #region Damage Popup
    void ShowDamage(CharacterGameData target, int damage)
    {
        if (damagePopupPrefab == null || target == null) return;

        GameObject popup = Instantiate(damagePopupPrefab, target.transform.position, Quaternion.identity);
        var dmgScript = popup.GetComponent<DamagePopup>();
        if (dmgScript != null) dmgScript.SetDamage(damage);
    }
    #endregion

    public void EndMovementPhase(CharacterStateManager character)
    {
        Debug.Log($"{character.CharacterData.CharacterName} finished moving. Switching to action phase...");
        // Here you could enable the action menu, or call a method on the character
        character.SetActionPhase();
    }

    public void EndActionPhase(CharacterStateManager character)
    {
        Debug.Log($"{character.CharacterData.CharacterName} finished action. Ending turn...");
        character.EndTurn();
    }

}
