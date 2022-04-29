using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraTools
{
    public static void AddCameraBuffer()
    {
        Camera camera = GameObject.FindObjectOfType<Camera>();
        CameraSettings camSettings = camera.GetComponent<CameraSettings>();

        GameObject bufferParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bufferParent.name = "CameraBuffer";
        bufferParent.layer = 11;
        //bufferParent.transform.parent = camera.transform;
        bufferParent.transform.localScale = new Vector3(12, 7, 1);
        bufferParent.transform.localPosition = new Vector3(bufferParent.transform.position.x + camSettings.camOffset.x, bufferParent.transform.position.y + camSettings.camOffset.y);
        bufferParent.GetComponent<BoxCollider>().enabled = false;
        bufferParent.GetComponent<MeshRenderer>().enabled = false;

        CameraBuffer camBuffer = bufferParent.AddComponent<CameraBuffer>();
        camBuffer.bufferParent = bufferParent;
        camBuffer.cam = camera;
        //camSettings.camBuffer = camBuffer;
        //camSettings.camBuffer.Initialize();
    }
}
