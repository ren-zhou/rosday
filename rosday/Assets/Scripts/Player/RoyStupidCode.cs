using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PushState;

public class RoyStupidCode : MonoBehaviour
{
    private RoyMove rm;

    private Animator anim;
    public Transform groundCheck;
    public Transform WallCheckA;
    public Transform WallCheckB;
    private CameraBounder cb;


    /* Surrounding Check Related: */
    public float groundCheckDistance = 0.3f;
    public float wallCheckDistance;
    public LayerMask groundLM;
    public LayerMask slideLM;

    public float gravityAcceleration;

    /* Keep track of states during gameplay. */
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isWallSliding;
    private bool isGrounded;
    private int pfsg;

    private bool isOnWall;
    private bool isNextToWall;

    private bool canGroundJump;
    private bool canAirJump;
    private int lastWallDirection;
    private bool canInput;

    private float moveInputDir;

    private bool jumpedLastFrame;
    private bool isCrouching;
    public int jumpLeeway;
    private PushPull pushll;
    private BoxCollider2D bc;

    public Vector2 velocity;
    void Start()
    {
        rm = GetComponent<RoyMove>();
        anim = GetComponent<Animator>();
        pfsg = 0;
        pushll = GetComponent<PushPull>();
        canInput = true;

    }

    // Update is called once per frame
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
        ApplyMovement();
        rm.CapFallSpeed();
        DoStuff()

    }

    private void DoStuff()
    {
        velocity = rm.GetVel();
        Gravity();
        Move();
        ResolveHitboxes();
    }

    void Gravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravityAcceleration * Time.fixedDeltaTime;
        }


    }
    void Move()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    void ResolveHitboxes()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, bc.size, 0);

        foreach (Collider2D hit in hits)
        {
            if (hit == bc)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(bc);

            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                velocity.x = 0;
            }
        }
    }

    public int FacingDirection()
    {
        return isFacingRight ? 1 : -1;
    }

    private void CheckInput()
    {
        if (!canInput)
        {
            return;
        }
        moveInputDir = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            rm.Uncrouch();
            Jump();
        }
        else if (Input.GetButtonUp("Jump") && jumpedLastFrame)
        {
            rm.ShortenJump();
            jumpedLastFrame = false;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (isGrounded)
            {
                Crouch();
            }
        }
        else if (Input.GetAxisRaw("Vertical") >= 0)
        {
            if (isCrouching)
            {
                Uncrouch();
            }
        }
        if (Input.GetButtonDown("Push"))
        {
            pushll.currAction = Pushing;
        }
        else if (Input.GetButtonDown("Pull"))
        {
            pushll.currAction = Pulling;
        }
        else if ((Input.GetButtonUp("Push") && pushll.currAction == Pushing)
            || (Input.GetButtonUp("Pull") && pushll.currAction == Pulling))
        {
            pushll.currAction = None;
        }
    }

    private void Crouch()
    {
        isCrouching = true;
        rm.Crouch();
    }

    private void Uncrouch()
    {
        isCrouching = false;
        rm.Uncrouch();
    }

    private void CheckIfWallSliding()
    {
        isWallSliding = isOnWall && !isGrounded && (velocity.y <= 0) && (moveInputDir == FacingDirection() || isWallSliding);
    }

    private void CheckAnimConditions()
    {
        if ((isFacingRight && moveInputDir < 0) ||
            (!isFacingRight && moveInputDir > 0))
        {
            Flip();
        }
        isWalking = Mathf.Abs(velocity.x) > 0.1f;
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLM) || Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, slideLM);
        pfsg = isGrounded ? 0 : pfsg + 1;
        isOnWall = Physics2D.OverlapArea(WallCheckA.position, WallCheckB.position, groundLM);
        if (isOnWall)
        {
            lastWallDirection = FacingDirection();
        }
        isNextToWall = Physics2D.Raycast(WallCheckA.position, -transform.right, wallCheckDistance + 0.05f, groundLM) || isOnWall;
    }

    private void CheckMoveConditions()
    {
        canGroundJump = isGrounded || pfsg < jumpLeeway;
        canAirJump = isGrounded || canAirJump || isOnWall;
    }

    private void Jump()
    {
        jumpedLastFrame = true;
        if (canGroundJump)
        {
            rm.GroundedJump();
        }
        else if (isOnWall || isWallSliding || isNextToWall)
        {
            rm.WallJump(-lastWallDirection);
            isWallSliding = false;
            Flip();
        }
        else
        {
            jumpedLastFrame = false;
        }
    }

    public void LockInputs()
    {
        canInput = false;
    }

    public void LockInputs(float seconds)
    {
        LockInputs();
        StartCoroutine(Wait(seconds));

    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReleaseInputs();
    }
    public void ReleaseInputs()
    {
        canInput = true;
    }

    private bool BelowMaxSpeed()
    {
        return velocity.x < rm.CurrentSpeed();
    }

    public void ApplyMovement()
    {
        if (isGrounded)
        {
            if (moveInputDir != 0 && BelowMaxSpeed())
            {
                rm.AccelerateOnGround(moveInputDir);
            }
            else if (Mathf.Abs(velocity.x) > 0)
            {
                rm.DecelerateOnGround();
            }
        }
        else if (!isGrounded && !isWallSliding && moveInputDir != 0 && BelowMaxSpeed())
        {
            rm.AccelerateInAir(moveInputDir);
        }
        else if (!isGrounded && !isWallSliding && moveInputDir == 0 && Mathf.Abs(velocity.x) > 0)
        {
            rm.DecelerateInAir();
        }
        if (isWallSliding)
        {
            rm.WallSlide();

        }
        if (!isGrounded && isCrouching)
        {
            rm.Uncrouch();
        }
    }

    /// <summary>
    /// Flips the sprite and also flips conditions associated with facing direction.
    /// </summary>
    public void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isCrouching", isCrouching);
    }

    public void Die()
    {
        moveInputDir = 0;
        jumpedLastFrame = false;
        pfsg = 1000;
        if (!isFacingRight)
        {
            Flip();
        }
        LockInputs(0.75f);
    }

    public void OnDrawGizmos()
    {
        //UnityEditor.Handles.color = Color.yellow;
        //UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, 1f);
        //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + 11f, transform.position.y + 6.2f, transform.position.z));
    }

    public void ZeroAll()
    {
        velocity = Vector2.zero;
        pushll.ZeroAction();
        moveInputDir = 0;
    }
}
