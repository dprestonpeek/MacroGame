using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MovementController
{
    public override void Update()
    {
        base.Update();
    }

    private void JankyPositionTracking()
    {

        //RaycastHit hit;
        //int layerMask = 1 << 8;
        //layerMask = ~layerMask;

        //float offset = 1;
        //Vector3 offsetNearest = new Vector3(transform.position.x + (offset * CalculateDirection()) * 1, transform.position.y);
        //Vector3 offsetNear = new Vector3(transform.position.x + (offset * CalculateDirection()) * 2, transform.position.y);
        //Vector3 offsetMedNear = new Vector3(transform.position.x + (offset * CalculateDirection()) * 3, transform.position.y);
        //Vector3 offsetMedFar = new Vector3(transform.position.x + (offset * CalculateDirection()) * 4, transform.position.y);
        //Vector3 offsetFar = new Vector3(transform.position.x + (offset * CalculateDirection()) * 5, transform.position.y);
        //Vector3 offsetFarthest = new Vector3(transform.position.x + (offset * CalculateDirection()) * 6, transform.position.y);
        //List<Vector3> rayPos = new List<Vector3>() { offsetNearest, offsetNear, offsetMedNear, offsetMedFar, offsetFar, offsetFarthest };
        //float one = 0, two = 0, three = 0, four = 0, five = 0, six = 0;

        //// Does the ray intersect any objects
        //for (int i = 0; i < rayPos.Count; i++)
        //{
        //    if (Physics.Raycast(rayPos[i], Vector3.down, out hit, 25.1f, layerMask))
        //    {
        //        if (hit.transform.CompareTag("Floor") || hit.transform.CompareTag("Wall"))
        //        {
        //            Debug.DrawRay(rayPos[i], Vector2.down * 25.1f, Color.yellow);
        //            //newFloor = hit.transform.position.y;
        //            switch (i)
        //            {
        //                case 0:
        //                    one = hit.transform.position.y;
        //                    break;
        //                case 1:
        //                    two = hit.transform.position.y;
        //                    break;
        //                case 2:
        //                    three = hit.transform.position.y;
        //                    break;
        //                case 3:
        //                    four = hit.transform.position.y;
        //                    break;
        //                case 4:
        //                    five = hit.transform.position.y;
        //                    break;
        //                case 5:
        //                    six = hit.transform.position.y;
        //                    break;
        //            }
        //            newFloor = (one + two + three + four + five + six) / 6;
        //            newFloor = (newFloor + transform.position.y) / 2;
        //        }
        //    }
        //    else
        //    {
        //        newFloor = transform.position.y;
        //    }
            ////if (Physics.Raycast(position, Vector3.up, out hit, 10.1f, layerMask))
            ////{
            ////    if (hit.transform.CompareTag("Floor") || hit.transform.CompareTag("Wall"))
            ////    {
            ////        Debug.DrawRay(position, Vector2.up * 10.1f, Color.yellow);
            ////        newCeiling = hit.transform.position.y;
            ////    }
            ////}
            ////else
            ////{
            ////    newCeiling = 100;
            ////}
        //}
    }
}
