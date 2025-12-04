using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;

    public void OpenSettings()
    {
        settingsPanel.SetActive(true); // show panel
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false); // hide panel
    }
}
