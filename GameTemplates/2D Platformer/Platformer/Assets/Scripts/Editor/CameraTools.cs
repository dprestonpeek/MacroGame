using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraTools
{
    public static void AddCameraRepositionTrigger()
    {
        Camera camera = GameObject.FindObjectOfType<Camera>();
        CameraSettings camSettings = camera.GetComponent<CameraSettings>();

        GameObject triggerParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        triggerParent.name = "CameraRepositionTrigger";
        triggerParent.layer = 11;

        triggerParent.transform.localScale = new Vector3(.1f, .1f, 1);
        triggerParent.transform.localPosition = new Vector3(triggerParent.transform.position.x + camSettings.camOffset.x, triggerParent.transform.position.y + camSettings.camOffset.y);
        triggerParent.GetComponent<BoxCollider>().isTrigger = true;
        triggerParent.GetComponent<MeshRenderer>().enabled = false;

        CameraReposition camRepo = triggerParent.AddComponent<CameraReposition>();
        camRepo.camera = camSettings;
    }
}
