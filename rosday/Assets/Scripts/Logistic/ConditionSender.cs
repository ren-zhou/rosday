using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSender : MonoBehaviour
{

    public string condition;

    public bool isTrue;
    public void Act()
    {
        GlobalEvents.UpdateCondition(condition, isTrue);
    }
}
