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
    [Tooltip("The frequency of which attacks are pulled from the attack pool. Higher frequency = less time between attacks.")]
    [SerializeField]
    [Range(1, 10)]
    private int attackFrequency = 1;
    [Tooltip("The overall aggressiveness of the enemy; how easily provoked they are. 0 = passive, does not deal damage at all.")]
    [SerializeField]
    [Range(0, 10)]
    private int entityHostility = 1;

    public enum EnemyActions { NOTHING, FLEE, DEFEND, CHARGE }
    [Header("Attack Behavior")]
    [Tooltip("The action for the enemy when the interruption trigger is collided with.")]
    [SerializeField]
    public EnemyActions whenInterrupted = EnemyActions.NOTHING;
    [Tooltip("The action for the enemy when the interruption trigger is collided with.")]
    [SerializeField]
    public EnemyActions whenProvoked = EnemyActions.DEFEND;

    [Tooltip("The horizontal distance (in units) at which the enemy notices a target's presence.")]
    [SerializeField]
    [Range(0, 30)]
    public int interruptionRangeX = 25;
    [Tooltip("The vertical distance (in units) at which the enemy notices a target's presence.")]
    [SerializeField]
    [Range(0, 30)]
    public int interruptionRangeY = 25;
    [Tooltip("The horizontal distance (in units) at which the enemy becomes provoked and begins to attack the target.")]
    [SerializeField]
    [Range(0, 30)]
    public int provocationRangeX = 10;
    [Tooltip("The vertical distance (in units) at which the enemy becomes provoked and begins to attack the target.")]
    [SerializeField]
    [Range(0, 30)]
    public int provocationRangeY = 10;

    [Header("Attack Pool")]
    [Tooltip("The probability of each attack to be pulled from the pool when combat is engaged.")]
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

    private enum AttackType { DASH, LUNGE, THRUST, CLUB, PROJECTILE, SMASH }
    private AttackType[] pool;

    private bool turningAround = false;
    //private bool faceRight = true;
    public bool neutral = true;
    public bool interrupted = false;
    public bool provoked = false;
    public bool pace = false;
    private bool attacking = false;
    private bool stuck = false;
    private bool maybeStuck = false;

    private int stuckCount = 0;

    private Transform target;
    EnemyTrigger interruptionTrigger;
    EnemyTrigger provocationTrigger;

    private bool canAttack = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        direction = faceRightAtStart ? Direction.Right : Direction.Left;
        pace = paceWhileWaiting;
    }

    private void Awake()
    {
        InitializeTriggers();
        InitializeAttackPool();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (neutral && paceWhileWaiting)
        {
            pace = true;
            Pace();
            allowSkidding = true;
        }
        else
        {
            pace = false;
        }
        //the player has entered the enemy's interruption range
        if (interrupted && !provoked)
        {
            if (whenInterrupted != EnemyActions.NOTHING)
            {
                neutral = false;
                switch (whenInterrupted)
                {
                    case EnemyActions.FLEE:
                        Flee();
                        break;
                    case EnemyActions.DEFEND:
                        FaceTarget();
                        break;
                    case EnemyActions.CHARGE:
                        break;
                }
            }

            //WalkBackwards(walkSpeed / 4);
        }
        //the player has exited the enemy's interruption range
        else if (!interrupted && !neutral)
        {
            allowSkidding = false;
            FaceAwayFromTarget();
            //direction = direction == Direction.Right ? Direction.Left : Direction.Right;
            pace = paceWhileWaiting;
            neutral = true;
        }
        //the player has entered the enemy's provocation range
        if (provoked)
        {
            if (whenProvoked != EnemyActions.NOTHING)
            {
                switch (whenProvoked)
                {
                    case EnemyActions.FLEE:
                        Flee();
                        break;
                    case EnemyActions.DEFEND:
                        FaceTarget();
                        break;
                    case EnemyActions.CHARGE:
                        AttackRandom();
                        break;
                }
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetInterrupted(bool interrupt)
    {
        interrupted = interrupt;
    }

    public void SetProvoked(bool provoke)
    {
        provoked = provoke;
    }

    private void Pace()
    {
        CheckAndDoWalk(direction == Direction.Right ? 1 : -1, direction);
        if (Walled && !turningAround)
        {
            turningAround = true;
            direction = direction == Direction.Right ? Direction.Left : Direction.Right;
        }
        else if (!Walled)
        {
            turningAround = false;
        }
    }

    private void FaceTarget()
    {
        if (target.position.x < transform.position.x)
        {
            SetDirection(Direction.Left);
        }
        else if (target.position.x > transform.position.x)
        {
            SetDirection(Direction.Right);
        }
    }

    private void FaceAwayFromTarget()
    {
        if (target.position.x < transform.position.x)
        {
            SetDirection(Direction.Right);
        }
        else if (target.position.x > transform.position.x)
        {
            SetDirection(Direction.Left);
        }
    }

    private void WalkBackwards(float speed)
    {
        CheckAndDoWalk(speed, direction);
    }

    private void Dance(float speed)
    {

    }

    private void Flee()
    {
        FaceAwayFromTarget();
        CheckAndDoWalk(direction == Direction.Right ? 1 : -1, direction);

        if (IsFalling || stuckCount > 100)
        {
            CheckAndDoJump(false);
            stuckCount = 0;
        }

        if (IsWalled(4) && !IsFalling)
        {
            CheckAndDoJump(true);
            stuckCount++;
        }
        Debug.Log("Fleeing");
    }

    private void WaitForAttackDelay()
    {

    }

    private void InitializeTriggers()
    {
        EnemyTrigger[] triggers = GetComponentsInChildren<EnemyTrigger>();
        for (int i = 0; i < 2; i++)
        {
            if (triggers[i].triggerType == EnemyTrigger.TriggerType.INTERRUPTION)
            {
                interruptionTrigger = triggers[i];
            }
            else if (triggers[i].triggerType == EnemyTrigger.TriggerType.PROVOCATION) 
            {
                provocationTrigger = triggers[i];
            }
        }
    }

    private void InitializeAttackPool()
    {
        int poolSize = dashAttack + lungeAttack + thrustAttack + clubAttack + projectileAttack + smashAttack;
        pool = new AttackType[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            if (i < dashAttack)
            {
                pool[i] = AttackType.DASH;
            }
            else if (i < dashAttack + lungeAttack)
            {
                pool[i] = AttackType.LUNGE;
            }
            else if (i < dashAttack + lungeAttack + thrustAttack)
            {
                pool[i] = AttackType.THRUST;
            }
            else if (i < dashAttack + lungeAttack + thrustAttack + clubAttack)
            {
                pool[i] = AttackType.CLUB;
            }
            else if (i < dashAttack + lungeAttack + thrustAttack + clubAttack + projectileAttack)
            {
                pool[i] = AttackType.PROJECTILE;
            }
            else if (i < dashAttack + lungeAttack + thrustAttack + clubAttack + projectileAttack + smashAttack)
            {
                pool[i] = AttackType.SMASH;
            }
        }
    }

    private void AttackRandom()
    {
        
        attacking = true;
        switch (pool[Random.Range(0, pool.Length - 1)])
        {
            case AttackType.DASH:
                StartCoroutine(DashAttack());
                break;
            case AttackType.LUNGE:
                LungeAttack();
                break;
            case AttackType.THRUST:
                ThrustAttack();
                break;
            case AttackType.CLUB:
                ClubAttack();
                break;
            case AttackType.PROJECTILE:
                ProjectileAttack();
                break;
            case AttackType.SMASH:
                SmashAttack();
                break;
        }
    }

    /// <summary>
    /// The enemy dashes at the player until it hits a wall. 
    /// </summary>
    private IEnumerator DashAttack()
    {
        while (!IsWalled())
        {
            int dir = 0;
            if (direction == Direction.Left)
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }
            CheckAndDoWalk(dir * 2);
            yield return null;
        }
        attacking = false;
    }

    /// <summary>
    /// The enemy lunges towards the player, then moves back to original X position
    /// </summary>
    private void LungeAttack()
    {

    }

    /// <summary>
    /// The enemy thrusts an appendage or weapon at the player
    /// </summary>
    private void ThrustAttack()
    {

    }

    /// <summary>
    /// The enemy swings a club-type weapon or sword at the player
    /// </summary>
    private void ClubAttack()
    {

    }

    /// <summary>
    /// The enemy shoots or throws a projectile object at the player
    /// </summary>
    private void ProjectileAttack()
    {

    }

    /// <summary>
    /// The enemy smashes the ground, causing a shockwave which throws the player
    /// </summary>
    private void SmashAttack()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackPlayerWhenInRange && other.GetComponentInParent<PlayerMovement>())
        {
            paceWhileWaiting = false;
            provoked = true;
        }
    }
}
