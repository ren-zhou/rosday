using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Royspawn : MonoBehaviour
{
    Vector3 respawnPoint;
    private Rigidbody2D rb;
    private PushPull pushll;
    void Start()
    {

        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        Vector3 entry = GlobalEvents.GetEntry(SceneManager.GetActiveScene().name);
        if (entry != null)
        {
            respawnPoint = entry;
            rb.transform.position = entry;
            GameObject.Find("CamLooksAt").transform.position = entry;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        rb.transform.position = respawnPoint;
        
    }
}
