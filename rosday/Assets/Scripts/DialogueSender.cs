using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attached to NPCs. Set up to have conditions and return a dialogue set based on the conditions.
/// </summary>
/// 
public class DialogueSender : MonoBehaviour
{
    [SerializeField]
    private DialogueSet[] sets;

    [SerializeField]
    private string[] conditions;

    [SerializeField]
    private int[] setNums;

    GlobalEvents events;

    [SerializeField] private GameObject bubble;

    TextboxPresenter presenter;


    public void Set(string text)
    {
        CreateSets(text);

    }

    void Start()
    {
        presenter = FindObjectOfType<TextboxPresenter>();
        events = FindObjectOfType<GlobalEvents>();
        bubble.SetActive(false);
    }
    private void CreateSets(string text)
    {
        string[] splits = { "<><><>" };
        string[] temp = text.Split(splits, System.StringSplitOptions.RemoveEmptyEntries);
        sets = new DialogueSet[temp.Length];
        for (int i = 0; i < sets.Length; i++)
        {
            sets[i] = new DialogueSet(temp[i]);
        }
    }

    public string GetLine()
    {
        if (setNums.Length == 0)
        {
            return sets[0].nextLine();
        }

        int index = events.GetFirstTrueIndex(conditions);
        if (index < 0)
        {
            return sets[0].nextLine();
        }
        else if (index >= setNums.Length)
        {
            index = setNums.Length - 1;
        }
        index = setNums[index];
        if (index >= sets.Length)
        {
            index = sets.Length - 1;
        }
        Debug.Log(index);
        return sets[index].nextLine();
    }

    public void SendDialogue()
    {
        presenter.SetUp(Inter(), GetLine());
    }

    private Interactable Inter()
    {
        return GetComponent<Interactable>();
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        bubble.SetActive(true);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        bubble.SetActive(false);
    }
}
