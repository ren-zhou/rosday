using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyController : MonoBehaviour
{
    /* Components referenced: */
    private Rigidbody2D rb;
    private Animator anim;
    public Transform groundCheck;
    public Transform wallCheck;

    /* Parameters to fiddle with: */
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;
    public float airJumpForce = 7.0f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLM;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceAir;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.5f;
    public Vector2 wallJumpDirection;
    public Vector2 wallHopDirection;
    public float wallJumpForce;
    public float wallHopForce;
    public float maxFallSpeed;
    public float dashSpeed;


    /* Keep track of states during gameplay. */
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isWallSliding;
    private bool isGrounded;
    private bool isOnWall;
    private bool canGroundJump;
    private bool canDoubleJump; 
    private int facingDirection = 1;
    private int lastWallDirection = 1;
    private bool isNextToWall;
    private bool jumpedLastFrame;

    /* Inputs: */
    private float movementInputDirection;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();

    }
    void Update()
    {
        CheckInput();
        CheckAnimConditions();
        UpdateAnimations();

    }

    private void FixedUpdate()
    {
        CheckIfWallSliding();
        CheckSurroundings();
        CheckCanJump();
        ApplyMovement();
        ClampVelocityY();
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        else if (Input.GetButtonUp("Jump") && jumpedLastFrame)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
        if (Input.GetButtonDown("Dash"))
        {
            Dash();
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLM);
        isOnWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLM);
        if (isOnWall)
        {
            lastWallDirection = facingDirection;
        }
        isNextToWall = Physics2D.Raycast(wallCheck.position, -transform.right, wallCheckDistance + 0.05f, groundLM) || isOnWall;
    }

    private void CheckCanJump()
    {
        canGroundJump = isGrounded;
        canDoubleJump = isGrounded || canDoubleJump || isOnWall;
    }

    private void CheckAnimConditions()
    {
        if ((isFacingRight && movementInputDirection < 0) ||
            (!isFacingRight && movementInputDirection > 0))
        {
            Flip();
        }
        isWalking = Mathf.Abs(rb.velocity.x) > 0.1f;
    }

    private void CheckIfWallSliding()
    {
        isWallSliding = isOnWall && !isGrounded && rb.velocity.y < 0;
    }
    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void ApplyMovement()
    {
        if (isGrounded)
        {
            if (movementInputDirection != 0)
            {
                if (BelowMaxSpeed())
                {
                    rb.AddForce(new Vector2(moveSpeed * 10 * movementInputDirection, 0));
                    ClampVelocityX();
                }
                
            } 
            else
            {
                rb.velocity = new Vector2(0.5f * rb.velocity.x, rb.velocity.y);
            }
            //ClampVelocityX(); 
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0 && BelowMaxSpeed())
        {
            rb.AddForce(new Vector2(movementForceAir * movementInputDirection, 0));
            ClampVelocityX();
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        if (isWallSliding && rb.velocity.y < -wallSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    private bool BelowMaxSpeed()
    {
        return rb.velocity.x < moveSpeed;
    }

    private void ClampVelocityX()
    {
        if (Mathf.Abs(rb.velocity.x) > moveSpeed)
        {
            rb.velocity = new Vector2(moveSpeed * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }
    }
    
    private void ClampVelocityY()
    {
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }
    }

    private void Dash()
    {
        if (movementInputDirection == 0)
        {
            rb.velocity = new Vector2(dashSpeed * facingDirection, 0);
            Debug.Log("Dash");
        } else
        {
            rb.velocity = new Vector2(dashSpeed * movementInputDirection, 0);
            Debug.Log("Dash");
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
        facingDirection *= -1;
    }

    private void Jump()
    {
        jumpedLastFrame = true;
        if (canGroundJump)
        {
            GroundedJump();
        } 
        else if (isOnWall || isWallSliding || isNextToWall) {
            WallJump();
        } 
        else if (canDoubleJump)
        {
            AirJump();
        } else
        {
            jumpedLastFrame = false;
        }
    }

    private void GroundedJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void WallJump()
    {
        Vector2 forceToAdd;
        //if (movementInputDirection == 0 && isWallSliding)
        forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -lastWallDirection, wallJumpForce * wallJumpDirection.y);
        Debug.Log("wall jump");
        Debug.Log(forceToAdd);
        
        isWallSliding = false;
        rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        Flip();
    }

    private void AirJump()
    {
        if (!isOnWall && !isWallSliding) {
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, airJumpForce);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }   
}
