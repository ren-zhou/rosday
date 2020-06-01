using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyNPC : MonoBehaviour
{
    public void Act()
    {
        TextboxManager box = FindObjectOfType<TextboxManager>();
        box.Activate();
        box.FreezePlayer();
    }
}
