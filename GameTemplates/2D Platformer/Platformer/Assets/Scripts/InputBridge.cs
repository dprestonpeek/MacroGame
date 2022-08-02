using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBridge : MonoBehaviour
{
    /// <summary>
    /// Instance of our Singleton
    /// </summary>
    public static InputBridge Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputBridge>();
                if (_instance == null)
                {
                    _instance = new GameObject("InputBridge").AddComponent<InputBridge>();
                }
            }
            return _instance;
        }
    }
    public enum Input { Gamepad, Keyboard };
    public Input input = Input.Keyboard;
    public int playerId = 0;

    private static InputBridge _instance;
    private Gamepad gamepad;
    private Keyboard keyboard;

    int m_StickId;

    bool initialized = false;
    bool pausedThisFrame = false;
    bool unpausedThisFrame = false;

    private void Update()
    {
        if (!initialized)
            Initialize();
        pausedThisFrame = false;
        unpausedThisFrame = false;

        Thumbsticks();
        DPad();
        Buttons();
        StartButton();
        PauseGame();
    }

    private void Initialize()
    {
        InputBridge[] bridges = FindObjectsOfType<InputBridge>();
        Gamepad[] gamepads = Gamepad.all.ToArray();
        for (int i = 0; i < gamepads.Length; i++)
        {
            try
            {
                bridges[i].playerId = i;
            }
            catch(Exception e) 
            {
                Debug.Log("Fuck!");
            }
            if (bridges.Length == gamepads.Length)
            {

            }
            playerId = i;
        }
        // Stick ID is the player ID + 1
        m_StickId = playerId;
        gamepad = Gamepad.current;
        keyboard = Keyboard.current;

        if (initialized = gamepad != null)
        {
            input = Input.Gamepad;
        }
    }

    private void Thumbsticks()
    {
        if (gamepad != null)
        {
            GameInput.movementAxes = new Vector2(gamepad.leftStick.x.ReadValue(), gamepad.leftStick.y.ReadValue());
            GameInput.visualAxes = new Vector2(gamepad.rightStick.x.ReadValue(), gamepad.rightStick.y.ReadValue());
        }
        else
        {
            float horizontal = keyboard.rightArrowKey.ReadValue() - keyboard.leftArrowKey.ReadValue();
            float vertical = keyboard.upArrowKey.ReadValue() - keyboard.downArrowKey.ReadValue();
            GameInput.movementAxes = new Vector2(horizontal, vertical);
            GameInput.visualAxes = Vector2.zero;
        }
    }

    private void DPad()
    {
        if (gamepad != null)
        {
            GameInput.secondaryMovementAxes = gamepad.dpad.ReadValue();
        }
    }

    private void Buttons()
    {
        if (gamepad != null)
        {
            GameInput.jump = gamepad.aButton.isPressed;
            GameInput.action2 = gamepad.squareButton.isPressed;
            GameInput.action1 = gamepad.triangleButton.isPressed;
            GameInput.crouch = gamepad.circleButton.isPressed;

            GameInput.bumpLeft = gamepad.leftShoulder.isPressed;
            GameInput.bumpRight = gamepad.rightShoulder.isPressed;
            GameInput.aim = gamepad.leftTrigger.ReadValue();
            GameInput.fire = gamepad.rightTrigger.ReadValue();
        }
        else
        {
            GameInput.jump = keyboard.spaceKey.isPressed;
            GameInput.action2 = keyboard.qKey.isPressed;
            GameInput.action1 = keyboard.eKey.isPressed;
            GameInput.crouch = keyboard.cKey.isPressed || keyboard.leftAltKey.isPressed;

            GameInput.bumpLeft = keyboard.zKey.isPressed;
            GameInput.bumpRight = keyboard.xKey.isPressed;
            GameInput.aim = keyboard.shiftKey.isPressed ? 1 : 0;
            GameInput.fire = keyboard.ctrlKey.isPressed ? 1 : 0;
        }
    }

    private void StartButton()
    {
        if (gamepad != null)
        {
            GameInput.pause = gamepad.startButton.isPressed;
        }
    }

    private void PauseGame()
    {
        if (GameInput.freezeTime && Time.timeScale == 1)
            pausedThisFrame = true;
        else if (GameInput.freezeTime && Time.timeScale == 0)
            unpausedThisFrame = true;

        if (pausedThisFrame)
            Time.timeScale = 0;
        if (unpausedThisFrame)
            Time.timeScale = 1;
    }
}

public class GameInput
{
    public static Vector2 movementAxes; //left joystick
    public static Vector2 visualAxes;  //right joystick
    public static Vector2 secondaryMovementAxes; //d-pad

    public static bool uniqueLeft, uniqueRight; //left stick, right stick
    public static bool jump, action2, action1, crouch; //Cross, Square, Triangle, Circle (A, X, Y, B)
    public static bool bumpLeft, bumpRight; //left shoulder, right shoulder
    public static float aim, fire; //left trigger, right trigger

    public static bool pause, menu; //Start, Select/Back

    public static bool freezeTime;
}

//        ______                                 ______
//       |  LT  |                               |  RT  |
//       |______|                               |______|
//       |__LS__|                               |__RS__|
//        _=====_                               _=====_
//       / _____ \                             / _____ \
//     +.-'_____'-.---------------------------.-'_____'-.+
//    /   |     |  '.                       .'  |     |   \
//   / ___| /|\ |___ \                     / ___| (3) |___ \
//  / |      |      | ;  __           _   ; |             | ;
//  | | <---   ---> | | |__|         |_:> | |(2)       (4)| |
//  | |___   |   ___| ;SELECT       START ; |___       ___| ;
//  |\    | \|/ |    /  _              _   \    | (1) |    /|
//  | \   |_____|  .','" "',        ,'" "', '.  |_____|  .' |
//  |  '-.______.-' /       \      /       \  '-._____.-'   |
//  |               |   L   |------|   R   |                |
//  |              /\       /      \       /\               |
//  |             /  '.___.'        '.___.'  \              |
//  |            /                            \             |
//   \          /                              \           /
//    \________/                                \_________/
