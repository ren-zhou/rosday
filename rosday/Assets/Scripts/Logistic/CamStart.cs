using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CamStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 entry = GlobalEvents.GetEntry(SceneManager.GetActiveScene().name);
        if (!entry.Equals(new Vector3(0, 0, 0)))
        {
            transform.position = entry;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
