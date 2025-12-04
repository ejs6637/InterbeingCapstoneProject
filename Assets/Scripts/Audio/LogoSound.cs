using UnityEngine;

public class LogoSound : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        audioSource.Play(); // Plays immediately when scene starts
    }
}
