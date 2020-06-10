using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Random;

public class DialogueSet
{
    private Queue<string> orderedDialogue;

    private string[] randomDialogue;

    private int seed;

    private System.Random random;

    private int lastRandom;

    /// <summary>
    /// Gives the next line of dialogue from this set.
    /// </summary>
    /// <returns></returns>
    public string nextLine()
    {
        string next;
        if (orderedDialogue.Count > 0)
        {
            next = orderedDialogue.Dequeue();
        } else
        {
            int index;
            do
            {
                index = random.Next(0, randomDialogue.Length);
            }
            while (lastRandom == index && randomDialogue.Length > 1);
            lastRandom = index;
            next = randomDialogue[index];
        }
        next = next.Trim('\r', '\n');
        return next;
    }


    public DialogueSet(string text)
    {
        Random.InitState(seed);
        random = new System.Random(seed);
        orderedDialogue = new Queue<string>();
        lastRandom = -1;

        string[] sep = { ">>>>>" };
        string[] sets = text.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);
        if (sets.Length == 1)
        {
            randomDialogue = (sets[0].Split('@'));
        } else
        {
            randomDialogue = (sets[1].Split('@'));
            string[] ordered = sets[0].Split('@');
            foreach (string str in ordered)
            {
                orderedDialogue.Enqueue(str);
            }
        }
    }
}
