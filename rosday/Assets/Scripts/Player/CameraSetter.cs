using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    Transform camLooksAt;
    void Start()
    {
        camLooksAt = GameObject.FindGameObjectWithTag("CamLooksAt").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Roy"))
        {
            camLooksAt.position = transform.position;
            //collision.transform.Find("CamLooksAt").transform.position = transform.position;
            
        }
    }
}
