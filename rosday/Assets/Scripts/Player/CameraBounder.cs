using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounder : MonoBehaviour
{
    public Transform upperRight;
    public Transform lowerLeft;

    private float minX;
    private float maxX;

    private float minY;
    private float maxY;

    private GameObject roy;

    public float camX;
    public float camY;
    void Start()
    {
        minX = lowerLeft.position.x + camX;
        maxX = upperRight.position.x - camX;
        minY = lowerLeft.position.y + camY;
        maxY = upperRight.position.y - camY;
        //Debug.Log(maxX + " " +  minX);
        //Debug.Log(maxY + " " + minY);
        roy = GameObject.FindGameObjectWithTag("Roy");
        transform.position = roy.transform.position;
        Limit();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //bool inX = !(transform.position.x <= minX || transform.position.x <= maxX);
        //bool inY = !(transform.position.y <= minY || transform.position.y <= maxY);
        //if (inX && inY)
        //{
        //    transform.position = transform.parent.transform.position;
        //    return;
        //}
        //if (!inX)
        //{
        //    transform.position = new Vector2(Mathf.Clamp(transform.parent.transform.position.x, minX, maxX), transform.position.y);
        //}
        //if (!inY)
        //{
        //    transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.parent.transform.position.y, minY, maxY));
        //}




        //transform.position = new Vector2(Mathf.Clamp(transform.parent.transform.position.x, minX, maxX), transform.position.y);
        //transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.parent.transform.position.y, minY, maxY));

        Limit();

    }

    public void Limit()
    {
        transform.position = new Vector2(Mathf.Clamp(roy.transform.position.x, minX, maxX), transform.position.y);
        transform.position = new Vector2(transform.position.x, Mathf.Clamp(roy.transform.position.y, minY, maxY));
    }

}
