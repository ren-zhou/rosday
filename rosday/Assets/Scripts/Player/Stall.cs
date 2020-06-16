using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stall : MonoBehaviour
{

    public int stallTime;
    private int stallTimer;
    private int tillNextStall;
    private bool canStall;
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
            StartStall();
        }
    }

    private void FixedUpdate()
    {
        if (stallTimer > 0)
        {
            StallCont();
        }
        if((rc.AnyRefreshConditions() || canStall) && tillNextStall == 0 && rc.HasAbility("stall"))
        {
            stallsLeft = totalStalls;
        }
        tillNextStall = tillNextStall > 0 ? tillNextStall - 1 : 0;
    }

    public void StartStall()
    {
        if (stallsLeft > 0)
        {
            stallTimer = stallTime;
            canStall = false;
            stallsLeft = stallsLeft > 0 ? stallsLeft - 1 : 0;
            rc.LockInputs();
        }
        StallCont();
    }

    private void StallCont()
    {
        if (Physics2D.OverlapCircle(rb.position, bubbleRadius, spikeLM))
        {
            Bounce();
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
        rb.velocity = bounceVector;

    }

    private void EndStall()
    {
        stallTimer = 0;
        tillNextStall = 1;
        rc.ReleaseInputs();
    }
}
