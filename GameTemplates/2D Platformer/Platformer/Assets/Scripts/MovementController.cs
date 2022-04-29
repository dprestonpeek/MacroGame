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

    public bool CanJump = false;

    bool Walking = false;
    public bool Jumping = false;
    bool Falling = false;
    bool JumpHold = false;
    bool CanHold = false;

    public float xVelocity;
    public float yVelocity;
    [SerializeField]
    public float jumpVelocity;
    [SerializeField]
    public float fallVelocity;
    [SerializeField]
    public float walkVelocity;
    [SerializeField]
    public float rightJoystickMag;

    private int jumpCount = 0;

    [SerializeField]
    [Range(1, 10)]
    private int walkSpeed = 7;
    [SerializeField]
    [Range(1, 10)]
    private int jumpSpeed = 9;
    [SerializeField]
    [Range(1, 10)]
    private int jumpHeight = 10;
    [SerializeField]
    [Range(1, 10)]
    private int extraHeight = 6;
    [SerializeField]
    [Range(1, 10)]
    private int extraHeightSpeed = 1;
    [SerializeField]
    [Range(1, 10)]
    private int floatyFall = 3;
    [SerializeField]
    [Range(1, 15)]
    private int fallSpeed = 3;

    // Start is called before the first frame update
    public void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (allowMovement)
        {
            CheckWalk();
            CheckJump();
        }
    }

    void CheckWalk()
    {
        float lJoystickX = GameInput.lJoystick.x;
        if (Mathf.Abs(lJoystickX) > .25f)
        {
            Walk(lJoystickX);
            CalculateDirection(lJoystickX);
            Walking = true;
        }
        else
        {
            Walking = false;
        }
    }

    void CheckJump()
    {
        if (yVelocity > .1f)
        {
            Jumping = true;

            if (CanHold && JumpHold )
            {
                //AddHeightToJump();
            }
            else
            {
                CanHold = false;
            }
        }
        else if (yVelocity < -.1f)
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
        if (Falling)
        {
            fallVelocity = yVelocity;
            CanHold = false;
        }

        if (!JumpHold)
        {
            ForceFall();
        }

        if (GameInput.cross || GameInput.circle)
        {
            if (IsGrounded() && !Jumping && !Falling && !JumpHold)
            {
                JumpHold = true;
                Jump(jumpHeight);
            }
        }
        else
        {
            JumpHold = false;
        }
    }

    public void Walk(float dir)
    {
        float airwalkModifier = 1;
        if (Jumping || Falling)
        {
            airwalkModifier -= (Mathf.Abs(yVelocity) / 50);
        }
        rb.velocity = new Vector2(dir * airwalkModifier * walkSpeed, rb.velocity.y);
        walkVelocity = xVelocity;
    }
    public void Jump(float force)
    {
        rb.AddForce(Vector2.up * force, ForceMode.Impulse);
    }

    public void HitGround()
    {
        jumpVelocity = 0;
        jumpCount = 0;
        if (Falling)
        {
            GetComponent<AudioSource>().PlayOneShot(land);
        }
        Falling = false;
        Jumping = false;
        anim.Grounded();
    }

    void AddHeightToJump()
    {
        if (rb.velocity.y < jumpHeight)
        {
            rb.AddForce(Vector2.up * jumpSpeed * (extraHeight / 3));
        }
    }

    void FloatFall()
    {
        rb.AddForce(Vector2.up * 2 * floatyFall);
    }

    void ForceFall()
    {
        rb.AddForce(Vector2.down * 5 * fallSpeed);
    }

    Direction CalculateDirection(float dir)
    {
        if (dir > 0)
        {
            direction = Direction.Right;
            return Direction.Right;
        }
        if (dir < 0)
        {
            direction = Direction.Left;
            return Direction.Left;
        }
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

    bool IsGrounded()
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
            if (Physics.Raycast(position, Vector3.down, out hit, 1.1f, layerMask))
            {
                if (hit.transform.CompareTag("Floor") || hit.transform.CompareTag("Wall"))
                {
                    Debug.DrawRay(position, Vector2.down * 1.1f, Color.yellow);
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
            IsGrounded();
        }
    }
}
