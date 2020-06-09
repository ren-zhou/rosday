using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    private bool active;

    private string name;
    public virtual void Act()
    {
        //TextboxManager box = FindObjectOfType<TextboxManager>();
        //box.Activate();
        //box.FreezePlayer();
    }

    public void Deactivate()
    {
        active = false;
    }

    public bool isActive()
    {
        return active;
    }

    public void Activate()
    {
        active = true;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public string Name()
    {
        return name;
    }

    public virtual void OnEnter()
    {

    }
    
    public virtual void OnExit()
    {

    }
}
