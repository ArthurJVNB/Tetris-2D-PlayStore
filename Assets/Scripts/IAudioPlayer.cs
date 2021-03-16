using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioPlayer
{
    void Play(AudioClip audio);
    void Play(AudioClip audio, float volume);
    bool IsPlaying();
}
