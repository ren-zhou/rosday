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

    private DialogueSender sender;

    private void Start()
    {
        SetName(npc_name);
        sender = GetComponent<DialogueSender>();
        sender.Set(dialogueFile.text);
    }

    public override void Act()
    {
        if (!IsActive())
        {
            Talk();
        }
    }

    private void Talk()
    {
        sender.SendDialogue();
        FreezeClicks();
    }
}
