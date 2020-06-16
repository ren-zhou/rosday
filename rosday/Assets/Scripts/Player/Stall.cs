using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stall : MonoBehaviour
{

    public int stallTime;
    private int stallTimer;
    private int tillNextStall;
    private int totalStalls = 3;
    private int stallsLeft;
    private Rigidbody2D rb;
    private RoyController rc;
    public Vector2 bounceVector;
    public LayerMask spikeLM;
    public float bubbleRadius;
    private void Start()
    {
        rb = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        rc = transform.parent.gameObject.GetComponent<RoyController>();

    }
    private void Update()
    {
        if (Input.GetButtonDown("Stall"))
        {
            if (rc.IsWallSliding())
            {
                WallStall();
            }
            else
            {
                StartStall();
            }

        }
    }

    private void FixedUpdate()
    {
        if (stallTimer > 0)
        {
            StallCont();
        }
        if((rc.AnyRefreshConditions()) && rc.HasAbility("stall"))
        {
            stallsLeft = totalStalls;
        }
        tillNextStall = tillNextStall > 0 ? tillNextStall - 1 : 0;
    }

    public void StartStall()
    {
        if (stallsLeft > 0 && tillNextStall == 0)
        {
            rc.EndDash();
            stallTimer = stallTime;
            stallsLeft = stallsLeft > 0 ? stallsLeft - 1 : 0;
            rc.LockInputs();
            StallCont();
        }
    }

    private void WallStall()
    {
        rc.isWallStalling = true;
    }

    private void StallCont()
    {
        if (Physics2D.OverlapCircle(transform.position, bubbleRadius, spikeLM))
        {
            Bounce();
            return;
        }
        if (stallTimer > 0)
        {
            rb.velocity = new Vector2(0, 9.81f * Time.fixedDeltaTime * 3);
            stallTimer -= 1;
        }
        if (stallTimer == 1)
        {
            EndStall();
        }
    }

    private void Bounce()
    {
        EndStall();
        rb.velocity = new Vector2(rb.velocity.x, bounceVector.y);
        rc.RefreshMovement();
        stallsLeft++;

    }

    private void EndStall()
    {
        stallTimer = 0;
        tillNextStall = 1;
        rc.ReleaseInputs();
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, bubbleRadius);
    }
}
