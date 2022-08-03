using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MovementController
{
    [Header("Enemy Stats")]
    [SerializeField]
    private bool faceRightAtStart = true;

    [SerializeField]
    public bool paceWhileWaiting = true;

    [SerializeField]
    public bool attackPlayerWhenInRange = false;

    [Header("Attack Stats")]
    [Tooltip("The speed at which each attack is performed.")]
    [SerializeField]
    [Range(1, 10)]
    private int attackSpeed = 1;
    [Tooltip("The frequency of which attacks are pulled from the attack pool.")]
    [SerializeField]
    [Range(1, 10)]
    private int attackFrequency = 1;

    [Header("Attack Pool")]
    [Tooltip("The probability of each attack to be pulled from the pool.")]
    [SerializeField]
    [Range(0, 10)]
    private int dashAttack = 0;
    [SerializeField]
    [Range(0, 10)]
    private int lungeAttack = 0;
    [SerializeField]
    [Range(0, 10)]
    private int thrustAttack = 0;
    [SerializeField]
    [Range(0, 10)]
    private int clubAttack = 0;
    [SerializeField]
    [Range(0, 10)]
    private int projectileAttack = 0;
    [SerializeField]
    [Range(0, 10)]
    private int smashAttack = 0;

    private bool turningAround = false;
    private bool faceRight = true;
    private bool combat = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        faceRight = faceRightAtStart;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (paceWhileWaiting)
        {
            Pace();
        }
        if (combat)
        {
            FacePlayer();
        }
    }

    private void Pace()
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

    private void FacePlayer()
    {

    }

    private void BeginAttackSequence()
    {

    }

    /// <summary>
    /// The enemy dashes at the player until it hits a wall. 
    /// </summary>
    private void DashAttack()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackPlayerWhenInRange && other.GetComponentInParent<PlayerMovement>())
        {
            paceWhileWaiting = false;
            combat = true;
        }
    }
}
