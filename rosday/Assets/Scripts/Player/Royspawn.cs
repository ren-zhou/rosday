using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Royspawn : MonoBehaviour
{
    Vector3 respawnPoint;
    private Rigidbody2D rb;
    private PushPull pushll;
    private RoyCheck rc;
    private bool respawn;
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rc = GetComponent<RoyCheck>();
        pushll = GetComponent<PushPull>();
        rb.velocity = Vector3.zero;
        Vector3 entry = GlobalEvents.GetEntry(SceneManager.GetActiveScene().name);
        if (entry != Vector3.zero)
        {
            respawnPoint = entry;
            rb.transform.position = entry;
            GameObject.Find("CamPoint").GetComponent<CameraBounder>().Limit();
            if (!GlobalEvents.GetFacingDir(SceneManager.GetActiveScene().name))
            {
                rc.Flip();
            }
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("respawning", respawn);
        respawn = false;
    }

    public void Die()
    {
        rb.transform.position = respawnPoint;
        rb.velocity = Vector3.zero;
        pushll.Die();
        respawn = true;

    }
}
