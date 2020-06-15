using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Extends the NPC class but also unlocks ability ABILITY when acting.*/
public class AbilityGiver : NPC
{
    public string ability;

    public override void Act()
    {
        base.Act();
        RoyController player = FindObjectOfType<RoyController>();
        player.UnlockAbility(ability);
    }
}
