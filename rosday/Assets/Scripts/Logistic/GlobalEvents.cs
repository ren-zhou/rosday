using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Controls various states of the game, including NPC dialogue that is dependent on player actions/player state,
 map conditions, etc. Manages "Conditions," which are each uniquely identified by a string. */
public static class GlobalEvents
{
    /** Dictionary containing conditions and whether they are true or false. */
    private static Dictionary<string, bool> conditions = new Dictionary<string, bool>();
    private static Dictionary<string, Vector3> spawnLocations = new Dictionary<string, Vector3>();
    private static Dictionary<string, bool> spawnDirections = new Dictionary<string, bool>();
    private static Dictionary<string, bool> itemExists = new Dictionary<string, bool>();

    /** Set the conditionName to be tf. */
    public static void UpdateCondition(string conditionName, bool tf)
    {
        if (conditions.ContainsKey(conditionName))
        {
            conditions[conditionName] = tf;
        } else
        {
            conditions.Add(conditionName, tf);
        }
    }

    /** Returns whether or not conditionName is currently true. */
    public static bool GetCondition(string conditionName)
    {
        bool def;
        conditions.TryGetValue(conditionName, out def);
        return def;
    }

    /// <summary>
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns>A list of bools being true or false corresponding to the conditions. </returns>
    public static bool[] GetConditions(List<string> conditions)
    {
        bool[] results = new bool[conditions.Count];
        for (int i = 0; i < conditions.Count; i++)
        {
            results[i] = GetCondition(conditions[i]);
        }
        return results;
    }

    /** Returns the index of the first true condition. If none of the conditions are
     true, then it returns -1. */
    public static int GetFirstTrueIndex(string[] conditions)
    {
        for (int i = 0; i < conditions.Length; i++)
        {
            if (GetCondition(conditions[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public static void SetEntry(string sceneName, Vector3 spawnPoint, bool facingRight)
    {
        if (spawnLocations.ContainsKey(sceneName))
        {
            spawnLocations[sceneName] = spawnPoint;
            spawnDirections[sceneName] = facingRight;
        }
        else
        {
            spawnLocations.Add(sceneName, spawnPoint);
            spawnDirections.Add(sceneName, facingRight);
        }
    }

    public static Vector3 GetEntry(string sceneName)
    {
        Vector3 ret;
        spawnLocations.TryGetValue(sceneName, out ret);
        return ret;
    }

    public static bool GetFacingDir(string sceneName)
    {
        bool ret;
        spawnDirections.TryGetValue(sceneName, out ret);
        return ret;
    }

    public static void UpdateItemExistence(string item, bool exists)
    {
        if (itemExists.ContainsKey(item))
        {
            itemExists[item] = exists;
        }
        else
        {
            itemExists.Add(item, exists);
        }
    }

    public static bool GetItemExistence(string item)
    {
        if (itemExists.ContainsKey(item))
        {
            return itemExists[item];
        }
        return true;
    }

   
}
