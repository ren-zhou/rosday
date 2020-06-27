using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{

    public string scene;
    public bool facingRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Roy"))
        {
            GlobalEvents.SetEntry(SceneManager.GetActiveScene().name, transform.position, facingRight);
            SceneManager.LoadScene(scene);
        }
    }
}
