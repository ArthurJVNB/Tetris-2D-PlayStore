using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerOneShot : MonoBehaviour, IAudioPlayer
{
    [SerializeField] private AudioSource audioSource;

    public void Play(AudioClip audio)
    {
        audioSource?.PlayOneShot(audio);
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
