using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PushState;


public enum PushState
{
    Pushing,
    Pulling,
    None
}
public class PushPull : MonoBehaviour
{

    public float pushForce;
    public float pushDecay;

    private Rigidbody2D rb;
    private Transform[] metals;
    private Transform nearestMetalTrans;
    LineRenderer nearLR;
    public LineRenderer currLR;

    public PushState currAction;

    private Transform currMetalTrans;

    public bool forceOn;

    public bool log;

    public float boostForce;


    void Start()
    {
        if (!GlobalEvents.GetCondition("pushll") && !forceOn)
        {
            this.enabled = false;
        }

        rb = GetComponent<Rigidbody2D>();
        metals = TransformsFromGameObjects(GameObject.FindGameObjectsWithTag("Metal"));
        if (metals.Length == 0)
        {
            GetComponent<PushPull>().enabled = false;
        }
        CreateNearLine();
        CreateCurrLine();
        currAction = None;
    }

    private Transform[] TransformsFromGameObjects(GameObject[] lst)
    {
        Transform[] transforms = new Transform[lst.Length];
        for(int i = 0; i < lst.Length; i++)
        {
            transforms[i] = lst[i].transform;
        }
        return transforms;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNearestLine();
        UpdateCurrLine();
    }

    private void FixedUpdate()
    {
        FindNearestMetal();
        UpdateCurrMetal();
        if (currAction == Pushing)
        {
            Pushll(-1);
        } else if (currAction == Pulling)
        {
            Pushll(1);
        }
    }

    private void CreateNearLine()
    {
        //GameObject line = new GameObject();
        //line.transform.position = transform.position;
        //transform.gameObject.AddComponent<LineRenderer>();
        nearLR = GetComponent<LineRenderer>();
        nearLR.startWidth = 0.05f;
        nearLR.endWidth = 0.05f;
        //nearLR.SetPosition(0, transform.position);
        //nearLR.SetPosition(1, nearestMetalPos);
    }

    private void CreateCurrLine()
    {
        //GameObject line = new GameObject();
        //line.transform.position = transform.position;
        //line.AddComponent<LineRenderer>();

        currLR = currLR.GetComponent<LineRenderer>();
        currLR.startWidth = 0.1f;
        currLR.endWidth = 0.1f;
        currLR.startColor = Color.blue;
        currLR.endColor = Color.blue;

    }

    private void UpdateNearestLine()
    {
        nearLR.SetPosition(1, nearestMetalTrans.position);
        nearLR.SetPosition(0, transform.position);
    }

    private void UpdateCurrLine()
    {
        if (currMetalTrans == null)
        {
            currLR.enabled = false;
        }
        else
        {
            currLR.enabled = true;
            currLR.SetPosition(1, currMetalTrans.position);
            currLR.SetPosition(0, transform.position);
        }

    }



    /// <summary>
    /// Applies the push or pull force. Pulling is 1, Pushing is -1.
    /// </summary>
    /// <param name="dir"></param>
    private void Pushll(int dir)
    {
        Transform metal = currMetalTrans != null ? currMetalTrans : nearestMetalTrans;
        currMetalTrans = metal;
        Vector3 direction = metal.position - transform.position;
        direction.Normalize();
        float distance = Vector3.Distance(metal.position, transform.position);
        float force;
        if (distance < 10)
        {
            force = pushForce * dir * 1 / pushDecay;
        }
        else
        {
            force = pushForce * dir / (distance * pushDecay);
        }
        rb.AddForce(direction * force);
    }

    private void UpdateCurrMetal()
    {
        if (currAction == None)
        {
            if (currMetalTrans != null)
            {
                Boost();
            }
            currMetalTrans = null;

        }
    }

    private void Boost()
    {
        Debug.Log("boost");
        Vector2 vel = rb.velocity;
        rb.AddForce(vel.normalized * boostForce, ForceMode2D.Impulse);

    }

    private void FindNearestMetal()
    {
        Transform min = metals[0];
        float minDistance = Vector3.Distance(min.position, transform.position);
        for (int i = 1; i < metals.Length; i++)
        {
            if (log)
            {
                print(i + " " + Vector3.Distance(metals[i].position, transform.position));
            }
            if (Vector3.Distance(metals[i].position, transform.position) < minDistance)
            {
                min = metals[i];
                minDistance = Vector3.Distance(metals[i].position, transform.position);
            }
        }
        nearestMetalTrans = min;
        //if (log)
        //{
        //    print(Vector3.Distance(nearestMetalTrans.position, transform.position));
        //}
        //if (log)
        //{
        //    foreach (Transform metal in metals)
        //    {
        //        print(metal.position);
        //    }
        //}
       
    }

    public void Die()
    {
        ZeroAction();
    }

    public void ZeroAction()
    {
        currAction = None;
    }
}
