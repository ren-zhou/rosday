using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Random;

/// <summary>
/// Class for triggering random animations on a sprite.
/// </summary>
public class RandomAnim : MonoBehaviour
{

    private int seed = 0;
    private System.Random rand;
    private Animator anim;
    private void Start()
    {
        rand = new System.Random(seed);
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        bool val = false;
        if (rand.Next(200) == 1)
        {
            val = true;
        }
        anim.SetBool("start", val);
        Debug.Log(val);
    }
}
