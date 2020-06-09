using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private GameObject currInter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            Debug.Log(other.name);
            currInter = other.gameObject;
            currInter.SendMessage("OnEnter");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable") && other.gameObject == currInter)
        {
            currInter.SendMessage("OnExit");
            currInter = null;
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Interact") && currInter)
        {
            currInter.SendMessage("Act");
        }
    }

}
