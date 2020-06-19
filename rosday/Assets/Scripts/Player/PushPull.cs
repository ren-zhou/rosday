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
    private Vector3 nearestMetalPos;
    LineRenderer lr;

    public PushState currAction;

    private bool isPulling;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        metals = TransformsFromGameObjects(GameObject.FindGameObjectsWithTag("Metal"));
        CreateLine();
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
        UpdateMetalLine();
    }

    private void FixedUpdate()
    {
        FindNearestMetal();
        if (currAction == Pushing)
        {
            Pushll(-1);
        } else if (currAction == Pulling)
        {
            Pushll(1);
        }
    }

    private void CreateLine()
    {
        GameObject line = new GameObject();
        line.transform.position = transform.position;
        line.AddComponent<LineRenderer>();
        lr = line.GetComponent<LineRenderer>();
        lr.startColor = Color.blue;
        lr.endColor = Color.blue;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, nearestMetalPos);
    }

    private void UpdateMetalLine()
    {
        lr.SetPosition(1, nearestMetalPos);
        lr.SetPosition(0, transform.position);
    }



    /// <summary>
    /// Applies the push or pull force. Pulling is 1, Pushing is -1.
    /// </summary>
    /// <param name="dir"></param>
    private void Pushll(int dir)
    {
        Vector3 direction = nearestMetalPos - transform.position;
        direction.Normalize();
        rb.AddForce(direction * pushForce * dir);
    }

    private void FindNearestMetal()
    {
        Vector3 min = metals[0].position;
        float minDistance = Vector3.Distance(min, transform.position);
        for (int i = 1; i < metals.Length; i++)
        {
            if (Vector3.Distance(metals[i].position, transform.position) < minDistance)
            {
                min = metals[i].position;
            }
        }
        nearestMetalPos = min;

    }
}
