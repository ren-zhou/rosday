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


    [SerializeField] private GameObject talkToMe;

    RoyController player;

    private void Start()
    {
        box = FindObjectOfType<DialogueManager>();
        player = FindObjectOfType<RoyController>();
        SetName(npc_name);
        talkToMe.SetActive(false);
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
        talkToMe.SetActive(true);
    }

    public override void OnExit()
    {
        talkToMe.SetActive(false);
    }

    private void SetText()
    {
        box.SetDialogueFile(dialogueFile);
    }
}
