using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReposition : MonoBehaviour
{
    [Tooltip("This object should automatically be set to a script in the camera in-scene.")]
    [SerializeField]
    public CameraSettings camera;

    [Tooltip("Desired floor level upon passing through the trigger.")]
    [SerializeField]
    private int newFloor = 0;

    [Tooltip("Desired floor level upon returning back through the trigger.")]
    [SerializeField]
    private int oldFloor = 0;

    [Tooltip("Indicates a one-way switch to newFloor. Even if the player passes back through the trigger, the floor will not be updated to oldFloor.")]
    [SerializeField]
    private bool oneWayChange = false;

    [Tooltip("The camera will follow the player instead of snap to newFloor.")]
    [SerializeField]
    private bool playerAsNewFloor = false;

    [Tooltip("Swaps the values of newFloor and oldFloor, effectively reversing the trigger.")]
    [SerializeField]
    private bool invertTrigger = false;

    private enum ScanTime { Enter, Exit, Stay }
    [Tooltip("The exact moment the trigger is activated. Whether the player enters the trigger, exits the trigger, or while the player stays in the trigger.")]
    [SerializeField]
    private ScanTime scanTime = ScanTime.Enter;

    private enum Orientation { Vertical, Horizontal }
    [Tooltip("Indicates the orientation of the trigger itself. The trigger should be positioned vertically like a wall if the player will be passing through horizontally.")]
    [SerializeField]
    private Orientation orientation = Orientation.Vertical;

    private GameObject currCollision;
    private Rigidbody currRb;

    //Exit right and down advance to new camera positions (think walking forward or falling)
    private void ExitRight(int value)
    {
        camera.SetNewFloor(value);
    }
    private void ExitLeft(int value)
    {
        camera.SetNewFloor(value);
    }
    private void ExitUp(int value)
    {
        camera.SetNewFloor(value);
    }
    private void ExitDown(int value)
    {
        camera.SetNewFloor(value);
    }

    private void RepositionCamera(Collider other)
    {
        if (currCollision == null)
        {
            currCollision = other.gameObject.GetComponentInParent<MovementController>().gameObject;
            currRb = currCollision.GetComponent<Rigidbody>();
        }
        if (currRb != null)
        {
            if (playerAsNewFloor)
            {
                camera.SetPlayerAsFloor();
            }
            if (orientation == Orientation.Vertical)
            {
                if (currRb.velocity.x > 0)
                {
                    if (invertTrigger && oneWayChange)
                    {
                        ExitRight(oldFloor);
                    }
                    else if (!invertTrigger)
                    {
                        ExitRight(newFloor);
                    }
                }
                else if (currRb.velocity.x < 0)
                {
                    if (invertTrigger && oneWayChange)
                    {
                        ExitLeft(newFloor);
                    }
                    else if (!invertTrigger)
                    {
                        ExitLeft(oldFloor);
                    }
                }
            }
            else if (orientation == Orientation.Horizontal)
            {
                if (invertTrigger && oneWayChange)
                {
                    ExitUp(oldFloor);
                }
                else if (!invertTrigger)
                {
                    ExitUp(newFloor);
                }
                else if (currRb.velocity.y < 0)
                {
                    if (invertTrigger && oneWayChange)
                    {
                        ExitDown(newFloor);
                    }
                    else if (!invertTrigger)
                    {
                        ExitDown(oldFloor);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (scanTime != ScanTime.Enter)
        {
            return;
        }
        RepositionCamera(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (scanTime != ScanTime.Stay)
        {
            return;
        }
        RepositionCamera(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (scanTime != ScanTime.Exit)
        {
            return;
        }
        RepositionCamera(other);
    }
}
