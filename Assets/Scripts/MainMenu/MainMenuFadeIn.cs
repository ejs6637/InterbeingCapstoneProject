using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuFadeIn : MonoBehaviour
{
    public Image menuFadeImage;  // Black overlay in MainMenu
    public float fadeDuration = 1.5f;

    private void Start()
    {
        if (menuFadeImage != null)
        {
            menuFadeImage.gameObject.SetActive(true);
            menuFadeImage.color = Color.black; // start fully black
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        Color startColor = menuFadeImage.color;
        Color endColor = new Color(0, 0, 0, 0); // transparent

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            menuFadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            yield return null;
        }

        menuFadeImage.color = endColor;
        menuFadeImage.gameObject.SetActive(false); // optional: disable after fade
    }
}
