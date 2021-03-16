using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerOneShot : MonoBehaviour, IAudioPlayer
{
    [SerializeField] private AudioSource audioSource;

    public void Play(AudioClip audio)
    {
        Play(audio, 1);
    }

    public void Play(AudioClip audio, float volume)
    {
        if (audioSource)
            audioSource.PlayOneShot(audio, volume);
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    private void Awake()
    {
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }
}
