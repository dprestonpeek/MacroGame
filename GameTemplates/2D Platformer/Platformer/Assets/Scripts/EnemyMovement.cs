using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MovementController
{
    [SerializeField]
    public bool moveAutomatically = true;

    [SerializeField]
    private bool faceRight = true;

    private bool turningAround = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public void Pace()
    {
        CheckAndDoWalk(faceRight ? 1 : -1);
        if (Walled && !turningAround)
        {
            turningAround = true;
            faceRight = !faceRight;
        }
        else if (!Walled)
        {
            turningAround = false;
        }
    }
}
