using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CameraSettings : MonoBehaviour
{
    [SerializeField]
    private bool followPlayer = true;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float horizontalBuffer;
    [SerializeField]
    private float verticalBuffer;

    [SerializeField]
    private Vector2 camPos;
    [SerializeField]
    [Range(0,1)]
    private float camSpeed;

    private Vector2 target;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (EditorApplication.isPlaying)
        {
            if (player != null)
            {
                target = player.transform.position + new Vector3(camPos.x, camPos.y, -10);
            }

            if (followPlayer)
            {
                Vector3.Lerp(transform.position, target, camSpeed);
            }
        }
        else 
        {
            transform.position = new Vector3(camPos.x, camPos.y, -10);
        }
    }
}

