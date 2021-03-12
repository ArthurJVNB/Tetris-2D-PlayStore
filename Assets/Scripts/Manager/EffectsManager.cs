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
    [SerializeField] private AudioClip gameOverAudio;

    // TODO: Particle Effects

    private IAudioPlayer audioPlayer;

    public void PlayAudio(AudioType type)
    {
        switch (type)
        {
            case AudioType.RowClearing:
                audioPlayer?.Play(rowClearingAudio);
                break;
            case AudioType.GameOver:
                audioPlayer?.Play(gameOverAudio);
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
