using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class TextboxPresenter : MonoBehaviour
{
    private GameObject panel;

    /** Panel that has image of dialogue box. */
    private GameObject textbox;

    /** The filer text that gives the locations, font, size, etc, of the text. */
    private Text textHolder;

    /** The currently showing dialogue in the panel. */
    private StringBuilder dialogue;
    /** The currently loaded line to feed into the dialogue. */
    private StringReader lineSourceSR;

    /** Panel that holds the name. */
    private GameObject namebox;
    /** The text that is filled a name. */
    private Text nameHolder;

    /** Queue of lines in the loaded dialogue. */
    private Queue lineQ;
    /** True when the current line is finished reading and a click can advance the dialogue. */
    private bool canClick;

    /** Characters per physics frame. */
    [SerializeField] private int cppf = 2;

    /** The interactable that is currently activating the dialogue manager. */
    Interactable currInter;

    /** The player. */
    private RoyController player;

    /** True when the loaded dialogue has been finished. */
    private bool dialogueFinished;

    /** Keeps track of if interact has been pressed. */
    private bool interactPress;

    private void Start()
    {
        panel = GameObject.FindGameObjectsWithTag("DialogueTextbox")[0];
        textbox = panel.transform.GetChild(0).gameObject;
        textHolder = textbox.transform.GetChild(0).gameObject.GetComponent<Text>();
        namebox = panel.transform.GetChild(1).gameObject;
        nameHolder = namebox.transform.GetChild(0).gameObject.GetComponent<Text>();



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

    public void SetUp(Interactable inter, string textlines)
    {
        SetDialogueFromString(textlines);
        SetInter(inter);
        Activate();
    }


    /// <summary>
    /// Deprecated. Used to load dialogue from a text file.
    /// </summary>
    /// <param name="textlines"></param>
    private void SetDialogueFromFile(TextAsset textlines)
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

    /** Loads dialogue from a string fed to it. Sets the lineQ, updates the string reader,
     and clears the current dialogue. */
    private void SetDialogueFromString(string text)
    {
        dialogueFinished = false;
        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            lineQ.Enqueue(lines[i]);
        }
        UpdateReader();
        dialogue.Clear();
    }

    /** Sets the currently loaded interactable. */
    private void SetInter(Interactable inter)
    {
        currInter = inter;
        nameHolder.text = inter.Name();
    }

    /** Updates the string reader with the next line of dialogue. */
    private void UpdateReader()
    {
        if (lineQ.Count == 0)
        {
            dialogueFinished = true;
            return;
        }
        lineSourceSR = new StringReader((string) lineQ.Dequeue());
    }

    /** Takes cppf characters from the stringreader linesource, feeds them to the dialogue string
     * builder, and figures out conditions relating to the clicking. Also updates the text holder
     the new text.*/
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

    /** Closes the current dialogue by closing the textbox, unfreezing the player, and releasing
     the interactable from the inAction state.*/
    private void CloseDialogue()
    {
        dialogue.Clear();
        textbox.SetActive(false);
        namebox.SetActive(false);
        UnfreezePlayer();
        canClick = false;
        currInter.UnfreezeClicks();
        currInter = null;

    }

    /** Freezes the input of the player and zeroes the velocity. */
    private void FreezePlayer()
    {
        player.LockInputs();
        player.ZeroXVelocity();
    }

    /** Unlocks player inputs. */
    private void UnfreezePlayer()
    {
        player.ReleaseInputs();
    }

    /** Activates the dialogue manager's components and freezes the player.*/
    public void Activate()
    {
        textbox.SetActive(true);
        namebox.SetActive(true);
        FreezePlayer();
    }
}
