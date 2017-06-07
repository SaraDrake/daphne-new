using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using DG.Tweening;


public class DialogueUI : Yarn.Unity.DialogueUIBehaviour
{
    public Mom mom;
	// The object that contains the dialogue and the options.
	// This object will be enabled when conversation starts, and
	// disabled when it ends.
	public GameObject dialogueContainer;

	// The UI element that displays lines
	public Text lineText;

	// A UI element that appears after lines have finished appearing
	public GameObject continuePrompt;

    public float DialogueNextLineDelay;

	// A delegate (ie a function-stored-in-a-variable) that
	// we call to tell the dialogue system about what option
	// the user selected
	private Yarn.OptionChooser SetSelectedOption;

	[Tooltip("How quickly to show the text, in seconds per character")]
	public float textSpeed = 0.03f;

	// The buttons that let the user choose an option
	public List<Button> optionButtons;

	public RectTransform gameControlsContainer;

    StringBuilder stringBuilder;

    public bool LastLineOfNode;
    public bool MoveToNextLine = false;
    public bool firstLineOfNode = true;

    public DaphneDriver DD;
    public GameObject DialogueOptions;

    public int DialogueOptionsStartPos;
    public int DialogueOptionsEndPos;

    public float DialogueTweenInLength;
    public float DialogueTweenOutLength;

    public DialogueOptionsController DOC;

    private bool ReadytoShowOptions
    {
        get
        {
            if (DD.VOPlaying) return false;
            else return true;
        }
    }

    private NodeStartCallback OnNodeStart;

    void Awake()
	{
		// Start by hiding the container, line and option buttons
		if (dialogueContainer != null)
			dialogueContainer.SetActive(false);

        
		lineText.gameObject.SetActive(false);

		foreach (var button in optionButtons)
		{
			button.gameObject.SetActive(false);
		}

		// Hide the continue prompt if it exists
		if (continuePrompt != null)
			continuePrompt.SetActive(false);

        stringBuilder = new StringBuilder(); 
	}

    public void SubscribeToNodeStartCallback(NodeStartCallback func)
    {
        OnNodeStart += func;
    }

    public void UnsubscribeToNodeStartCallback(NodeStartCallback func)
    {
        OnNodeStart -= func;
    }

    // Show a line of dialogue, gradually
    public override IEnumerator RunLine(Yarn.Line line)
	{
        //if there are subtitle timings, wait to be told to play
        if (DD.currentNode.SubtitleTimestamps.Count != 0)
        {            
            yield return new WaitUntil(() => MoveToNextLine);
            MoveToNextLine = false;
        }

        // Show the text
        lineText.gameObject.SetActive(true);

		if (textSpeed > 0.0f)
		{
			// Display the line one character at a time
			var stringBuilder = new StringBuilder();

			foreach (char c in line.text)
			{
				stringBuilder.Append(c);
				lineText.text = stringBuilder.ToString();
				yield return new WaitForSeconds(textSpeed);
			}
		}
		else {
			// Display the line immediately if textSpeed == 0
			lineText.text = line.text;
		}

		// Show the 'press any key' prompt when done, if we have one
		if (continuePrompt != null)
			continuePrompt.SetActive(true);

        //if this node has no subtitle timing data then automatically move forward
        if (DD.currentNode.SubtitleTimestamps.Count == 0)
        {
            yield return new WaitForSeconds(1f);
        }
        
	}

    

	// Show a list of options, and wait for the player to make a selection.
	public override IEnumerator RunOptions(Yarn.Options optionsCollection,
											Yarn.OptionChooser optionChooser)
	{
        while (!ReadytoShowOptions)
        {
            //check again next frame
            yield return null;
        }

		Debug.Log ("This should not be using GameObject.Find - ie");
		GameObject.Find ("SettingsPlus").GetComponent<UnityEngine.UI.Button> ().interactable = true;


        lineText.text = "";

        DOC.SpreadToAllButtons(optionsCollection.options.Count);        

		// Do a little bit of safety checking
		if (optionsCollection.options.Count > optionButtons.Count)
		{
			Debug.LogWarning("There are more options to present than there are" +
							 "buttons to present them in. This will cause problems.");
		}

		// Display each option in a button, and make it visible
		int i = 0;
		foreach (var optionString in optionsCollection.options)
		{
			optionButtons[i].gameObject.SetActive(true);
			optionButtons[i].GetComponentInChildren<Text>().text = optionString;
			i++;
		}

        // Record that we're using it

        if (!optionsCollection.options[0].ToLower().Contains("ask me"))
        {
            SetSelectedOption = DOC.MoveToQuestionSelected;
            SetSelectedOption += optionChooser;
        }
        else
        {
            SetSelectedOption = optionChooser;
        }      

		// Wait until the chooser has been used and then removed (see SetOption below)
		while (SetSelectedOption != null)
		{
			yield return null;
		}

        if (!optionsCollection.options[0].ToLower().Contains("ask me"))
        {
            // Hide all the buttons
            foreach (var button in optionButtons)
            {
                button.GetComponent<DialogueButtonController>().Disable(true);
				GameObject.Find ("SettingsPlus").GetComponent<UnityEngine.UI.Button> ().interactable = false;
            }
        }

        yield return new WaitWhile(DOC.Busy);
        
	}

	// Called by buttons to make a selection.
	public void SetOption(int selectedOption)
	{

		// Call the delegate to tell the dialogue system that we've
		// selected an option.
		SetSelectedOption(selectedOption);

		// Now remove the delegate so that the loop in RunOptions will exit
		SetSelectedOption = null;
	}

	// Run an internal command.
	public override IEnumerator RunCommand(Yarn.Command command)
	{
		// "Perform" the command
		Debug.Log(Yarn.Unity.DialogueRunner.yarnDebugPrefix + "Command: " + command.text);
        mom.RunCommand(command.text);
		yield break;
	}

	public override IEnumerator DialogueStarted()
	{
		// Enable the dialogue controls.
		if (dialogueContainer != null)
			dialogueContainer.SetActive(true);

		// Hide the game controls.
		if (gameControlsContainer != null)
		{
			gameControlsContainer.gameObject.SetActive(false);
		}

		yield break;
	}

    public override void Stop()
    {
        lineText.text = "";
    }

    // Yay we're done. Called when the dialogue system has finished running.
    public override IEnumerator DialogueComplete()
	{
		Debug.Log(Yarn.Unity.DialogueRunner.yarnDebugPrefix + "Complete!");

		// Hide the dialogue interface.
		if (dialogueContainer != null)
			dialogueContainer.SetActive(false);

		// Show the game controls.
		if (gameControlsContainer != null)
		{
			gameControlsContainer.gameObject.SetActive(true);
		}

		yield break;
	}

    public override IEnumerator NodeStart(string node)
    {
        if (OnNodeStart != null)
        {
            OnNodeStart.Invoke(node);
        }
        yield break;
    }

    public override IEnumerator NodeComplete(string node)
    {
        stringBuilder = new StringBuilder();
        LastLineOfNode = true;
        yield break;
    }

}

public delegate void NodeStartCallback(string ID);

