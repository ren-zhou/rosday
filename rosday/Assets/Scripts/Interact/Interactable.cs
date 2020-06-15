using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    /** Is true when the object is currently being interacted with.
     Locks the Act method. */
    private bool inAction;
    /** Name of the interactable. */
    private string interactableName;
    public virtual void Act()
    {
    }

    public void UnfreezeClicks()
    {
        inAction = false;
    }

    public bool IsActive()
    {
        return inAction;
    }

    public void FreezeClicks()
    {
        inAction = true;
    }

    public void SetName(string name)
    {
        this.interactableName = name;
    }

    public string Name()
    {
        return interactableName;
    }
}
