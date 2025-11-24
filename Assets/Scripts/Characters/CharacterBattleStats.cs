using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterStats baseStats;

    [HideInInspector] public int currentHealth;
    [HideInInspector] public int attack;
    [HideInInspector] public int defense;
    [HideInInspector] public int speed;

    void Start()
    {
        // Copy stats from ScriptableObject
        currentHealth = baseStats.maxHealth;
        attack = baseStats.attack;
        defense = baseStats.defense;
        speed = baseStats.speed;
    }

    public void TakeDamage(int dmg)
    {
        int finalDamage = Mathf.Max(0, dmg - defense);
        currentHealth -= finalDamage;

        Debug.Log($"{baseStats.characterName} took {finalDamage} damage!");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{baseStats.characterName} has died.");
        Destroy(gameObject);
    }
}
