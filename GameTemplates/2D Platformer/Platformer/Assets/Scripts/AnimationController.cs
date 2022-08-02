using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    Animator anim;
    [SerializeField]
    SpriteRenderer sprite;

    [SerializeField] //for temp use until sprite is set
    GameObject player;

    private bool animate = false;
    
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            animate = true;
        }
    }

    public void SetDirection(bool right)
    {
        if (animate)
        {
            sprite.flipX = !right;
        }
        else
        {
            player.transform.localEulerAngles = Vector3.up * (right ? 0 : 180);
        }
    }

    public void Walk(bool walking)
    {
        anim.SetBool("Walking", walking);
    }

    public void Jump(bool jumping)
    {
        anim.SetBool("Jumping", jumping);
    }

    public void Falling(bool falling)
    {
        anim.SetBool("Falling", falling);
    }

    public void Grounded()
    {

    }
}
