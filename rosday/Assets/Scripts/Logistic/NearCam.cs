using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearCam : MonoBehaviour
{
    public float bias;
    public Transform[] points;
    private bool nonMoving;
    private GameObject roy;
    void Start()
    {
        int children = transform.childCount;
        points = new Transform[children];
        roy = GameObject.Find("roy1");
        for (int i = 0; i < children; ++i)
        {
            points[i] = transform.GetChild(i);
        }


        if (points.Length == 0)
        {
            nonMoving = true;
        }
        GoToNearest();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GoToNearest();
    }

    private void GoToNearest()
    {
        if (nonMoving)
        {
            return;
        }
        Transform min = transform;
        float minDistance = Vector3.Distance(min.position, roy.transform.position);
        for (int i = 0; i < points.Length; i++)
        {
            Debug.Log(Vector3.Distance(points[i].position, roy.transform.position));
            if (Vector3.Distance(points[i].position, roy.transform.position) < minDistance)
            {
                min = points[i];

            }
        }
        transform.position = min.position;
    }
}
