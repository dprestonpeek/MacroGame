using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public enum Direction { Left, Right }
    public Direction direction = Direction.Right;

    [SerializeField]
    public Rigidbody rb;

    [SerializeField]
    AnimationController anim;

    [SerializeField]
    AudioClip land;

    [SerializeField]
    bool allowMovement = true;

    [SerializeField]
    bool allowSkidding = true;


    private CapsuleCollider collider;

    public bool Grounded = false;
    public bool Walled = false;
    private bool Walking = false;
    private bool Jumping = false;
    private bool Falling = false;
    private bool JumpHold = false;
    private bool CanJump = false;
    private bool Skidding = false;
    private bool Rolling = false;
    public bool Juking = false;


    [SerializeField]
    private Vector2 playerVelocity;

    private int jumpCount = 0;
    private int airTurnCount = 0;
    private float rollSpeed = 0;

    [SerializeField]
    [Range(1, 10)]
    private int walkSpeed = 7;
    [SerializeField]
    [Range(1, 10)]
    public int joystickWalkAcceleration = 5;
    [SerializeField]
    [Range(1, 10)]
    public int keyboardWalkAcceleration = 1;
    [Range(1, 10)]
    public int walkAcceleration = 5;
    //[SerializeField]
    //[Range(1, 10)]
    //private int skidTolerance = 5;
    [SerializeField]
    [Range(1, 10)]
    private int rollTolerance = 1;

    [SerializeField]
    [Range(1, 10)]
    private int jumpHeight = 10;
    [SerializeField]
    [Range(1, 10)]
    private int airwalkSpeed = 5;
    [SerializeField]
    [Range(1, 15)]
    private int fallSpeed = 3;

    // Start is called before the first frame update
    public virtual void Start()
    {
        collider = GetComponentInChildren<CapsuleCollider>();
        anim = GetComponentInChildren<AnimationController>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (allowMovement)
        {
            Grounded = IsGrounded();
            Walled = IsWalled();
        }
    }

    public virtual void LateUpdate()
    {
        UpdateVelocity();
    }

    public void CheckAndDoWalk(float horizInputMovement)
    {
        Direction oldDir = direction;
        float oldVel = Mathf.Abs(playerVelocity.x);
        direction = CalculateDirection(horizInputMovement);
        if (oldDir != direction)
        {
            if (Jumping || Falling)
            {
                airTurnCount++;
            }
            else
            {
                if (oldVel >= walkSpeed - 1)
                {
                    Skidding = true;
                }
            }
        }

        if (Falling)
        {
            anim.SetDirection(direction == Direction.Right);
        }

        if (Mathf.Abs(horizInputMovement) > .25f && !IsWalled())
        {
            rollSpeed = rb.velocity.x;
            if (Grounded)
            {
                anim.SetDirection(direction == Direction.Right);
            }
            if (!Skidding)
            {
                Walk(horizInputMovement);
                Walking = true;
            }
            else
            {
                anim.SetDirection(direction == Direction.Right);
                Skid();
                //Slow to a stop
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 10 - rollTolerance)
        {
            Rolling = true;
            Roll();
        }
        else
        {
            Walking = false;
            Rolling = false;
        }
    }

    public void CheckAndDoJump(bool jump)
    {
        if (playerVelocity.y > .1f)
        {
            Jumping = true;
        }
        else if (playerVelocity.y < -.1f)
        {
            Jumping = false;
            CanJump = false;
            Falling = true;
            if (jumpCount == 1 && !JumpHold)
            {
                CanJump = true;
            }
        }
        else
        {
            Falling = false;
        }

        if (!JumpHold)
        {
            ForceFall();
        }

        if (jump)
        {
            if (Grounded && !Jumping && !Falling && !JumpHold)
            {
                JumpHold = true;
                Jump(jumpHeight);
                if (playerVelocity.x == 0)
                {
                    airTurnCount++;
                }
            }
        }
        else
        {
            if (Jumping && Grounded)
            {
                Jumping = false;
            }
            JumpHold = false;
        }
        if (Grounded && !JumpHold)
        {
            airTurnCount = 0;
        }
    }

    private void UpdateVelocity()
    {
        playerVelocity = new Vector2(Mathf.Round(rb.velocity.x), Mathf.Round(rb.velocity.y));
    }

    private void Walk(float dir)
    {
        float gradualWalkSpeed = Mathf.Lerp(Mathf.Abs(rb.velocity.x), walkSpeed, (float)walkAcceleration / 10);
        if (gradualWalkSpeed > 5)
        {
            if (airTurnCount > 0)
            {
                gradualWalkSpeed = airwalkSpeed / airTurnCount;
            }
            else
            {
                gradualWalkSpeed = walkSpeed;
                //airTurnCount = 0;
            }
        }

        Vector3 newVelocity = new Vector2(dir * gradualWalkSpeed, rb.velocity.y);

        rb.velocity = newVelocity;
    }

    private void Skid()
    {
        if (allowSkidding)
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, 1 / 2), rb.velocity.y);
            if (Mathf.Abs(rb.velocity.x) <= .1f)
            {
                Skidding = false;
            }
        }
        else
        {
            Skidding = false;
        }
    }

    private void Roll()
    {
        if (rollTolerance == 1)
            return;

        rb.velocity = new Vector2(rollSpeed, rb.velocity.y);
    }

    private void Jump(float force)
    {
        rb.AddForce(Vector2.up * force, ForceMode.Impulse);
    }

    private void HitGround()
    {
        //jumpVelocity = 0;
        jumpCount = 0;
        if (Falling)
        {
            GetComponent<AudioSource>().PlayOneShot(land);
        }
        Falling = false;
        Jumping = false;
        //airTurnCount = 0;
        //anim.Grounded();
    }

    void ForceFall()
    {
        rb.AddForce(Vector2.down * 5 * fallSpeed);
    }

    Direction CalculateDirection(float dir)
    {
        if (dir > 0)
        {
            return Direction.Right;
        }
        if (dir < 0)
        {
            return Direction.Left;
        }
        //this code is reached when the player has not moved at all yet
        return direction;
    }

    public float CalculateDirection()
    {
        return CalculateDirection(direction);
    }

    float CalculateDirection(Direction dir)
    {
        if (dir == Direction.Right)
        {
            return 1;
        }
        if (dir == Direction.Left)
        {
            return -1;
        }
        return (float)direction;
    }

    public bool IsWalled()
    {
        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        float offset = collider.height / (2 + (2/3)); //.75 for a height of 2
        Vector3 offsetUp = new Vector3(transform.position.x, transform.position.y + offset);
        Vector3 offsetDown = new Vector3(transform.position.x, transform.position.y - (offset - .1f));
        Vector3[] rayPos = new Vector3[] { transform.position, offsetUp, offsetDown };

        float dir = CalculateDirection(direction);
        float distance = .65f;

        // Does the ray intersect any objects
        foreach (Vector3 position in rayPos)
        {
            Debug.DrawRay(position, Vector2.right * dir * distance, Color.yellow);
            if (Physics.Raycast(position, Vector3.right * dir, out hit, distance, layerMask))
            {
                if (hit.transform.CompareTag("Floor") || hit.transform.CompareTag("Wall"))
                {
                    CanJump = true;
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        
        float offset = .25f;
        Vector3 offsetL = new Vector3(transform.position.x + offset, transform.position.y);
        Vector3 offsetR = new Vector3(transform.position.x - offset, transform.position.y);
        Vector3[] rayPos = new Vector3[] { transform.position, offsetL, offsetR };

        // Does the ray intersect any objects
        foreach (Vector3 position in rayPos)
        {
            Debug.DrawRay(position, Vector2.down * 1.1f, Color.yellow);
            if (Physics.Raycast(position, Vector3.down, out hit, 1.1f, layerMask))
            {
                if (hit.transform.CompareTag("Floor") || hit.transform.CompareTag("Wall"))
                {
                    CanJump = true;
                    return true;
                }
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            if (IsGrounded())
            {
                HitGround();
            }
        }
    }
}
