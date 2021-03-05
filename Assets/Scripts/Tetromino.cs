using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public static event Action OnStart;
    public static event Action OnAnyMovement;
    public static event Action OnRotation;
    public static event Action<Tetromino> OnEnd;

    private const float FASTER = .9f;

    public Transform[] Blocks { get; private set; }

#if UNITY_EDITOR
    [SerializeField] private bool _debugPivot;
#endif

    [SerializeField] private float timeToMoveDown;
    [SerializeField] private Vector3 pivot;

    [Header("Audio")]
    [SerializeField] private AudioClip movementAudio;
    [SerializeField] private AudioClip rotationAudio;
    [SerializeField] private AudioClip endAudio;

    private float timeNextMoveDown;
    private GameplayManager gameplay;
    private IAudioPlayer audioPlayer;

    private void Awake()
    {
        Blocks = Util.GetChildren(transform);
        audioPlayer = GetComponent<IAudioPlayer>();

        enabled = false;
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

    private void SubscribeToMyEvents()
    {
        OnStart += Tetromino_OnStart;
        OnEnd += Tetromino_OnEnd;
        OnAnyMovement += Tetromino_OnAnyMovement;
        OnRotation += Tetromino_OnRotation;
    }

    private void UnsubscribeFromMyEvents()
    {
        OnStart -= Tetromino_OnStart;
        OnEnd -= Tetromino_OnEnd;
        OnAnyMovement -= Tetromino_OnAnyMovement;
        OnRotation -= Tetromino_OnRotation;
    }

    private void Tetromino_OnStart()
    {
        Debug.LogWarning("Tetromino_OnStart not implemented");
    }

    private void Tetromino_OnEnd(Tetromino obj)
    {
        audioPlayer.Play(endAudio);
        Debug.Log("Playing endAudio");
    }

    private void Tetromino_OnAnyMovement()
    {
        audioPlayer.Play(movementAudio);
    }

    private void Tetromino_OnRotation()
    {
        audioPlayer.Play(rotationAudio);
    }

    public void Initialize(float timeToMoveDown, GameplayManager gameplayInstance)
    {
        this.timeToMoveDown = timeToMoveDown;
        this.gameplay = gameplayInstance;

        SubscribeToMyEvents();

        UpdateNextTimeToMoveDown();
        OnStart?.Invoke();
        enabled = true;
    }

    private void MoveLeft()
    {
        transform.position += Vector3Int.left;
        if (!gameplay.IsMovementValid(Blocks))
            transform.position -= Vector3Int.left;

        OnAnyMovement?.Invoke();
    }

    private void MoveRight()
    {
        transform.position += Vector3Int.right;
        if (!gameplay.IsMovementValid(Blocks))
            transform.position -= Vector3Int.right;

        OnAnyMovement?.Invoke();
    }

    public void Rotate(bool clockwise)
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
                OnAnyMovement?.Invoke();
            }
        }
    }

    private void UpdateNextTimeToMoveDown()
    {
        timeNextMoveDown = Time.time + timeToMoveDown;
    }

    private void FinalizeMovement()
    {
        OnEnd?.Invoke(this);

        UnsubscribeFromMyEvents();
        enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (_debugPivot)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.TransformPoint(pivot), .5f);
        }
    }
}
