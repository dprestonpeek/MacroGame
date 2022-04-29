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
    private static InputBridge _instance;

    public Gamepad gamepad;

    public int playerId = 0;
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
        if (gamepad != null)
            initialized = true;
    }

    private void Thumbsticks()
    {
        if (gamepad != null)
        {
            GameInput.lJoystick = new Vector2(gamepad.leftStick.x.ReadValue(), gamepad.leftStick.y.ReadValue());
            GameInput.rJoystick = new Vector2(gamepad.rightStick.x.ReadValue(), gamepad.rightStick.y.ReadValue());
        }
    }

    private void DPad()
    {
        if (gamepad != null)
        {
            GameInput.dPad = gamepad.dpad.ReadValue();
        }
    }

    private void Buttons()
    {
        if (gamepad != null)
        {
            GameInput.cross = gamepad.aButton.isPressed;
            GameInput.square = gamepad.squareButton.isPressed;
            GameInput.triangle = gamepad.triangleButton.isPressed;
            GameInput.circle = gamepad.circleButton.isPressed;

            GameInput.lShoulder = gamepad.leftShoulder.isPressed;
            GameInput.rShoulder = gamepad.rightShoulder.isPressed;
            GameInput.lTrigger = gamepad.leftTrigger.ReadValue();
            GameInput.rTrigger = gamepad.rightTrigger.ReadValue();
        }
    }

    private void StartButton()
    {
        if (gamepad != null)
        {
            GameInput.startButton = gamepad.startButton.isPressed;
        }
    }

    private void PauseGame()
    {
        if (GameInput.pauseGame && Time.timeScale == 1)
            pausedThisFrame = true;
        else if (GameInput.pauseGame && Time.timeScale == 0)
            unpausedThisFrame = true;

        if (pausedThisFrame)
            Time.timeScale = 0;
        if (unpausedThisFrame)
            Time.timeScale = 1;
    }
}

public class GameInput
{
    public static Vector2 lJoystick;
    public static Vector2 rJoystick;
    public static Vector2 dPad;

    public static bool lStick, rStick;
    public static bool cross, square, triangle, circle;
    public static bool rShoulder, lShoulder;
    public static float rTrigger, lTrigger;

    public static bool startButton, selectButton;

    public static bool pauseGame;
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
