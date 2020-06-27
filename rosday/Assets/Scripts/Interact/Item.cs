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
        if (!GlobalEvents.GetItemExistence(this.name))
        {
            transform.gameObject.SetActive(false);
        }
        presenter = FindObjectOfType<TextboxPresenter>();
        bubble.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Act()
    {
        presenter.SetUp(this, line, false);
        PickUp();
        transform.gameObject.SetActive(false);


    }

    private void PickUp()
    {
        GlobalEvents.UpdateItemExistence(name, false);
        GameObject.Find("roy1").GetComponent<PushPull>().enabled = true;
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
