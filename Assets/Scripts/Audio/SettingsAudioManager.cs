using UnityEngine;

public class SettingsAudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    // Call this when settings panel opens
    public void PlayAudio()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    // Call this when settings panel closes
    public void StopAudio()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
    }
}
