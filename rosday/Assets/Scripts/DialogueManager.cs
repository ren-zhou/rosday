using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class DialogueManager : MonoBehaviour
{
    /** Panel that has image of dialogue box. */
    public GameObject textbox;

    /** The filer text that gives the locations, font, size, etc, of the text. */
    public Text textHolder;

    private StringBuilder dialogue;
    private StringReader lineSourceSR;

    public GameObject namebox;
    public Text nameHolder;

    private Queue lineQ;
    private bool canClick;

    /** Characters per physics frame. */
    [SerializeField] private int cppf = 1;

    Interactable currInter;

    private RoyController player;

    private bool dialogueFinished;

    private bool interactPress;

    private void Start()
    {
        textbox.SetActive(false);
        player = FindObjectOfType<RoyController>();
        dialogue = new StringBuilder();
        lineQ = new Queue();
        namebox.SetActive(false);
    }

    private void Update()
    {
        interactPress = (Input.GetButtonDown("Interact") || interactPress) && textbox.activeInHierarchy && canClick;
    }

    private void FixedUpdate()
    {

        if (canClick && interactPress)
        {
            interactPress = false;
            canClick = false;
            if (dialogueFinished)
            {
                CloseDialogue();
            }
            else
            {
                dialogue.Clear();
                ReadText();
            }
        }
        else if (textbox.activeInHierarchy && !canClick)
        {
            ReadText();
        }
    }

    public void SetDialogueFile(TextAsset textlines)
    {
        dialogueFinished = false;
        string[] lines = textlines.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            lineQ.Enqueue(lines[i]);
        }
        UpdateReader();
        dialogue.Clear();
    }

    public void SetInter(Interactable inter)
    {
        currInter = inter;
        nameHolder.text = inter.Name();
    }

    private void UpdateReader()
    {
        if (lineQ.Count == 0)
        {
            dialogueFinished = true;
            return;
        }
        lineSourceSR = new StringReader((string) lineQ.Dequeue());
    }

    private void ReadText()
    {
        char[] buffer = new char[cppf];
        int numRead = lineSourceSR.Read(buffer, 0, cppf);
        for (int i = 0; i < numRead; i++)
        {
            dialogue.Append(buffer[i]);
        }
        if (lineSourceSR.Peek() < 0 || numRead < cppf)
        {
            UpdateReader();
            canClick = true;
        }
        textHolder.text = dialogue.ToString();
    }

    private void CloseDialogue()
    {
        textbox.SetActive(false);
        namebox.SetActive(false);
        UnfreezePlayer();
        canClick = false;
        currInter.Deactivate();
        currInter = null;

    }

    private void FreezePlayer()
    {
        player.LockInputs();
    }

    private void UnfreezePlayer()
    {
        player.ReleaseInputs();
    }

    public void Activate()
    {
        textbox.SetActive(true);
        namebox.SetActive(true);
        FreezePlayer();
    }
}
