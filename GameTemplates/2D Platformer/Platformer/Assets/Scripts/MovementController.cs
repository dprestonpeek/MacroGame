using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    enum Direction { Left, Right }
    Direction direction = Direction.Right;

    [SerializeField]
    public Rigidbody rb;

    [SerializeField]
    AnimationController anim;

    [SerializeField]
    AudioClip land;

    [SerializeField]
    bool allowMovement = true;

    bool CanJump = false;

    bool Walking = false;
    bool Jumping = false;
    bool Falling = false;

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
    private int walkSpeed = 5;
    [SerializeField]
    [Range(1, 10)]
    private int jumpSpeed = 7;
    [SerializeField]
    [Range(1, 10)]
    private int jumpHeight = 6;
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
    public void Update()
    {
        if (allowMovement)
        {
            CheckWalk();
        }
    }

    void CheckWalk()
    {
        if (Mathf.Abs(GameInput.lJoystick.x) > .25f)
        {
            Walk(GameInput.lJoystick.x);
            Walking = true;
        }
        else
        {
            Walking = false;
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

    void CalculateDirection(float dir)
    {
        if (dir > 0)
        {
            direction = Direction.Right;
        }
        if (dir < 0)
        {
            direction = Direction.Left;
        }
    }

    bool IsGrounded()
    {
        RaycastHit2D hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        // Does the ray intersect any objects
        if (hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, layerMask))
        {
            if (hit.transform.CompareTag("Floor") || hit.transform.CompareTag("Wall"))
            {
                Debug.DrawRay(transform.position, Vector2.down * 1.1f, Color.yellow);
                return true;
            }
        }
        else
        {
            return false;
        }
        return false;
    }
}
