using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LogoSceneFader : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image logoImage;          // Your logo
    public float fadeInDuration = 1f;
    public float holdDuration = 2f;
    public float fadeOutDuration = 1f;

    [Header("Background Settings")]
    public Image blackBackground;    // Black background behind logo

    [Header("Scene Settings")]
    public string nextSceneName = "MainMenuScene"; // Main menu scene

    private void Start()
    {
        if (logoImage == null || blackBackground == null)
        {
            Debug.LogError("Assign logoImage and blackBackground!");
            return;
        }

        logoImage.gameObject.SetActive(true);
        blackBackground.gameObject.SetActive(true);

        // Set initial alphas
        logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, 0f);
        blackBackground.color = Color.black;

        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // Fade in logo
        yield return Fade(logoImage, 1f, fadeInDuration);

        // Hold logo
        yield return new WaitForSeconds(holdDuration);

        // Fade out logo (screen remains black)
        yield return Fade(logoImage, 0f, fadeOutDuration);

        // Small delay before loading MainMenu
        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator Fade(Image image, float targetAlpha, float duration)
    {
        float startAlpha = image.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
    }
}
