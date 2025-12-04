using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    public AudioSource audioSource; // Drag your AudioSource here
    public AudioClip clip;          // Drag your audio clip here

    // Play sound once
    public void PlaySound()
    {
        audioSource.PlayOneShot(clip);
    }
}
