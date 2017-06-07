using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(AudioSource))]
public class CallCenter : MonoBehaviour
{
    public List<CallCenterNode> Nodes;
    public string StartingID = "1";
    public float TimeBeforeRepeatingMenu;
    public ObjectController OC;   

    private AudioSource sound;

    private string currentNode = "0";

    private ButtonNumber ButtonInput = ButtonNumber.inv;

    private Coroutine call;

	// Use this for initialization
	void Start ()
    {
        sound = GetComponent<AudioSource>();
        StartCall();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void StartCall()
    {
        call = StartCoroutine(RunCallCenter());
    }

    public void StopCall()
    {
        StopCoroutine(call);
        OC.Done();
    }

    public void ButtonPress(int number)
    {
        switch (number)
        {
            case 1:
                ButtonInput = ButtonNumber.one;
                break;
            case 2:
                ButtonInput = ButtonNumber.two;
                break;
            case 3:
                ButtonInput = ButtonNumber.three;
                break;
            case 4:
                ButtonInput = ButtonNumber.four;
                break;
            case 5:
                ButtonInput = ButtonNumber.five;
                break;
            case 6:
                ButtonInput = ButtonNumber.six;
                break;

            default:
                break;
        }
    }

    CallCenterNode FindNode(string id)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].ID == id)
            {
                return Nodes[i];
            }
        }

        throw new Exception("Cannot find node: " + id);
    }

    IEnumerator RunCallCenter()
    {
        PlayNode(StartingID);        

        while (true)
        {
            //don't do anything while a clip is playing
            while (sound.isPlaying)
            {
				/*
				//this allows you to interupt the talking with a button press
				//but i wasn't sure if this was gonna mess with anyting going on

				if (ButtonInput != ButtonNumber.inv) {
					sound.Stop ();
					break;
				}
				*/

                yield return null;
            }

            //move on to the linked node if there is one
            if (FindNode(currentNode).NodeToMoveToOnComplete != "")
            {
                yield return new WaitForSeconds(1f);
                PlayNode(FindNode(currentNode).NodeToMoveToOnComplete);
            }
            //or wait for selection
            else
            {
                float tempTimerVar = Time.timeSinceLevelLoad + TimeBeforeRepeatingMenu;
                while (ButtonInput == ButtonNumber.inv)
                {
                    Debug.Log("waitforselection");
                    //if it's been long enough, replay the current menu
                    if (Time.timeSinceLevelLoad >= tempTimerVar)
                    {
                        PlayNode(currentNode);                        
                        break;
                    }                    
                    yield return null;
                }

                if (ButtonInput != ButtonNumber.inv)
                {
                    //if the input corisponds to a choice, play it
                    string temp = TryButton(ButtonInput, currentNode);
                    if (temp != null)
                    {
                        PlayNode(temp);
                        ButtonInput = ButtonNumber.inv;
                    }
                }
            } 
            yield return null;
        }
    }

    string TryButton(ButtonNumber button, string id)
    {
        CallCenterNode temp = FindNode(id);

        for (int i = 0; i < temp.Options.Count; i++)
        {
            if (temp.Options[i].button == button)
            {
                return temp.Options[i].ID;
            }
        }

        return null;
    }

    private void PlayNode(string node)
    {
        sound.clip = FindNode(node).Clip;
        sound.Play();
        currentNode = node;
    }
}

[Serializable]
public class CallCenterNode
{
    public AudioClip Clip;
    public string ID;
    public List<CallCenterButtonNodePair> Options;
    public string NodeToMoveToOnComplete;

    
}

[Serializable]
public class CallCenterButtonNodePair
{
    public ButtonNumber button;
    public string ID;
}

public enum ButtonNumber {one, two, three, four, five, six, inv}
