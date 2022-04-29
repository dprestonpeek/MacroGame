using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerTools
{
    public static void AddPlayer()
    {
        GameObject playerParent = new GameObject("PlayerParent");
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.localScale = new Vector3(1, 1, 1);
        player.transform.parent = playerParent.transform;
        playerParent.tag = "Player";

        PlayerMovement movement = playerParent.AddComponent<PlayerMovement>();
        movement.rb = playerParent.AddComponent<Rigidbody>();
        movement.rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        movement.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        PlayerAnimation anim = player.AddComponent<PlayerAnimation>();
        InputBridge bridge = player.AddComponent<InputBridge>();
    }
}
