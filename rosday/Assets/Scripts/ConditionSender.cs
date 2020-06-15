using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSender : MonoBehaviour
{
    private GlobalEvents controller;

    public string condition;

    public bool isTrue;
    void Start()
    {
        controller = FindObjectOfType<GlobalEvents>();
    }

    public void Act()
    {
        controller.UpdateCondition(condition, isTrue);
        Debug.Log("here");
    }
}
