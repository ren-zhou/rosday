using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NPC : Interactable
{
    [SerializeField]
    private TextAsset dialogueFile;
    [SerializeField]
    private string npc_name;
    DialogueManager box;

    //public Transform talkToSpot;

    /** Array of dialogue sets. */
    private DialogueSet[] dialogueSets;

    public List<string> conditions;

    /** Numbers corresponding the conditions where the number at the index of the condition
     will be switched to when the condition is true. Lower indexes happen before higher if multiple
        conditions are true. */
    public List<int> setNums;

    /** The index of the set that should be used. By default 0. */
    private int setNum;

    [SerializeField] private GameObject bubble;

    RoyController player;

    ConditionController controller;

    private void Start()
    {
        box = FindObjectOfType<DialogueManager>();
        player = FindObjectOfType<RoyController>();
        SetName(npc_name);
        bubble.SetActive(false);
        setNum = 0;
        createSets();
        controller = FindObjectOfType<ConditionController>();
    }

    public override void Act()
    {
        if (!isActive())
        {
            DetermineSetNum();
            SetText();
            box.SetInter(this);
            box.Activate();
            Activate();
        }
    }

    public override void OnEnter()
    {
        bubble.SetActive(true);
    }

    public override void OnExit()
    {
        bubble.SetActive(false);
    }

    /** Sends info to the dialogue manager with the next line of this npc.*/
    private void SetText()
    {
        box.SetDialogueFromString(dialogueSets[setNum].nextLine());
    }

    /** Determines which set of dialogue should currently be active and sets the set number. */
    private void DetermineSetNum()
    {
        int index = controller.GetFirstTrueIndex(conditions);
        if (index >= 0)
        {
            setNum = setNums[index];
        } else
        {
            setNum = 0;
        }
    }

    /** Creates dialogue sets from the text file attached to this NPC by splitting across
     dialogue files using <><><> and then creating a dialogue set from each split string. */
    private void createSets()
    {
        string[] splits = { "<><><>" };
        string[] temp = dialogueFile.text.Split(splits, System.StringSplitOptions.RemoveEmptyEntries);
        dialogueSets = new DialogueSet[temp.Length];
        for (int i = 0; i < dialogueSets.Length; i++)
        {
            dialogueSets[i] = new DialogueSet(temp[i]);
        }
    }

}
