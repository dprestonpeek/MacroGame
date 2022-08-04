using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MovementController
{
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
