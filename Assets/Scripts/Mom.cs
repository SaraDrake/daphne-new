using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Mom : MonoBehaviour
{
    public MoviePlayer MoviePlayer;
    public Yarn.Unity.DialogueRunner Runner;

    public ObjectController Daphne;
    public ObjectController DialogueCanvas;
    public DaphneDriver DD;
    public ObjectController Questionaire;
    public ObjectController Permission1;
    public ObjectController Permission2;
    public ObjectController Permission3;
    public ObjectController Permission4;
    public ObjectController Permission5;
    public ObjectController Myth;
    public ObjectController CallCenter;
    public MovieTexture Logo;
    public MovieTexture IntroCommercial;
    public MovieTexture EncryptedMsg;

    public bool SkipIntro;
    public string startNode;

	public string currentYarnNodeName;

    // Use this for initialization
    void Start ()
    {
        if (!SkipIntro)
        {
            StartCoroutine(StartSequence());
        }
        else
        {
            Daphne.TurnON();                   		
            DialogueCanvas.TurnON();
            if (startNode == "") startNode = "DAPH02_ThankYou";
            Runner.StartDialogue(startNode);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		currentYarnNodeName = Runner.currentNodeName;
    }

    IEnumerator StartSequence()
    {        
        yield return null;
        MoviePlayer.Play(Logo);
        yield return new WaitWhile(MoviePlayer.IsPlaying);
        Permission1.TurnON();
        yield return new WaitUntil(Permission1.IsDone);
        Permission1.TurnOFF();
        MoviePlayer.Play(IntroCommercial);   
        yield return new WaitWhile(MoviePlayer.IsPlaying);
        Daphne.TurnON();
        Runner.StartDialogue();
        DialogueCanvas.TurnON();
        yield return null;
    }

	public  void RejoinYarn(string nodeName){
	
	}

    public void RunCommand(string input)
    {
        switch (input)
        {
            case "questionaire":
                StartCoroutine(QuestionaireSection());
                break;

            case "myth":
                StartCoroutine(MythSection());
                break;

            case "callcenter":
                StartCoroutine(CallCenterSection());
                break;

			case "permission2":
			StartCoroutine(AppPermissionTwo());
			break;

            case "permission3":
                StartCoroutine(AppPermissionThree());
                break;

            case "permission4":
                StartCoroutine(AppPermissionFour());
                break;

            case "permission5":
                StartCoroutine(AppPermissionFive());
                break;
            default:
                Debug.LogError("no command: " + input + " found");
                break;
        }
    }

    public void RunCallCenter()
    {
        StartCoroutine(CallCenterSection());   
    }

	IEnumerator AppPermissionTwo()
	{
		yield return new WaitWhile(DD.IsBusy);
		Daphne.TurnOFF();
		Runner.Stop();
		DialogueCanvas.TurnOFF();
		Permission2.TurnON();
		yield return new WaitUntil(Permission2.IsDone);
		Daphne.TurnON();
		Runner.StartDialogue("postmyth");
		DialogueCanvas.TurnON();
		yield return null;
	}

    IEnumerator AppPermissionThree()
    {
        yield return new WaitWhile(DD.IsBusy);
        Daphne.TurnOFF();
        Runner.Stop();
        DialogueCanvas.TurnOFF();
        Permission3.TurnON();
        yield return new WaitUntil(Permission3.IsDone);
        Daphne.TurnON();
		Runner.StartDialogue("afterPermission03");
        DialogueCanvas.TurnON();
        yield return null;
    }

    IEnumerator AppPermissionFour()
    {
        yield return new WaitWhile(DD.IsBusy);
        Daphne.TurnOFF();
        Runner.Stop();
        DialogueCanvas.TurnOFF();
        Permission4.TurnON();
        yield return new WaitUntil(Permission4.IsDone);
        Daphne.TurnON();
        //this line sets the next dialog or options
		Runner.StartDialogue("afterPermission04");
        DialogueCanvas.TurnON();
        yield return null;
    }

    IEnumerator AppPermissionFive()
    {
        yield return new WaitWhile(DD.IsBusy);
        Daphne.TurnOFF();
        Runner.Stop();
        DialogueCanvas.TurnOFF();
        Permission5.TurnON();
        yield return new WaitUntil(Permission5.IsDone);
        MoviePlayer.Play(Logo);
        yield return new WaitWhile(MoviePlayer.IsPlaying);
        Daphne.TurnON();
        Runner.StartDialogue("DAPH70_HowCanIHelp");
        DialogueCanvas.TurnON();
        yield return null;
    }

    IEnumerator QuestionaireSection()
    {
        yield return new WaitWhile(DD.IsBusy);
        Daphne.TurnOFF();
        Runner.Stop();
        DialogueCanvas.TurnOFF();
        Questionaire.TurnON();
        yield return new WaitUntil(Questionaire.IsDone);
        Daphne.TurnON();
        Runner.StartDialogue("DAPH02_ThankYou");
        DialogueCanvas.TurnON();
        yield return null;
    }

    IEnumerator MythSection()
    {
        yield return new WaitWhile(DD.IsBusy);
		if (Daphne == null) {
			Debug.Log ("AH");
		}
		Daphne.TurnOFF();
        Runner.Stop();
        DialogueCanvas.TurnOFF();
        Myth.TurnON();
        yield return new WaitUntil(Myth.IsDone);
        Permission2.TurnON();
        yield return new WaitUntil(Permission2.IsDone); 
        Daphne.TurnON();
        Runner.StartDialogue("postmyth");
        DialogueCanvas.TurnON();
        yield return null;
    }

    IEnumerator CallCenterSection()
    {
        yield return new WaitWhile(DD.IsBusy);
        Daphne.TurnOFF();
        Runner.Stop();
        DialogueCanvas.TurnOFF();
        CallCenter.TurnON();
        yield return new WaitUntil(CallCenter.IsDone);
        CallCenter.TurnOFF();
        StartCoroutine(AppPermissionFour());
        yield return null;
    }

    void Restart()
    {
        MoviePlayer.Clear();
        IntroCommercial.Stop();
        Logo.Stop();
        Application.LoadLevel(Application.loadedLevel);
    }
}
