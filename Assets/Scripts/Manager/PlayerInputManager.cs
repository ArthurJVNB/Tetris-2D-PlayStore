using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.InputSystem.EnhancedTouch;

public enum Direction
{
    Left,
    Right
}

public class PlayerInputManager : MonoBehaviour
{
    //public static Action<Direction> OnSidewaysPressed;
    //public static Action OnDownBeingPressed;

    private static bool leftPressed;
    private static bool rightPressed;
    private static bool downPressed;
    private static bool rotateClockwisePressed;
    private static bool rotateAnticlockwisePressed;

    //private void OnEnable()
    //{
    //    EnhancedTouchSupport.Enable();
    //}

    //private void OnDisable()
    //{
    //    EnhancedTouchSupport.Disable();
    //}

    //private void Update()
    //{
    //    if (IsLeftPressed)
    //    {
    //        OnSidewaysPressed(Direction.Left);
    //    }

    //    if (IsRightPressed)
    //    {
    //        OnSidewaysPressed(Direction.Right);
    //    }

    //    //if (IsDownBeingPressed)
    //    //{
    //    //    OnDownPressing();
    //    //}
    //}

    #region Static Methods
    public static bool IsLeftPressed
    {
        get
        {
            if (leftPressed || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                leftPressed = false;
                return true;
            }

            return false;
        }
    }

    public static bool IsRightPressed
    {
        get
        {
            if (rightPressed || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                rightPressed = false;
                return true;
            }

            return false;
        }
    }

    public static bool IsDownPressed
    {
        get
        {
            if (downPressed || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                return true;
            }

            return false;
        }
    }

    public static bool IsRotateClockwisePressed
    {
        get
        {
            if (rotateClockwisePressed || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                rotateClockwisePressed = false;
                return true;
            }

            return false;
        }
    }

    public static bool IsRotateAnticlockwisePressed
    {
        get
        {
            if (rotateAnticlockwisePressed || Input.GetKeyDown(KeyCode.Q))
            {
                rotateAnticlockwisePressed = false;
                return true;
            }

            return false;
        }
    }
    #endregion

    private void Awake()
    {
        leftPressed = false;
        rightPressed = false;
        downPressed = false;
        rotateClockwisePressed = false;
        rotateAnticlockwisePressed = false;
    }

    public void SetLeftInputState()
    {
        leftPressed = true;
    }

    public void SetRightInputState()
    {
        rightPressed = true;
    }

    public void SetDownInputState(bool state)
    {
        downPressed = state;
    }

    public void SetRotate(bool clockwise)
    {
        if (clockwise)
            rotateClockwisePressed = true;
        else
            rotateAnticlockwisePressed = true;
    }

    //private void SidewaysPressed(Direction direction)
    //{
    //    OnSidewaysPressed?.Invoke(direction);
    //}

    //public void OnDownPressing()
    //{
    //    OnDownBeingPressed?.Invoke();
    //}
}
