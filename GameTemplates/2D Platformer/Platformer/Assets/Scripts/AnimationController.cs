using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    Animator anim;
    [SerializeField]
    SpriteRenderer sprite;
    
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sprite = null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
    }

    public void SetDirection(bool right)
    {
        sprite.flipX = !right;
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
