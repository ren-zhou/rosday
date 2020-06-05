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

    private bool inAction;

    RoyController player;

    private void Start()
    {
        box = FindObjectOfType<TextboxManager>();
        player = FindObjectOfType<RoyController>();
    }
    public override void Act()
    {
        if (!inAction)
        {
            Debug.Log("hi");
            SetText();
            box.Activate();
            box.FreezePlayer();
            player.UnlockAbility("dash");
            inAction = true;
        }

    }

    private void SetText()
    {
        box.SetText(dialogueFile);
    }

    public void Reactivate()
    {
        inAction = false;
    }
}
