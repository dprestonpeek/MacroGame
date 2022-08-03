using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraSettings : MonoBehaviour
{
    [SerializeField]
    private bool followPlayer = true;
    [SerializeField]
    private PlayerMovement[] players;

    Vector3 target;

    [SerializeField]
    public Vector3 camOffset;
    [SerializeField]
    [Range(0,.5f)]
    private float camXSpeed;
    [SerializeField]
    [Range(0, .25f)]
    private float camYSpeed;
    [SerializeField]
    private float floor = 3;

    private bool playerAsFloor = false;
    private float playerFloorSpeed = .5f;
    private float defaultYSpeed = .02f;

    // Start is called before the first frame update
    void Awake()
    {
        players = FindObjectsOfType<PlayerMovement>();
        defaultYSpeed = camYSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying && followPlayer)
        {
            FollowPlayer();
        }
    }

    public void SetNewFloor(int newFloor)
    {
        floor = newFloor;
    }

    public void SetPlayerAsFloor()
    {
        playerAsFloor = true;
    }

    private void LateUpdate()
    {
        if (Application.isPlaying && followPlayer)
        {
            FollowPlayerLate();
        }
    }

    private void FollowPlayer()
    {
        //There is one player
        if (players.Length == 1)
        {
            PlayerMovement playerObj = players[0];
            if (playerObj != null)
            {
                target = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, -10);
            }
        }
        //There is more than one player
        else if (players.Length > 1)
        {
            float avgX = 0;
            float avgY = 0;
            foreach (PlayerMovement p in players)
            {
                avgX += p.transform.position.x;
                avgY += p.transform.position.y;
            }
            target = new Vector3(avgX / players.Length, avgY / players.Length, -10);
        }
    }

    private void FollowPlayerLate()
    {
        Vector3 newPos = new Vector3(target.x + camOffset.x, target.y + camOffset.y, -10);
        newPos = ClampToLimits(newPos);

        float xLerp = Mathf.Lerp(transform.position.x, newPos.x, camXSpeed);
        float yLerp = Mathf.Lerp(transform.position.y, newPos.y, camYSpeed);

        newPos.x = xLerp;
        newPos.y = yLerp;

        transform.position = ClampToLimits(Vector3.Lerp(transform.position, newPos, camXSpeed));
    }

    Vector3 ClampToLimits(Vector3 position)
    {
        if (playerAsFloor)
        {
            camYSpeed = playerFloorSpeed;
            position.y = Mathf.Lerp(position.y, players[0].transform.position.y, camYSpeed);
        }
        else
        {
            camYSpeed = defaultYSpeed;
            position.y = Mathf.Lerp(position.y, floor, camYSpeed);
        }
        return position;
    }
}

