using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    // Start is called before the first frame update
    RoyController player;
    void Start()
    {
        player = FindObjectOfType<RoyController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("spike");
        if (other.CompareTag("Roy"))
        {
            player.Die();
        }
    }
}
