using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip mismatchClip;
    public AudioClip gameOverClip;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayFlip()
    {
        PlayClip(flipClip);
    }

    public void PlayMatch()
    {
        PlayClip(matchClip);
    }

    public void PlayMismatch()
    {
        PlayClip(mismatchClip);
    }

    public void PlayGameOver()
    {
        PlayClip(gameOverClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
