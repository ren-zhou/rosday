using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    /** Is true when the object is currently being interacted with.
     Locks the Act method. */
    private bool inAction;
    /** Name of the interactable. */
    private string interactableName;
    public virtual void Act()
    {
    }

    public void Deactivate()
    {
        inAction = false;
    }

    public bool isActive()
    {
        return inAction;
    }

    public void Activate()
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

    /// <summary>
    /// Called when the player enters the collider of the interactable.
    /// </summary>
    public virtual void OnEnter()
    {

    }
    
    /// <summary>
    /// Called when the player exits the collider of the interactable.
    /// </summary>
    public virtual void OnExit()
    {

    }
}
