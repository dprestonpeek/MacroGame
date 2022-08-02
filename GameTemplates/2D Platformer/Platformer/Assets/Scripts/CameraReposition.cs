using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReposition : MonoBehaviour
{
    [SerializeField]
    public CameraSettings camera;

    [SerializeField]
    private bool oneWayChange = false;

    [SerializeField]
    private bool invertTrigger = false;

    [SerializeField]
    private int oldFloor = 0;

    [SerializeField]
    private int newFloor = 0;

    private enum ScanTime { Enter, Exit, Stay }
    [SerializeField]
    private ScanTime scanTime = ScanTime.Enter;

    private enum Orientation { Vertical, Horizontal }
    [SerializeField]
    private Orientation orientation = Orientation.Vertical;

    private GameObject currCollision;
    private Rigidbody currRb;

    //Exit right and down advance to new camera positions (think walking forward or falling)
    //old and new floor can be reversed for the opposite effect

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
