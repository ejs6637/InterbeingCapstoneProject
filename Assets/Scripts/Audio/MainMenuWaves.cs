using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        audioSource.Play(); // plays immediately when scene starts
    }
}
