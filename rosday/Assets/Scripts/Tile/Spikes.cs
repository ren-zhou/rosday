using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    // Start is called before the first frame update
    Royspawn player;
    void Start()
    {
        player = FindObjectOfType<Royspawn>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Roy"))
        {
            //x player.Die();
            other.SendMessage("Die");
        }
    }
}
