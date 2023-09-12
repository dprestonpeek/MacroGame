using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MovementController
{
    [Header("UI Elements")]
    [SerializeField]
    private Slider healthBar;

    private InputBridge.Input input;

    public override void FixedUpdate()
    {
        CheckInput();
        base.FixedUpdate();
        CheckAndDoWalk(GameInput.movementAxes.x);
        CheckAndDoJump(GameInput.jump);
    }

    void CheckInput()
    {
        input = InputBridge.Instance.input;
        if (input == InputBridge.Input.Gamepad)
        {
            walkAcceleration = joystickWalkAcceleration;
        }
        else
        {
            walkAcceleration = keyboardWalkAcceleration;
        }
    }
}
