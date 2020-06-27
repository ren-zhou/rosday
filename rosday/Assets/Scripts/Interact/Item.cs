using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public string line;
    [SerializeField] private GameObject bubble;

    private TextboxPresenter presenter;
    void Start()
    {
        presenter = FindObjectOfType<TextboxPresenter>();
        bubble.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Act()
    {
        if (!IsActive())
        {
            presenter.SetUp(this, line, false);
            FreezeClicks();
            transform.gameObject.SetActive(false);
        }


    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        bubble.SetActive(true);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        bubble.SetActive(false);
    }
}
