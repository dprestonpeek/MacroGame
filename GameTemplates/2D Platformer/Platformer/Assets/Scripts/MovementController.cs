using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    public Rigidbody rb;

    [SerializeField]
    AnimationController anim;

    [SerializeField]
    AudioClip land;

    [SerializeField]
    bool allowMovement = true;

    [SerializeField]
    public bool allowSkidding = true;

    public enum Direction { Left, Right }
    public Direction direction = Direction.Right;

    public bool IsFalling { get { return Falling; } set { IsFalling = value; } }

    public bool Grounded = false;
    public bool Walled = false;
    private bool Walking = false;
    public bool Jumping = false;
    private bool Falling = false;
    private bool JumpHold = false;
    public bool CanJump = false;
    public bool CanLedgeGrab = false;
    private bool Skidding = false;
    private bool Rolling = false;
    public bool Juking = false;
    public bool LedgeGrabbing = false;

    private CapsuleCollider collider;

    [SerializeField]
    private Vector2 playerVelocity;

    private int jumpCount = 0;
    public int airTurnCount = 0;
    private float rollSpeed = 0;

    [Header("Entity Stats")]

    [Tooltip("Maximum walk velocity")]
    [SerializeField]
    [Range(1, 10)]
    public int walkSpeed = 7;

    [Tooltip("How quickly the velocity increases from 0 to walkSpeed when a joystick is connected.")]
    [SerializeField]
    [Range(1, 10)]
    public int joystickWalkAcceleration = 5;
    [Tooltip("How quickly the velocity increases from 0 to walkSpeed when only a keyboard is connected.")]
    [SerializeField]
    [Range(1, 10)]
    public int keyboardWalkAcceleration = 1;
    [Tooltip("The current amount of walk acceleration. This value is set by one of the 2 previous values.")]
    [SerializeField]
    [Range(1, 10)]
    public int walkAcceleration = 5;

    [Tooltip("The velocity at which the roll mechanic is used.")]
    [SerializeField]
    [Range(1, 10)]
    private int rollTolerance = 1;

    [Tooltip("The maximum height reached when jumping.")]
    [SerializeField]
    [Range(1, 20)]
    private int jumpHeight = 10;
    [Tooltip("The walk speed while jumping or falling. (automatically decreases with each in-air x-direction change.")]
    [SerializeField]
    [Range(1, 10)]
    private int airwalkSpeed = 5;
    [Tooltip("The speed at which the entity falls when jump input stops.")]
    [SerializeField]
    [Range(1, 15)]
    private int fallSpeed = 3;
    [Tooltip("The speed at which the entity falls when jump input stops.")]
    [SerializeField]
    [Range(-15, -1)]
    private int gravity = -10;

    // Start is called before the first frame update
    public virtual void Start()
    {
        collider = GetComponentInChildren<CapsuleCollider>();
        anim = GetComponentInChildren<AnimationController>();
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (allowMovement)
        {
            Grounded = IsGrounded();
            Walled = IsWalled();
            if (CanLedgeGrab)
            {
                LedgeGrabbing = IsLedgeGrabbing();
                if (LedgeGrabbing)
                {
                    LedgeGrab();
                    CanJump = true;
                }
            }
            Gravity();
        }
    }

    public virtual void LateUpdate()
    {
        UpdateVelocity();
    }

    public void SetDirection(Direction dir)
    {
        direction = dir;
    }

    public void CheckAndDoWalk(float horizInputMovement)
    {
        CheckAndDoWalk(horizInputMovement, CalculateDirection(horizInputMovement));
    }

    public void CheckAndDoWalk(float horizInputMovement, Direction dir)
    {
        Direction oldDir = direction;
        float oldVel = Mathf.Abs(playerVelocity.x);
        SetDirection(dir);
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
            if ((Grounded || LedgeGrabbing) && !Jumping && !Falling && !JumpHold)
            {
                StopLedgeGrabbing();
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

    private void Gravity()
    {
        rb.AddForce(Vector3.up * gravity);
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

    private bool IsLedgeGrabbing()
    {
        if (CanLedgeGrab)
        {
            if (GameInput.bumpRight)
            {
                return true;
            }
        }
        rb.isKinematic = false;
        return false;
    }

    private void LedgeGrab()
    {
        if (LedgeGrabbing)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            Jumping = false;
            airTurnCount = 0;
        }
    }

    private void StopLedgeGrabbing()
    {
        CanLedgeGrab = false;
        rb.isKinematic = false;
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
        return IsWalled(.65f);
    }

    public bool IsWalled(float distance)
    {
        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        float offset = collider.height / (2 + (2/3)); //.75 for a height of 2
        Vector3 offsetUp = new Vector3(transform.position.x, transform.position.y + offset);
        Vector3 offsetDown = new Vector3(transform.position.x, transform.position.y - (offset - .1f));
        Vector3[] rayPos = new Vector3[] { transform.position, offsetUp, offsetDown };

        float dir = CalculateDirection(direction);

        // Does the ray intersect any objects
        foreach (Vector3 position in rayPos)
        {
            //Check for ledgegrab
            if (position == offsetUp && (!Grounded))
            {
                if (Physics.Raycast(position, Vector3.right * dir, out hit, distance+.25f, layerMask))
                {
                    //make sure the object to grab is actually a ledge
                    Vector3 newPos = position;
                    newPos.y += .35f;
                    if (!Physics.Raycast(newPos, Vector3.right * dir, out hit, distance + .25f, layerMask))
                    {
                        Debug.DrawRay(newPos, Vector2.right * dir * (distance + .25f), Color.yellow);
                        CanLedgeGrab = true;
                    }
                    else
                    {
                        CanLedgeGrab = false;
                    }
                }
                else
                {
                    CanLedgeGrab = false;
                }
            }
            else
            {
                CanLedgeGrab = false;
            }
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
