using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    public AudioSource audioSource; // Drag AudioSource here
    public AudioClip clip;          // Drag audio clip here

    // Play sound once
    public void PlaySound()
    {
        audioSource.PlayOneShot(clip);
    }
}
