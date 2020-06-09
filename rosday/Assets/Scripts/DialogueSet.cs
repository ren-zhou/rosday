using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Random;

public class DialogueSet : MonoBehaviour
{
    private Queue<string> orderedDialogue;

    private List<string> randomDialogue;

    private int seed;

    private System.Random random;

    void Start()
    {
        Random.InitState(seed);
        random = new System.Random(seed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string nextLine()
    {
        if (orderedDialogue.Count > 0)
        {
            return orderedDialogue.Dequeue();
        }
        int index = random.Next(0, randomDialogue.Count);
        return randomDialogue[index];
    }
}
