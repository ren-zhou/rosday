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
        if (stallTimer < 0)
        {
            stallTimer = 0;
        }
        if (stallTimer > 0)
        {
            StallCont();
        }
        if((rc.AnyRefreshConditions() || canStall) && tillNextStall == 0 && rc.HasAbility("stall"))
        {
            stallsLeft = totalStalls;
        }
    }

    public void StartStall()
    {
        if (stallsLeft > 0)
        {
            stallTimer = stallTime;
            canStall = false;
            rc.LockInputs();
        }
    }

    private void StallCont()
    {
        if (stallTimer > 0)
        {
            rb.velocity = new Vector2(0, 9.81f * Time.fixedDeltaTime * 3);
            stallTimer -= 1;
        }
        if (stallTimer == 1)
        {
            tillNextStall = 1;
            stallsLeft--;
            rc.ReleaseInputs();
            Debug.Log("hi?");
        }
    }

}
