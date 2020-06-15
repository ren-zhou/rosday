using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Interactable currInter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currInter = other.GetComponent<Interactable>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable") && other.gameObject == currInter)
        {
            currInter = null;
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Interact") && currInter)
        {
            currInter.Act();
        }
    }

}
