using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidFuckingCode : MonoBehaviour
{

    private float walkSpeedMax;
    private float walkSpeedCurr;
    public float walkSpeedDef;
    public float walkAccelerationDef;
    private float walkAccelerationCurr;
    private float externalSpeed;
    public float groundDeceleration;
    private Vector2 velocity;

    public Transform WallCheckA;
    public Transform WallCheckB;
    public Transform groundCheck;
    private bool isOnWall;

    public float groundCheckDistance = 0.3f;
    public float wallCheckDistance;
    public LayerMask groundLM;
    public LayerMask slideLM;

    public float jumpHeight;

    public float fallSpeedMax;
    public float gravityAcceleration;

    private bool grounded;

    private BoxCollider2D bc;
    private float moveInput;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        walkSpeedCurr = walkSpeedDef;
        walkAccelerationCurr = walkAccelerationDef;
    }

    // Update is called once per frame
    void Update()
    {
        CheckSurroundings();
        CheckInput();
       
        Gravity();
        Move();
        ResolveHitboxes();
       
    }

    void CheckInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (grounded)
        {
            velocity.y = 0;
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }

    }
    
    void Gravity()
    {
        if( !grounded)
        {
            velocity.y -= gravityAcceleration * Time.deltaTime;
        }


    }
    void Move()
    {
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, walkSpeedCurr * moveInput, walkAccelerationCurr * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundDeceleration * Time.deltaTime);
        }
        transform.Translate(velocity * Time.deltaTime);
    }

    void Jump()
    {
        if (grounded)
        {
            velocity.y = Mathf.Sqrt(2 * jumpHeight * gravityAcceleration);
        }

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

    private void CheckSurroundings()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLM) || Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, slideLM);
        isOnWall = Physics2D.OverlapArea(WallCheckA.position, WallCheckB.position, groundLM);

    }
}
