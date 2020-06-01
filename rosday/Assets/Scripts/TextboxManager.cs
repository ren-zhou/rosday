using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour
{
    public GameObject textbox;

    public Text text;

    public TextAsset textfile;
    public string[] lines;

    public int currentLine;
    public int endAtLine;

    public RoyController player;
    private void Start()
    {
        textbox.SetActive(false);
        player = FindObjectOfType<RoyController>();

        if (textfile != null)
        {
            lines = textfile.text.Split('\n');
        }

        if(endAtLine == 0)
        {
            endAtLine = lines.Length;
        }

    }

    private void Update()
    {
        if (currentLine < endAtLine && textbox.activeInHierarchy)
        {
            player.UnlockDash();
            text.text = lines[currentLine];
            if(Input.GetButtonDown("Interact"))
            {
                currentLine += 1;
            }
        } else
        {
            textbox.SetActive(false);
            currentLine = 0;
            Unfreeze();
        }
    }

    public void Activate()
    {
        textbox.SetActive(true);

    }

    public void FreezePlayer()
    {
        player.LockInputs();
    }

    private void Unfreeze()
    {
        player.ReleaseInputs();
    }
}
