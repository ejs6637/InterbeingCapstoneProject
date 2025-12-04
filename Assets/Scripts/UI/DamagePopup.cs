using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TMPro.TMP_Text damageText;

    public void SetDamage(int amount)
    {
        damageText.text = amount.ToString();
        // Play animation (assumes Animator attached)
        var animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("Play");

        // Destroy after animation length
        Destroy(gameObject, 1f);
    }
}
