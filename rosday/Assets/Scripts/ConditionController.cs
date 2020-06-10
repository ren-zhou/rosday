using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Controls various states of the game, including NPC dialogue that is dependent on player actions/player state,
 map conditions, etc. Manages "Conditions," which are each uniquely identified by a string. */
public class ConditionController : MonoBehaviour
{
    Dictionary<string, bool> conditions;

    void Start()
    {
        conditions = new Dictionary<string, bool>();
    }

    void Update()
    {
        
    }

    public void UpdateCondition(string conditionName, bool tf)
    {
        if (conditions.ContainsKey(conditionName))
        {
            conditions[conditionName] = tf;
        } else
        {
            conditions.Add(conditionName, tf);
        }
    }

    public bool GetCondition(string conditionName)
    {
        bool def;
        conditions.TryGetValue(conditionName, out def);
        return def;
    }

    public bool[] GetConditions(List<string> conditions)
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
    public int GetFirstTrueIndex(List<string> conditions)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (GetCondition(conditions[i]))
            {
                return i;
            }
        }
        return -1;
    }
}
