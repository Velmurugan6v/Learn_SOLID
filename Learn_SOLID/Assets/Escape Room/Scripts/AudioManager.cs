using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    public void PlaySound(int clipIndex)
    {
        audioSource.PlayOneShot(audioClips[clipIndex]);
    }
}
