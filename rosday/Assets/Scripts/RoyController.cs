using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyController : MonoBehaviour
{
    /* Components referenced: */
    private Rigidbody2D rb;
    private Animator anim;
    public Transform groundCheck;
    public Transform WallCheckTop;
    public Transform WallCheckMidA;
    public Transform WallCheckMidB;
    public Transform WallCheckBot;
    private BoxCollider2D bc;
    private float bcHeight;

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
    public LayerMask slideLM;

    /* Extra movement*/
    public float dashSpeed;
    public int dashLength;
    public int dashCooldown;
    private bool hasDash;

    /* Keep track of states during gameplay. */
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isWallSliding;
    private bool isGrounded;

    private bool isOnWall;
    private bool isNextToWall;
    private bool isByWall;

    private bool canGroundJump;
    private bool canAirJump;
    //private bool triedToJump;
    private int facingDirection = 1;
    private int lastWallDirection;

    private bool jumpedLastFrame;
    private bool canDash;
    private int dashTimer = 0;
    private int tillNextDash = 0;

    private bool isCrouching;
    public float crouchSpeed;
    private bool canInput = true;

    /* Inputs: */
    private float movementInputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallJumpDirection.Normalize();
        currMoveSpeed = moveSpeed;
        bc = GetComponent<BoxCollider2D>();
        bcHeight = bc.size.y;
    }

    void Update()
    {
        CheckInput();
        CheckAnimConditions();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
        CheckIfWallSliding();
        CheckMoveConditions();
        DashCont();
        ApplyMovement();
        ClampVelocityY();
        
    }

    private void CheckInput()
    {
        if (dashTimer > 0 || !canInput)
        {
            return;
        }
      
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        //if (Input.GetButtonDown("Jump") || triedToJump)
        if (Input.GetButtonDown("Jump"))
        {
            //triedToJump = false;
            Jump();
        }
        else if (Input.GetButtonUp("Jump") && jumpedLastFrame)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpDecreaseMultiplier);
        }
        else if (Input.GetButtonDown("Dash"))
        {
            DashStart();
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            Crouch();
        } else if (Input.GetAxisRaw("Vertical") >= 0)
        {
            Uncrouch();
        }
    }
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLM) || Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, slideLM);
        //isGrounded = Physics2D.Raycast(groundCheck.position, -transform.up, groundCheckDistance, groundLM);
        isOnWall = Physics2D.OverlapArea(WallCheckMidA.position, WallCheckMidB.position, groundLM);
        if (isOnWall)
        {
            lastWallDirection = facingDirection;
        }
        isNextToWall = Physics2D.Raycast(WallCheckMidA.position, -transform.right, wallCheckDistance + 0.05f, groundLM) || isOnWall;
        isByWall = Physics2D.Raycast(WallCheckTop.position, transform.right, wallCheckDistance, groundLM) ||
            Physics2D.Raycast(WallCheckBot.position, transform.right, wallCheckDistance, groundLM) || isOnWall;

        isByWall = Physics2D.Raycast(WallCheckTop.position, transform.right, wallCheckDistance, slideLM) ||
            Physics2D.Raycast(WallCheckBot.position, transform.right, wallCheckDistance, slideLM) || isByWall ||
            Physics2D.Raycast(WallCheckMidA.position, transform.right, wallCheckDistance, slideLM);
    }

    private void CheckMoveConditions()
    {
        tillNextDash = tillNextDash > 0 ? tillNextDash - 1 : 0;
        canGroundJump = isGrounded;
        canAirJump = isGrounded || canAirJump || isOnWall;
        canDash = (isGrounded || canDash || isOnWall) && tillNextDash == 0 && !(dashTimer > 0) && hasDash;
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
        anim.SetBool("isDashing", dashTimer > 0);
        anim.SetBool("isCrouching", isCrouching);
    }

    private void ApplyMovement()
    {
        if (dashTimer > 0 || !canInput)
        {
            return;
        }
        if (isGrounded)
        {
            if (movementInputDirection != 0)
            {
                    rb.AddForce(new Vector2(groundAcceleration * movementInputDirection, 0));
                    ClampVelocityX();
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
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
            rb.AddForce(new Vector2(airAcceleration * movementInputDirection, 0));
            ClampVelocityX();
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection == 0 && Mathf.Abs(rb.velocity.x) > 0)
        {
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
            //triedToJump = true;
        }

    }
    private void GroundedJump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -lastWallDirection, wallJumpForce * wallJumpDirection.y);
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
            //rb.position = new Vector2(rb.position.x, lastY);
            canDash = false;
        }
    }
    private void DashCont()
    {
        if (isByWall)
        {
            dashTimer = 0;
            return;
        }
        if (dashTimer > 1)
        {
            rb.velocity = new Vector2(dashSpeed * Mathf.Sign(rb.velocity.x), 9.81f * Time.fixedDeltaTime * 3);
            //rb.position = new Vector2(rb.position.x, lastY);
            dashTimer -= 1;
        }
        else if (dashTimer == 1)
        {
            rb.velocity = new Vector2(moveSpeed * Mathf.Sign(rb.velocity.x), 0);
            //rb.position = new Vector2(rb.position.x, lastY);
            dashTimer -= 1;
            tillNextDash = dashCooldown;
        }
    }

    private void Crouch()
    {
        isCrouching = true;
        bc.size = new Vector2(bc.size.x, 0.6902368f);
        bc.offset = new Vector2(bc.offset.x, 0.0326184f);
        currMoveSpeed = crouchSpeed;
    }

    private void Uncrouch()
    {
        isCrouching = false;
        bc.size = new Vector2(bc.size.x, bcHeight);
        bc.offset = new Vector2(bc.offset.x, -0.02939785f);
        currMoveSpeed = moveSpeed;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        RoyNPC interactable = collision.GetComponent<RoyNPC>();
        if (interactable != null && Input.GetButtonUp("Interact"))
        {
            interactable.Act();
        }
    }

    public void LockInputs()
    {
        canInput = false;
    }

    public void ReleaseInputs()
    {
        canInput = true;
    }

    public void UnlockDash()
    {
        hasDash = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
        Gizmos.DrawLine(WallCheckMidA.position, new Vector3(WallCheckMidA.position.x + wallCheckDistance, WallCheckMidA.position.y, WallCheckMidA.position.z));
    }
}