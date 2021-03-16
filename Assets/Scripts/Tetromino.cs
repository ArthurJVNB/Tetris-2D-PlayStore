using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour, IDestroyable
{
    public static event Action OnStart;
    public static event Action OnStrafeMovement;
    public static event Action OnDownMovement;
    public static event Action OnRotation;
    public static event Action<Tetromino> OnEnd;

    private const float FASTER = .9f;

    public Transform[] Blocks { get; private set; }

#if UNITY_EDITOR
    [SerializeField] private bool _debugPivot;
#endif

    [SerializeField] private float timeToMoveDown;
    [SerializeField] private Vector3 pivot;

    #region Audio Config
    [Header("Audio")]
    [SerializeField] private AudioClip strafeMovementAudio;
    [Range(0, 1)]
    [SerializeField] private float strafeAudioVolume = 1f;

    [Space]
    [SerializeField] private AudioClip downMovementAudio;
    [Range(0, 1)]
    [SerializeField] private float downAudioVolume = 1f;
    [Tooltip("Play down movement audio?")]
    [SerializeField] private bool willPlayDownMovementAudio = false;

    [Space]
    [SerializeField] private AudioClip rotationAudio;
    [Range(0, 1)]
    [SerializeField] private float rotationAudioVolume = 1f;

    [Space]
    [SerializeField] private AudioClip endAudio;
    [Range(0, 1)]
    [SerializeField] private float endAudioVolume = 1f;
    #endregion

    private float timeNextMoveDown;
    private GameplayManager gameplay;
    private IAudioPlayer audioPlayer;

    public void Initialize(float timeToMoveDown, GameplayManager gameplayInstance)
    {
        this.timeToMoveDown = timeToMoveDown;
        this.gameplay = gameplayInstance;

        //SubscribeToMyEvents();

        UpdateNextTimeToMoveDown();
        OnStart?.Invoke();
        enabled = true;
    }

    public void Destroy()
    {
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        if (audioPlayer != null)
        {
            yield return new WaitForSeconds(timeToMoveDown);
            while (audioPlayer.IsPlaying())
            {
                yield return null;
            }
        }

        //UnsubscribeFromMyEvents();
        Destroy(gameObject);
    }

    private void FinalizeMovement()
    {
        OnEnd?.Invoke(this);

        //UnsubscribeFromMyEvents();
        enabled = false;
    }

    private void MoveLeft()
    {
        transform.position += Vector3Int.left;
        if (!gameplay.IsMovementValid(Blocks))
            transform.position -= Vector3Int.left;

        OnStrafeMovement?.Invoke();
    }

    private void MoveRight()
    {
        transform.position += Vector3Int.right;
        if (!gameplay.IsMovementValid(Blocks))
            transform.position -= Vector3Int.right;

        OnStrafeMovement?.Invoke();
    }

    private void Rotate(bool clockwise)
    {
        transform.RotateAround(transform.TransformPoint(pivot), Vector3.forward, clockwise ? -90 : 90);
        if (!gameplay.IsMovementValid(Blocks))
            transform.RotateAround(transform.TransformPoint(pivot), Vector3.forward, clockwise ? 90 : -90);

        OnRotation?.Invoke();
    }

    private void MoveDown()
    {
        if (Time.time > (PlayerInputManager.IsDownPressed ? timeNextMoveDown - timeToMoveDown * FASTER : timeNextMoveDown))
        {
            UpdateNextTimeToMoveDown();
            transform.position += Vector3Int.down;

            if (!gameplay.IsMovementValid(Blocks))
            {
                transform.position -= Vector3Int.down;

                // MOVEMENT ENDED
                FinalizeMovement();
            }
            else
            {
                OnDownMovement?.Invoke();
            }
        }
    }

    private void UpdateNextTimeToMoveDown()
    {
        timeNextMoveDown = Time.time + timeToMoveDown;
    }

    private void SubscribeToMyEvents()
    {
        OnStart += Tetromino_OnStart;
        OnEnd += Tetromino_OnEnd;
        OnStrafeMovement += Tetromino_OnStrafeMovement;
        OnDownMovement += Tetromino_OnDownMovement;
        OnRotation += Tetromino_OnRotation;
    }


    private void UnsubscribeFromMyEvents()
    {
        OnStart -= Tetromino_OnStart;
        OnEnd -= Tetromino_OnEnd;
        OnStrafeMovement -= Tetromino_OnStrafeMovement;
        OnDownMovement -= Tetromino_OnDownMovement;
        OnRotation -= Tetromino_OnRotation;
    }

    private void Tetromino_OnStart()
    {
        Debug.LogWarning("Tetromino_OnStart not implemented");
    }

    private void Tetromino_OnEnd(Tetromino obj)
    {
        audioPlayer.Play(endAudio, endAudioVolume);
        Debug.Log("Playing endAudio");
    }

    private void Tetromino_OnStrafeMovement()
    {
        audioPlayer.Play(strafeMovementAudio, strafeAudioVolume);
    }
    private void Tetromino_OnDownMovement()
    {
        if (willPlayDownMovementAudio)
            audioPlayer.Play(downMovementAudio, downAudioVolume);
    }

    private void Tetromino_OnRotation()
    {
        audioPlayer.Play(rotationAudio, rotationAudioVolume);
    }

    private void Awake()
    {
        Blocks = Util.GetChildren(transform);
        audioPlayer = GetComponent<IAudioPlayer>();

        enabled = false;
    }

    private void OnEnable()
    {
        SubscribeToMyEvents();
    }

    private void Update()
    {
        if (PlayerInputManager.IsLeftPressed)
            MoveLeft();
        if (PlayerInputManager.IsRightPressed)
            MoveRight();
        if (PlayerInputManager.IsRotateClockwisePressed)
            Rotate(true);
        if (PlayerInputManager.IsRotateAnticlockwisePressed)
            Rotate(false);

        MoveDown();
    }

    private void OnDisable()
    {
        UnsubscribeFromMyEvents();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_debugPivot)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.TransformPoint(pivot), .5f);
        }
    }
#endif
}
