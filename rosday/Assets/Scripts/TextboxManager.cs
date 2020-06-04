using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour
{
    public GameObject textbox;

    public Text text;

    public TextAsset textfile;
    private string[] lines;

    public int currentLine;
    public int endAtLine;
    private bool linesUpdated;

    public RoyController player;
    private void Start()
    {
        textbox.SetActive(false);
        player = FindObjectOfType<RoyController>();
    }

    private void Update()
    {
        if (!linesUpdated)
        {
            lines = textfile.text.Split('\n');
            linesUpdated = true;
            endAtLine = lines.Length;
        }

        if (currentLine < endAtLine && textbox.activeInHierarchy)
        {
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

    public void ResetText()
    {
        linesUpdated = false;
        currentLine = 0;
    }

    public void SetText(TextAsset file)
    {
        ResetText();
        textfile = file;
        lines = textfile.text.Split('\n');
        endAtLine = lines.Length;
        linesUpdated = true;


    }
    
}
