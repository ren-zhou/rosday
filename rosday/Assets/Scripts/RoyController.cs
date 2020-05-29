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

    /* Grounded Movement: */
    public float moveSpeed = 5.0f;
    public float currMoveSpeed;
    public float groundAcceleration;
    public float groundDeceleration;

    /* Aerial Movement: */
    public float airAcceleration;
    public float airDeceleration;
    public float maxFallSpeed;

    /* Jump or Wall Related: */
    public float jumpForce = 10.0f;
    public float airJumpForce = 7.0f;
    public Vector2 wallJumpDirection;
    public float wallSlideSpeed;
    public float wallJumpForce;
    public float jumpDecreaseMultiplier;

    /* Surrounding Check Related: */
    public float groundCheckDistance = 0.3f;
    public float wallCheckDistance;
    public LayerMask groundLM;

    /* Extra movement*/
    public float dashSpeed;
    public int dashLength;

    /* Keep track of states during gameplay. */
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isWallSliding;
    private bool isGrounded;
    private bool isOnWall;
    private bool canGroundJump;
    private bool canAirJump;
    private int facingDirection = 1;
    private int lastWallDirection;
    private bool isNextToWall;
    private bool jumpedLastFrame;
    private bool canDash;
    private int dashTimer;
    private bool triedToJump;

    /* Inputs: */
    private float movementInputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallJumpDirection.Normalize();
        currMoveSpeed = moveSpeed;
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
        CheckMoveConditions();
        DashCont();
        ApplyMovement();
        ClampVelocityY();

    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") || triedToJump)
        {
            triedToJump = false;
            Jump();
        }
        else if (Input.GetButtonUp("Jump") && jumpedLastFrame)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpDecreaseMultiplier);
        }
        if (Input.GetButtonDown("Dash"))
        {
            DashStart();
        }

    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLM);
        //isGrounded = Physics2D.Raycast(groundCheck.position, -transform.up, groundCheckDistance, groundLM);
        isOnWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLM);
        if (isOnWall)
        {
            lastWallDirection = facingDirection;
        }
        isNextToWall = Physics2D.Raycast(wallCheck.position, -transform.right, wallCheckDistance + 0.05f, groundLM) || isOnWall;
    }

    private void CheckMoveConditions()
    {
        canGroundJump = isGrounded;
        canAirJump = isGrounded || canAirJump || isOnWall;
        canDash = isGrounded || canDash || isOnWall;
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
        isWallSliding = isOnWall && !isGrounded && rb.velocity.y < 0 && (movementInputDirection == facingDirection || isWallSliding);
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
        if (dashTimer > 0)
        {
            return;
        }
        if (isGrounded)
        {
            if (movementInputDirection != 0)
            {
                if (BelowMaxSpeed())
                {
                    rb.AddForce(new Vector2(groundAcceleration * movementInputDirection, 0));
                    ClampVelocityX();
                }
            }
            else if (Mathf.Abs(rb.velocity.x) > 0)
            {
                float dir = Mathf.Sign(rb.velocity.x);
                rb.velocity = new Vector2(rb.velocity.x - groundDeceleration*dir, rb.velocity.y);
                if (Mathf.Sign(rb.velocity.x) != dir && Mathf.Sign(rb.velocity.x) != 0)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0 && BelowMaxSpeed())
        {
            rb.AddForce(new Vector2(airAcceleration * movementInputDirection, 0));
            ClampVelocityX();
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection == 0 && Mathf.Abs(rb.velocity.x) > 0)
        {
            Debug.Log(rb.velocity.x);
            float dir = Mathf.Sign(rb.velocity.x);
            rb.velocity = new Vector2(rb.velocity.x - airDeceleration * dir, rb.velocity.y);
            if (Mathf.Sign(rb.velocity.x) != dir && Mathf.Sign(rb.velocity.x) != 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        if (isWallSliding && rb.velocity.y < -wallSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    private bool BelowMaxSpeed()
    {
        return rb.velocity.x < currMoveSpeed;
    }

    private void ClampVelocityX()
    {
        if (Mathf.Abs(rb.velocity.x) > currMoveSpeed)
        {
            rb.velocity = new Vector2(currMoveSpeed * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }
    }

    private void ClampVelocityY()
    {
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
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
        else if (isOnWall || isWallSliding || isNextToWall)
        {
            WallJump();
        }
        else if (canAirJump)
        {
            AirJump();
        }
        else
        {
            jumpedLastFrame = false;
            Debug.Log("didn'tJumpLastFrame");
            triedToJump = true;
        }

    }
    private void GroundedJump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -lastWallDirection, wallJumpForce * wallJumpDirection.y);
        Debug.Log("wall jump");
        Debug.Log(forceToAdd);
        isWallSliding = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        Flip();
    }

    private void AirJump()
    {
        if (!isOnWall && !isWallSliding)
        {
            canAirJump = false;
            rb.velocity = new Vector2(rb.velocity.x, airJumpForce);
        }

    }
    private void DashStart()
    { 
        if (canDash)
        {
            dashTimer = dashLength;
            float dashDir = movementInputDirection == 0 ? facingDirection : movementInputDirection;
            Vector2 yeet = new Vector2(dashSpeed * dashDir, 0);
            rb.velocity = new Vector2(dashSpeed * dashDir, 0);
            Debug.Log(dashDir);
            Debug.Log(yeet);
            canDash = false;
        }
    }
    private void DashCont()
    {
        if (dashTimer > 1)
        {
            Debug.Log(Mathf.Sign(rb.velocity.x));
            dashTimer -= 1;
            rb.velocity = new Vector2(dashSpeed * Mathf.Sign(rb.velocity.x), 0);

        }
        else if (dashTimer == 1)
        {
            rb.velocity = new Vector2(moveSpeed * Mathf.Sign(rb.velocity.x), 0);
            dashTimer -= 1;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}