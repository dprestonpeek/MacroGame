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
    public bool CanJump = false;
    public bool Juking = false;
    private bool Walking = false;
    private bool Jumping = false;
    private bool Falling = false;
    private bool JumpHold = false;
    private bool CanLedgeGrab = false;
    private bool Skidding = false;
    private bool Rolling = false;
    private bool LedgeGrabbing = false;
    private bool LedgeJumped = false;

    private CapsuleCollider collider;

    [SerializeField]
    private Vector2 playerVelocity;

    private int jumpCount = 0;
    private int airTurnCount = 0;
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
        //this is not so important, just for viewing purposes
        UpdateVelocity();
    }

    /// <summary>
    /// Force the entity to face a certain direction
    /// </summary>
    /// <param name="dir"></param>
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
        //face the right way
        SetDirection(dir);
        //if we change direction...
        if (oldDir != direction)
        {
            //while in the air
            if (Jumping || Falling)
            {
                airTurnCount++;
            }
            else
            {
                //skid if we're on the ground
                if (allowSkidding)
                {
                    Skidding = true;
                }
            }
        }

        if (Falling)
        {
            //update the direction of the animation in case we were backwards jumping
            anim.SetDirection(direction == Direction.Right);
            //now that we're falling, we aren't ledgejumping anymore
            LedgeJumped = false;
        }

        //If we're walking at full speed
        if (Mathf.Abs(horizInputMovement) > .25f && !IsWalled())
        {
            //save the roll speed in case we need that later
            rollSpeed = rb.velocity.x;
            //set the direction the right way
            if (Grounded)
            {
                anim.SetDirection(direction == Direction.Right);
            }
            if (Skidding)
            {
                anim.SetDirection(direction == Direction.Right);
                //Slow to a stop
                Skid();
            }
            else
            {
                Walk(horizInputMovement);
                Walking = true;
            }
        }
        //save the roll info, wait til last second to see if roll is enabled
        else if (Mathf.Abs(rb.velocity.x) > 10 - rollTolerance)
        {
            Rolling = true;
            Roll();
        }
        //we're not touching the joystick, stop moving
        else
        {
            Walking = false;
            Rolling = false;
        }
    }

    /// <summary>
    /// Check if jump button is pressed, and jump if it is
    /// </summary>
    /// <param name="jump"></param>
    public void CheckAndDoJump(bool jump)
    {
        //are we moving in an upwards direction?
        if (playerVelocity.y > .1f)
        {
            Jumping = true;
        }
        //we are now falling
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
        //we hit ground
        else
        {
            Falling = false;
        }

        //fall faster if we're not holding the jump button
        if (!JumpHold)
        {
            ForceFall();
        }

        //the jump button has been pressed
        if (jump)
        {
            //if we're allowed to jump (can't remember why we're not using CanJump?)
            if ((Grounded || LedgeGrabbing) && !Jumping && !Falling && !JumpHold)
            {
                StopLedgeGrabbing();
                JumpHold = true;
                //Don't do a full jump if we're grabbing a ledge
                if (LedgeGrabbing)
                {
                    Jump(jumpHeight / 1.5f);
                    LedgeJumped = true;
                }
                else
                {
                    Jump(jumpHeight);
                }
                //we've crossed the center axis, we've changed direction
                if (playerVelocity.x == 0)
                {
                    airTurnCount++;
                }
            }
        }
        //we're no longer pressing the jump button
        else
        {
            JumpHold = false;
            //is this the first frame we're jumping on?
            if (Jumping && Grounded)
            {
                Jumping = false;
            }
        }
        //reset airTurnCount when we hit ground
        if (Grounded && !JumpHold)
        {
            airTurnCount = 0;
        }
    }

    /// <summary>
    /// Responsible for the simulated gravity
    /// </summary>
    private void Gravity()
    {
        rb.AddForce(Vector3.up * gravity);
    }

    /// <summary>
    /// This is just for visibility
    /// </summary>
    private void UpdateVelocity()
    {
        playerVelocity = new Vector2(Mathf.Round(rb.velocity.x), Mathf.Round(rb.velocity.y));
    }

    /// <summary>
    /// Do the walk logic in a certain direction
    /// </summary>
    /// <param name="dir"></param>
    private void Walk(float dir)
    {
        float gradualWalkSpeed = Mathf.Lerp(Mathf.Abs(rb.velocity.x), walkSpeed, (float)walkAcceleration / 10);
        if (gradualWalkSpeed > 5)
        {
            if (airTurnCount > 1)
            {
                gradualWalkSpeed = airwalkSpeed / (airTurnCount * 1.5f);
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

    /// <summary>
    /// Do the skid logic
    /// </summary>
    private void Skid()
    {
        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, 1 / 2), rb.velocity.y);
        if (Mathf.Abs(rb.velocity.x) <= .1f)
        {
            Skidding = false;
        }
    }

    /// <summary>
    /// Do the roll logic
    /// </summary>
    private void Roll()
    {
        if (rollTolerance == 1)
            return;

        rb.velocity = new Vector2(rollSpeed, rb.velocity.y);
    }

    /// <summary>
    /// Add the jump force
    /// </summary>
    /// <param name="force"></param>
    private void Jump(float force)
    {
        rb.AddForce(Vector2.up * force, ForceMode.Impulse);
    }

    /// <summary>
    /// Things that should happen the moment the player hits the ground
    /// </summary>
    private void HitGround()
    {
        jumpCount = 0;
        if (Falling)
        {
            GetComponent<AudioSource>().PlayOneShot(land);
        }
        Falling = false;
        Jumping = false;
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
        LedgeGrabbing = false;
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
