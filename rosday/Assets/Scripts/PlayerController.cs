using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    public Transform groundCheck;

    public float moveSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public int numJumps = 2;

    private bool facingRight = true;
    private bool isWalking;
    public bool grounded;

    private float movementInputDirection;
    private bool canJump;
    private int numJumpsLeft;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckFlipped();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
        CheckCanJump();
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(moveSpeed*movementInputDirection, rb.velocity.y);
    }

    private void CheckSurroundings()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void CheckFlipped()
    {
        if((facingRight && movementInputDirection < 0)
            || (!facingRight && movementInputDirection > 0))
        {
            Flip();
        }
        if (rb.velocity.x != 0)
        {
            isWalking = true;
        } else
        {
            isWalking = false;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void Jump()
    {
        if (canJump)
        {
            rb.velocity += new Vector2(0, jumpForce);
            numJumpsLeft--;
        }
    }

    private void CheckCanJump()
    {
        if (grounded)
        {
            numJumpsLeft = numJumps;
        }
        if (numJumpsLeft > 0)
        {
            canJump = true;
        } else
        {
            canJump = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
