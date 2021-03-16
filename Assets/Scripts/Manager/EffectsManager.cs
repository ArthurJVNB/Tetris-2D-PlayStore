using UnityEngine;
using System.Collections;

public enum AudioType
{
    RowClearing,
    GameOver
}

public class EffectsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip rowClearingAudio;
    [Range(0, 1)]
    [SerializeField] private float rowAudioVolume = 1f;

    [SerializeField] private AudioClip gameOverAudio;
    [Range(0, 1)]
    [SerializeField] private float gameOverAudioVolume = 1f;

    // TODO: Particle Effects

    private IAudioPlayer audioPlayer;

    public void PlayAudio(AudioType type)
    {
        switch (type)
        {
            case AudioType.RowClearing:
                audioPlayer?.Play(rowClearingAudio, rowAudioVolume);
                break;
            case AudioType.GameOver:
                audioPlayer?.Play(gameOverAudio, gameOverAudioVolume);
                break;
            default:
                break;
        }
    }

    private void Awake()
    {
        if (audioPlayer == null)
            audioPlayer = GetComponent<IAudioPlayer>();
    }
}
