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
    TextboxManager box;

    RoyController player;

    private void Start()
    {
        box = FindObjectOfType<TextboxManager>();
        player = FindObjectOfType<RoyController>();
    }
    public override void Act()
    {
        Debug.Log("hi");
        setText();
        box.Activate();
        box.FreezePlayer();
        player.UnlockAbility("dash");
    }

    private void setText()
    {
        box.SetText(dialogueFile);
    }
}
