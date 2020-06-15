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
    private Vector2 bcSize;
    private Vector2 bcOffset;

    /* Parameters to fiddle with: */

    /* Grounded Movement: */
    /** The default max move speed of the player. */
    public float moveSpeed = 7.0f;
    /** The current max move speed of the player. */
    public float currMoveSpeed;
    public float accelerationBuff;
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
    private HashSet<string> abilitySet;

    /* Keep track of states during gameplay. */
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isWallSliding;
    private bool isGrounded;

    private bool isOnWall;
    private bool isNextToWall;
    private bool isBlockedByWall;

    private bool canGroundJump;
    private bool canAirJump;
    //private bool triedToJump;
    private int facingDirection = 1;
    private int lastWallDirection;

    private bool jumpedLastFrame;
    private bool canDash;
    private int dashTimer = 0;
    private int tillNextDash = 0;

    public Stall stall;

    private bool isCrouching;
    public float crouchSpeed;
    private bool canInput = true;

    private Vector3 respawnPos;

    /* Inputs: */
    private float movementInputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wallJumpDirection.Normalize();
        currMoveSpeed = moveSpeed;
        bc = GetComponent<BoxCollider2D>();
        bcSize = bc.size;
        bcOffset = bc.offset;
        accelerationBuff = 1;
        abilitySet = new HashSet<string>();
        respawnPos = transform.position;
        UnlockAbility("stall");
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
    /// <summary>
    /// Handles all inputs. Stores the arrow keys as a movement input and directly calls methods for the other inputs.
    /// </summary>
    private void CheckInput()
    {
        if (!canInput)
        {
            return;
        }
        if (Input.GetButtonDown("Stall"))
        {
            stall.StartStall();
            dashTimer = 0;
        }
        if (dashTimer > 0)
        {
            return;
        }
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        //if (Input.GetButtonDown("Jump") || triedToJump)
        if (Input.GetButtonDown("Jump"))
        {
            Uncrouch();
            //triedToJump = false;
            Jump();
        }
        else if (Input.GetButtonUp("Jump") && jumpedLastFrame)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpDecreaseMultiplier);
        }
        else if (Input.GetButtonDown("Dash"))
        {
            Uncrouch();
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

    /// <summary>
    /// Checks if the player is grounded, is on a wall (near wall and facing it), next to a wall (near wall but not necessarily facing),
    /// which direction the last wall the player was next to was, and if the player is blocked by a wall (used for stopping the player's dash).
    /// </summary>
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLM) || Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, slideLM);
        isOnWall = Physics2D.OverlapArea(WallCheckMidA.position, WallCheckMidB.position, groundLM);
        if (isOnWall)
        {
            lastWallDirection = facingDirection;
        }
        isNextToWall = Physics2D.Raycast(WallCheckMidA.position, -transform.right, wallCheckDistance + 0.05f, groundLM) || isOnWall;
        isBlockedByWall = Physics2D.Raycast(WallCheckTop.position, transform.right, wallCheckDistance, groundLM) ||
            Physics2D.Raycast(WallCheckBot.position, transform.right, wallCheckDistance, groundLM) || isOnWall;

        isBlockedByWall = Physics2D.Raycast(WallCheckTop.position, transform.right, wallCheckDistance, slideLM) ||
            Physics2D.Raycast(WallCheckBot.position, transform.right, wallCheckDistance, slideLM) || isBlockedByWall ||
            Physics2D.Raycast(WallCheckMidA.position, transform.right, wallCheckDistance, slideLM);
    }

    /// <summary>
    /// Checks the dash cooldown, and whether the conditions to jump and dash are met.
    /// </summary>
    private void CheckMoveConditions()
    {
        tillNextDash = tillNextDash > 0 ? tillNextDash - 1 : 0;
        canGroundJump = isGrounded;
        canAirJump = isGrounded || canAirJump || isOnWall;
        canDash = (isGrounded || canDash || isOnWall) && tillNextDash == 0   && HasAbility("dash");
    }

    /// <summary>
    /// Checks animation conditions before they are sent to the controller.
    /// </summary>
    private void CheckAnimConditions()
    {
        if ((isFacingRight && movementInputDirection < 0) ||
            (!isFacingRight && movementInputDirection > 0))
        {
            Flip();
        }
        isWalking = Mathf.Abs(rb.velocity.x) > 0.1f;
    }

    /// <summary>
    /// Checks if the player meets the conditions for wallsliding.
    /// </summary>
    private void CheckIfWallSliding()
    {
        isWallSliding = isOnWall && !isGrounded && rb.velocity.y < 0 && (movementInputDirection == facingDirection || isWallSliding);
    }
    /// <summary>
    /// Updates the animation controller.
    /// </summary>
    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isDashing", dashTimer > 0);
        anim.SetBool("isCrouching", isCrouching);
    }
    
    /// <summary>
    /// Applies movement unless inputs are somehow locked. Deals with horizontal movement (aerial and grounded), including
    /// acceleration and decceleration, applies wallsliding, caps fall speed, and uncrouches if the player is not grounded.
    /// </summary>
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
                    rb.AddForce(new Vector2(groundAcceleration * accelerationBuff * movementInputDirection, 0));
                    ClampVelocityX();
            }
            else if (Mathf.Abs(rb.velocity.x) > 0)
            {
                float dir = Mathf.Sign(rb.velocity.x);
                rb.velocity = new Vector2(rb.velocity.x - groundDeceleration * dir, rb.velocity.y);
                if (Mathf.Sign(rb.velocity.x) != dir && Mathf.Sign(rb.velocity.x) != 0)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
            rb.AddForce(new Vector2(airAcceleration * accelerationBuff * movementInputDirection, 0));
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
        if (!isGrounded && isCrouching)
        {
            Uncrouch();
        }
    }

    /// <summary>
    /// Clamps the x velocity to be within the current move speed cap.
    /// </summary>
    private void ClampVelocityX()
    {
        if (Mathf.Abs(rb.velocity.x) > currMoveSpeed)
        {
            rb.velocity = new Vector2(currMoveSpeed * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }
    }

    /// <summary>
    /// Clamps downwards y velocity only. Capped at the max fall speed.
    /// </summary>
    private void ClampVelocityY()
    {
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }
    }

    /// <summary>
    /// Flips the sprite and also flips conditions associated with facing direction.
    /// </summary>
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
        facingDirection *= -1;
    }

    /// <summary>
    /// Master jump method. Uses info about surroundings to determine which jump is performed.
    /// </summary>
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

    public bool AnyRefreshConditions()
    {
        return isGrounded || isOnWall;
    }

    /// <summary>
    /// Begins the dash by setting the timer to the length and setting the player's velocity to the dash velocity.
    /// Uses up the dash.
    /// </summary>
    private void DashStart()
    { 
        if (canDash)
        {
            dashTimer = dashLength;
            float dashDir = movementInputDirection == 0 ? facingDirection : movementInputDirection;
            rb.velocity = new Vector2(dashSpeed * dashDir, 0);
            canDash = false;
      
        }
    }

    /// <summary>
    /// Continues and finishes the dash. Is called every frame of the dash (after the first) and maintains the forwards
    /// velocity as well as providing enough upwards velocity to cancel out the effect of gravity. On the last physics frame
    /// of the dash, starts the dash cooldown. On the last frame, the player continues moving at their maximum move speed (not dash).
    /// </summary>
    private void DashCont()
    {
        if (isBlockedByWall)
        {
            dashTimer = 0;
            return;
        }
        if (dashTimer > 1)
        {
            rb.velocity = new Vector2(dashSpeed * Mathf.Sign(rb.velocity.x), 9.81f * Time.fixedDeltaTime * 3);
            dashTimer -= 1;
        }
        else if (dashTimer ==  1)
        {
            rb.velocity = new Vector2(moveSpeed * Mathf.Sign(rb.velocity.x), 0);
            dashTimer -= 1;
            tillNextDash = dashCooldown;
        }
    }

    /// <summary>
    /// Begins the crouch by setting the collider to be smaller and the offset to be lower. Also
    /// slows down the move speed to the crouch speed.
    /// </summary>
    private void Crouch()
    {
        if (isGrounded)
        {
            isCrouching = true;
            bc.size = bcSize;
            bc.offset = bcOffset;
            currMoveSpeed = crouchSpeed;
        }
    }



    /// <summary>
    /// Undoes what crouch does. Sets the animation condition, resets the box collider, ups the move
    /// speed to normal.
    /// </summary>
    private void Uncrouch()
    {
        if (isCrouching)
        {
            isCrouching = false;
            bc.size = bcSize;
            bc.offset = bcOffset;
            currMoveSpeed = moveSpeed;
        }
    }


    /// <summary>
    /// Locks player out of inputs by making it so that check inputs does not run.
    /// </summary>
    public void LockInputs()
    {
        canInput = false;
    }
    public void ReleaseInputs()
    {
        canInput = true;
    }
    public void ZeroXVelocity()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    /// <summary>
    /// Marks an ability as owned by adding it to a set of owned abilities.
    /// </summary>
    /// <param name="ability"></param>
    public void UnlockAbility(string ability)
    {
        abilitySet.Add(ability);
    }




    /// <summary>
    /// Returns whether the ability is owned.
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    public bool HasAbility(string ability)
    {
        return abilitySet.Contains(ability);
    }


    public void Die()
    {
        rb.transform.position = respawnPos;
    }
}