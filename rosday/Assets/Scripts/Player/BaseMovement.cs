using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    /* Components referenced: */
    private Rigidbody2D rb;

    /* Grounded Movement: */
    public float defaultSpeed;
    public float currSpeed;
    public float groundAcceleration;
    public float groundDeceleration;

    /* Aerial Movement: */
    public float airAcceleration;
    public float airDeceleration;
    public float maxFallSpeed;

    /* Jump or Wall Related: */
    public float jumpForce = 10.0f;
    public Vector2 wallJumpDirection;
    public float wallSlideSpeed;
    public float wallJumpForce;
    public float jumpDecreaseMultiplier;
    public float crouchSpeed;

    private BoxCollider2D bc;
    private Vector2 bcSize;
    private Vector2 bcOffset;

    private Vector3 respawnPos;



    public void ShortenJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpDecreaseMultiplier);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallJumpDirection.Normalize();
        currSpeed = defaultSpeed;
        respawnPos = transform.position;
    }


    public void Crouch()
    {
            //bc.size = bcSize;
            //bc.offset = bcOffset;
            currSpeed = crouchSpeed;
    }



    /// <summary>
    /// Undoes what crouch does. Sets the animation condition, resets the box collider, ups the move
    /// speed to normal.
    /// </summary>
    public void Uncrouch()
    {
            //bc.size = bcSize;
            //bc.offset = bcOffset;
            currSpeed = defaultSpeed;
    }


    /// <summary>
    /// Applies movement unless inputs are somehow locked. Deals with horizontal movement (aerial and grounded), including
    /// acceleration and decceleration, applies wallsliding, caps fall speed, and uncrouches if the player is not grounded.
    /// </summary>


    public void WallSlide()
    {
        if (rb.velocity.y < -wallSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    public void DecelerateInAir()
    {
        float dir = Mathf.Sign(rb.velocity.x);
        rb.velocity = new Vector2(rb.velocity.x - airDeceleration * dir, rb.velocity.y);
        if (Mathf.Sign(rb.velocity.x) != dir && Mathf.Sign(rb.velocity.x) != 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void AccelerateOnGround(float dir)
    {
        rb.AddForce(new Vector2(groundAcceleration * dir, 0));
        ClampVelocityX();
    }

    public void AccelerateInAir(float dir)
    {
        rb.AddForce(new Vector2(airAcceleration * dir, 0));
        ClampVelocityX();
    }

    public void DecelerateOnGround()
    {
        float dir = Mathf.Sign(rb.velocity.x);
        rb.velocity = new Vector2(rb.velocity.x - groundDeceleration * dir, rb.velocity.y);
        if (Mathf.Sign(rb.velocity.x) != dir && Mathf.Sign(rb.velocity.x) != 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public float CurrentSpeed()
    {
        return currSpeed;
    }

    /// <summary>
    /// Clamps the x velocity to be within the current move speed cap.
    /// </summary>
    public void ClampVelocityX()
    {
        if (Mathf.Abs(rb.velocity.x) > currSpeed)
        {
            rb.velocity = new Vector2(currSpeed * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }
    }

    /// <summary>
    /// Clamps downwards y velocity only. Capped at the max fall speed.
    /// </summary>
    public void CapFallSpeed()
    {
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }
    }



    public void GroundedJump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    public void WallJump(float dir)
    {
        Vector2 jumpForce = new Vector2(wallJumpForce * wallJumpDirection.x * dir, wallJumpForce * wallJumpDirection.y);
        rb.velocity = Vector2.zero;
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
    }


    public void ZeroXVelocity()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void SetSpawn(Transform point)
    {
        respawnPos = point.position;
    }




    public void Die()
    {
        rb.transform.position = respawnPos;
    }
}

