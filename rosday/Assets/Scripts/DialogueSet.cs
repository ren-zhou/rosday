using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Random;

/// <summary>
/// Class that manages dialogue. One dialogue set contains ordered dialogue and random dialogue. First, ordered dialogue
/// is sent through nextLine,  then random dialogue is chosen at random. Dialogue sets must have at least 1 random dialogue
/// but doesn't necessarily have ordered dialogue.
/// </summary>
public class DialogueSet
{
    /** Queue of ordered dialogue which is consumed after they are read. */
    private Queue<string> orderedDialogue;

    /** Array of strings of random dialogue.*/
    private string[] randomDialogue;
    
    /** Seed for the random. */
    private int seed;

    private System.Random random;

    /** The last random dialogue that was chosen. */
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


    /// <summary>
    /// Constructs a dialogue set from a string of next. Breaks them by ">>>>>" for separating ordered and random dialogue,
    /// and @ to separate instances of talking within the dialogue.
    /// </summary>
    /// <param name="text"></param>
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
