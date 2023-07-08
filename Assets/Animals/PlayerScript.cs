using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Rigidbody2D body;
    Animator anim;
    SpriteRenderer sprRender;

    private float horizontal;
    private float vertical;
    private float moveLimiter = 0.7f;

    private float runSpeed = 1.0f;
    public int hp = 3;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // anim.SetBool("h_walking", horizontal != 0); 
        // anim.SetBool("d_walking", vertical == -1);
        // anim.SetBool("u_walking", vertical == 1);

        if (horizontal == -1) { 
            sprRender.flipX = true; 
        }
        else {
            sprRender.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) {// Check for diagonal movement
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
}
