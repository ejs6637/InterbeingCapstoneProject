using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    [Header("Canvases")]
    public Canvas mainMenuCanvas;     // Assign your Main Menu Canvas
    public Canvas settingsCanvas;     // Assign your Settings Canvas

    [Header("Audio")]
    public AudioMixer audioMixer;     // Assign your AudioMixer

    private Resolution[] resolutions;

    void Start()
    {
        if (mainMenuCanvas == null || settingsCanvas == null)
            Debug.LogError("SettingsManager: Canvas references are not assigned!");

        // Populate resolution dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Load saved settings
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);

        volumeSlider.value = savedVolume;
        fullscreenToggle.isOn = savedFullscreen;
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        Screen.fullScreen = savedFullscreen;
        SetResolution(savedResolutionIndex);
        SetVolume(savedVolume);
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    public void SetVolume(float volume)
    {
        if (audioMixer != null)
        {
            // If slider is at 0, mute hard
            if (volume <= 0.0001f)
            {
                audioMixer.SetFloat("MasterVolume", -80f); // Unity near-silence
            }
            else
            {
                audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
            }
        }

        PlayerPrefs.SetFloat("Volume", volume);
    }


    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void ResetToDefault()
    {
        float defaultVolume = 1f;
        bool defaultFullscreen = true;
        int defaultResolutionIndex = resolutions.Length - 1;

        volumeSlider.value = defaultVolume;
        fullscreenToggle.isOn = defaultFullscreen;
        resolutionDropdown.value = defaultResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        SetVolume(defaultVolume);
        SetFullscreen(defaultFullscreen);
        SetResolution(defaultResolutionIndex);
    }

    // Updated Back button
    public void BackToMainMenu()
    {
        if (mainMenuCanvas != null && settingsCanvas != null)
        {
            settingsCanvas.gameObject.SetActive(false);
            mainMenuCanvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("SettingsManager: Canvas references not assigned!");
        }
    }
}
