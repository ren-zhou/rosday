﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosCam : MonoBehaviour
{
    public Transform target;
    public float speed;

    public float camX;
    public float camY;

    public float xBound;
    public float yBound;

    public Transform upperRight;
    public Transform lowerLeft;

    private float leeway = 0.1f;
    private float minX;
    private float maxX;

    private float minY;
    private float maxY;

    private GameObject roy;

    void Start()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -20);
        BoundSetUp();
    }

    private void BoundSetUp()
    {
        minX = lowerLeft.position.x + camX;
        maxX = upperRight.position.x - camX;
        minY = lowerLeft.position.y + camY;
        maxY = upperRight.position.y - camY;
        roy = GameObject.FindGameObjectWithTag("Roy");
        //Limit();

    }

    // Update is called once per frame
    void Update()
    {
        //if (!CloseEnough(transform.position, target.position))
        //{
            Adjust();
        //}
        //Limit();
    }
    private void Adjust()
    {
        float currx = transform.position.x;
        float curry = transform.position.y;
        float tarx = target.position.x;
        float tary = target.position.y;
        float xdiff = currx - tarx;
        float ydiff = curry - tary;

        //float moveX = -xdiff / xBound * speed;
        //float moveY = -ydiff / yBound * speed;
        float moveX = -xdiff / xBound * speed * Time.deltaTime;
        float moveY = -ydiff / yBound * speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z);

    }

    //private bool CloseEnough(Vector3 a, Vector3 b)
    //{
    //    if (a.x >= b.x - leeway && a.x <= b.x + leeway 
    //        && a.y >= b.y - leeway && a.y <= b.y + leeway)
    //    {
    //        return true;
    //    }
    //        return false;
    //}

    //public void Limit()
    //{
    //    transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, transform.position.z);
    //    transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
    //}
}
