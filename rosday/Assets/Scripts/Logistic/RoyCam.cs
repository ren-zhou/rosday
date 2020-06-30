using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyCam : MonoBehaviour
{
    public Transform target;
    public float speed;
    private float leeway = 0.1f;
    public float xBound;
    public float yBound;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!CloseEnough(transform.position, target.position))
        {
            Adjust();
        }
    }

    //private void Adjust()
    //{
    //    float x = target.position.x;
    //    float y = target.position.y;
    //    float angle = Vector3.Angle(target.position, transform.position) * Mathf.PI / 180;
    //    float distanceAway = Vector3.Distance(transform.position, target.position);

    //    if (distanceAway < xSafe)
    //    {
    //        distance
    //    }

    //    float dirX = Mathf.Sign(transform.position.x - x);
    //    float dirY = Mathf.Sign(transform.position.y - y);
    //    float moveX = -speed * distanceAway / xSafe * Mathf.Cos(angle) * dirX;


    //    float moveY = -speed * distanceAway / ySafe * Mathf.Sin(angle) * dirY;

    //    transform.position = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z);
    //    //if (Mathf.Sign(transform.position.x - x) != dirX)
    //    //{
    //    //    moveX = transform.position.x - x;
    //    //}
    //    //if (Mathf.Sign(transform.position.y - y) != dirY)
    //    //{
    //    //    moveY = transform.position.y - y;
    //    //}
    //    //transform.position = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z);
    //}

    private void Adjust()
    {
        float currx = transform.position.x;
        float curry = transform.position.y;
        float tarx = target.position.x;
        float tary = target.position.y;
        float xdiff = currx - tarx;
        float ydiff = curry - tary;

        float moveX = -xdiff / xBound * speed;
        float moveY = -ydiff / yBound * speed;

        transform.position = new Vector3(transform.position.x + moveX, transform.position.y + moveY, transform.position.z);

    }

    private bool CloseEnough(Vector3 a, Vector3 b)
    {
        if (a.x >= b.x - leeway && a.x <= b.x + leeway 
            && a.y >= b.y - leeway && a.y <= b.y + leeway)
        {
            return true;
        }
            return false;
    }
}
