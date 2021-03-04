using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private const float FASTER = .9f;

    public static event Action<Tetromino> OnMovementEnded;

    public Transform[] Blocks { get; private set; }

    [SerializeField] private float timeToMoveDown;
    // TODO: pivot
    [SerializeField] private Vector3 pivot;
    [SerializeField] private bool _debugPivot;

    private float timeNextMoveDown;
    private GameplayManager gameplay;

    private void Awake()
    {
        Blocks = Util.GetChildren(transform);

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

    public void Initialize(float timeToMoveDown, GameplayManager gameplayInstance)
    {
        this.timeToMoveDown = timeToMoveDown;
        this.gameplay = gameplayInstance;

        // subscribe inputs
        //PlayerInputManager.OnSidewaysPressed += HandleMovement;
        //PlayerInputManager.OnDownBeingPressed += MoveDownFaster;

        UpdateNextTimeToMoveDown();
        enabled = true;
    }

    private void HandleMovement(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                MoveLeft();
                break;
            case Direction.Right:
                MoveRight();
                break;
            default:
                break;
        }
    }

    private void MoveLeft()
    {
        transform.position += Vector3Int.left;
        if (!gameplay.IsMovementValid(Blocks))
            transform.position -= Vector3Int.left;
    }

    private void MoveRight()
    {
        transform.position += Vector3Int.right;
        if (!gameplay.IsMovementValid(Blocks))
            transform.position -= Vector3Int.right;
    }

    public void Rotate(bool clockwise)
    {
        transform.RotateAround(transform.TransformPoint(pivot), Vector3.forward, clockwise ? -90 : 90);
        if (!gameplay.IsMovementValid(Blocks))
            transform.RotateAround(transform.TransformPoint(pivot), Vector3.forward, clockwise ? 90 : -90);
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
        }
    }

    private void UpdateNextTimeToMoveDown()
    {
        timeNextMoveDown = Time.time + timeToMoveDown;
    }

    private void FinalizeMovement()
    {
        enabled = false;

        // unsubscribe inputs
        //PlayerInputManager.OnSidewaysPressed -= HandleMovement;
        //PlayerInputManager.OnDownBeingPressed -= MoveDownFaster;

        OnMovementEnded?.Invoke(this);
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
