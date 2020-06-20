﻿using System.Collections;
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
    LineRenderer currLR;

    public PushState currAction;

    private Transform currMetalTrans;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        metals = TransformsFromGameObjects(GameObject.FindGameObjectsWithTag("Metal"));
        CreateNearLine();
        //CreateCurrLine();
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
        //UpdateCurrLine();
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
        GameObject line = new GameObject();
        line.transform.position = transform.position;
        line.AddComponent<LineRenderer>();
        nearLR = line.GetComponent<LineRenderer>();
        nearLR.startWidth = 0.1f;
        nearLR.endWidth = 0.1f;
        //nearLR.SetPosition(0, transform.position);
        //nearLR.SetPosition(1, nearestMetalPos);
    }

    //private void CreateCurrLine()
    //{
    //    GameObject line = new GameObject();
    //    line.transform.position = transform.position;
    //    line.AddComponent<LineRenderer>();
    //    currLR = line.GetComponent<LineRenderer>();
    //    currLR.startWidth = 0.1f;
    //    currLR.endWidth = 0.1f;
    //}

    private void UpdateNearestLine()
    {
        nearLR.SetPosition(1, nearestMetalTrans.position);
        nearLR.SetPosition(0, transform.position);
    }

    //private void UpdateCurrLine()
    //{
    //    if (currMetalPos == impossiblePos)
    //    {
    //        currLR.enabled = false;
    //    }
    //    else
    //    {
    //        currLR.enabled = true;
    //        currLR.SetPosition(1, currMetalPos);
    //        currLR.SetPosition(0, transform.position);
    //    }

    //}



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
        rb.AddForce(direction * pushForce * dir);
    }

    private void UpdateCurrMetal()
    {
        if (currAction == None)
        {
            currMetalTrans = null;
        }
    }

    private void FindNearestMetal()
    {
        Transform min = metals[0];
        float minDistance = Vector3.Distance(min.position, transform.position);
        for (int i = 1; i < metals.Length; i++)
        {
            if (Vector3.Distance(metals[i].position, transform.position) < minDistance)
            {
                min = metals[i];
            }
        }
        nearestMetalTrans = min;

    }
}
