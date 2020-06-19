using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityUse
{

    private static HashSet<string> abilitySet = new HashSet<string>();

    public static void UnlockAbility(string ability)
    {
        abilitySet.Add(ability);
    }


    /// <summary>
    /// Returns whether the ability is owned.
    /// </summary>
    /// <param name="ability"></param>
    /// <returns></returns>
    public static bool HasAbility(string ability)
    {
        return abilitySet.Contains(ability);
    }
}
