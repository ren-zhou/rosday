using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyMove : MonoBehaviour
{
    private Vector2 velocity;

    /* Grounded Movement: */
    public float defaultSpeed;
    public float currSpeed;
    public float groundAcceleration;
    public float groundDeceleration;

    public float jumpHeight;
    public float gravityAcceleration;

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

    private float currWalkx;

    void Start()
    {
        velocity = Vector2.zero;
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

    public void Uncrouch()
    {
        //bc.size = bcSize;
        //bc.offset = bcOffset;
        currSpeed = defaultSpeed;
    }

    public void WallSlide()
    {
        if (velocity.y < -wallSlideSpeed)
        {
            velocity = new Vector2(velocity.x, -wallSlideSpeed);
        }
    }

    public void DecelerateOnGround()
    {
        float dir = Mathf.Sign(velocity.x);
        velocity = new Vector2(velocity.x - groundDeceleration * dir, velocity.y);
        if (Mathf.Sign(velocity.x) != dir && Mathf.Sign(velocity.x) != 0)
        {
            velocity = new Vector2(0, velocity.y);
        }
    }

    public void DecelerateInAir()
    {
        float dir = Mathf.Sign(velocity.x);
        velocity = new Vector2(velocity.x - airDeceleration * dir, velocity.y);
        if (Mathf.Sign(velocity.x) != dir && Mathf.Sign(velocity.x) != 0)
        {
            velocity = new Vector2(0, velocity.y);
        }
    }


    public void AccelerateOnGround(float dir)
    {
        currWalkx = currWalkx == 0 ? currSpeed * dir : 0;
        velocity.x += currWalkx;
    }

    public void AccelerateInAir(float dir)
    {
        AccelerateOnGround(dir);
    }

    public void CapFallSpeed()
    {
        if (velocity.y < -maxFallSpeed)
        {
            velocity = new Vector2(velocity.x, -maxFallSpeed);
        }
    }

    public void GroundedJump()
    {
        velocity.y = Mathf.Sqrt(2 * jumpHeight * gravityAcceleration);
    }

    public void WallJump(float dir)
    {
        Vector2 jumpForce = new Vector2(wallJumpForce * wallJumpDirection.x * dir, wallJumpForce * wallJumpDirection.y);
        velocity = Vector2.zero;
        velocity = jumpForce;
    }

    public void ZeroXVelocity()
    {
        velocity.x = 0;
    }

    public void SetSpawn(Transform point)
    {
        respawnPos = point.position;
    }

    public void ShortenJump()
    {
        velocity = new Vector2(velocity.x, velocity.y * jumpDecreaseMultiplier);
    }

    public float CurrentSpeed()
    {
        return currSpeed;
    }


    public void Die()
    {
        transform.position = respawnPos;
    }

    public Vector2 GetVel()
    {
        return velocity;
    }
}
