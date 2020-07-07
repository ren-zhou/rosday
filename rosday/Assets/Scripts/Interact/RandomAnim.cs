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

    public int oneInX;
    private void Start()
    {
        rand = new System.Random(seed);
        anim = GetComponent<Animator>();
        oneInX = oneInX == 0 ? 200 : oneInX;
    }
    void Update()
    {
        bool val = false;
        if (rand.Next(oneInX) == 1)
        {
            val = true;
        }
        anim.SetBool("start", val);
    }
}
