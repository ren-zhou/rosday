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

    private DialogueSet[] dialogueSets;

    private int setNum;

    [SerializeField] private GameObject bubble;

    RoyController player;

    private void Start()
    {
        box = FindObjectOfType<DialogueManager>();
        player = FindObjectOfType<RoyController>();
        SetName(npc_name);
        bubble.SetActive(false);
        setNum = 0;
        createSets();
    }
    public override void Act()
    {
        if (!isActive())
        {
            SetText();
            box.SetInter(this);
            box.Activate();
            player.UnlockAbility("dash");
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

    private void SetText()
    {
        box.SetDialogueFromString(dialogueSets[setNum].nextLine());
    }

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
