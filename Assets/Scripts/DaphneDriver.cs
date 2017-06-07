using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DaphneDriver : MonoBehaviour
{
    public GameObject DaphneHolder;
    public float BreatheStartDelay;
    public float BreatheFreq;
    private bool firstBreathe = true;
    private Coroutine breathe;
    
    public Animator animator;
    public AudioSource audioSource;
    public AudioReactor reactor;
    public DialogueUI DUI;
    public NodeDatabase NodeDB;
    [HideInInspector]
    public NodeData currentNode;

    private int currentEmoteIndex = 0;
    private int currentSubtitleIndex = 0;

	bool shouldBreathe = true;

    public bool VOPlaying
    {
        get
        {
            if (audioSource.isPlaying) return true;
            else return false;
        }
    }

    private bool pause;
    
	// Use this for initialization
	void Start ()
    {
        GameObject.Find("DialogueOverlord").GetComponent<DialogueUI>().SubscribeToNodeStartCallback(OnNodeStart);

        PrepairDatabase();

        //breathe = StartCoroutine(Breathe());
    }

    private void PrepairDatabase()
    {
        for (int i = 0; i < NodeDB.Nodes.Count; i++)
        {
            NodeDB.Nodes[i].Emotes.Sort();
        }
    }
    
    void Update ()
    {
        if (!pause)
        {
            EmoteLogic();
            SubtitleLogic();
			animator.SetBool ("shouldBreathe", shouldBreathe);
        }              
    }

    public bool IsBusy()
    {
        return VOPlaying;
    }

    public void Pause ()
    {
        pause = true;
		shouldBreathe = false;
        //StopCoroutine(breathe);
    }

    public void UnPause()
    {
        pause = false;
		shouldBreathe = true;
        //StartCoroutine(Breathe());
    }

    IEnumerator DaphneToCorner()
    {
        animator.Play("shrink");
        return null;
    }

    IEnumerator DaphneOutOfCorner()
    {
        return null;
    }

    IEnumerator Breathe()
    {
        while (true)
        {
			yield return null;
            //if we're not playing a voice line
			if (!audioSource.isPlaying && animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            {
                if (firstBreathe)
                {
                    firstBreathe = false;
                    yield return new WaitForSeconds(1f);
                }
                
                animator.Play("breathing");

                yield return new WaitForSeconds(1.11f);
            }
            yield return null;
        }
    }

    private void SubtitleLogic()
    {
        //if the current node has subitle data
        if (currentNode.SubtitleTimestamps.Count != 0)
        {
            //if we're playing a voice line
            if (audioSource.isPlaying)
            {
                //and the current index is in the bounds of the list
                if (currentSubtitleIndex < currentNode.SubtitleTimestamps.Count)
                {
                    //and if the timestamp of the current subtitle in samples is greater than the current time in samples of the VO
                    if (((currentNode.SubtitleTimestamps[currentSubtitleIndex]) * 48000) <= audioSource.timeSamples)
                    {
                        //Debug.Log((currentNode.SubtitleTimestamps[currentSubtitleIndex] * 48000) + " " + audioSource.timeSamples);
                        //play the emote
                        Debug.Log("HIT");
                        PlayNextLine();
                        currentSubtitleIndex++;
                    }
                }
            }
        }
    }

    private void EmoteLogic()
    {
		//if the current node has emotes
        if (currentNode.Emotes.Count != 0)
        {
            //if we're playing a voice line and not playing a emote animation
            if (audioSource.isPlaying && animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            {
                //and the current index is in the bounds of the list
                if (currentEmoteIndex < currentNode.Emotes.Count)
                {
                    //and if the timestamp of the current emote in samples is greater than the current time in samples of the VO
                    if ((currentNode.Emotes[currentEmoteIndex].TimeStamp * 48000) <= audioSource.timeSamples)
                    {
                        //play the emote
                        Debug.Log("emote");
                        StartCoroutine(PlayEmote(currentNode.Emotes[currentEmoteIndex].Animation));
                        currentEmoteIndex++;
                    }
                }
            }
        }
    }

    IEnumerator PlayEmote(string id)
    {
		shouldBreathe = false;

		reactor.SetReact(false);
        if (animator.HasState(0, Animator.StringToHash(id)))
        {
            animator.Play(id);
        }
        else
        {
            reactor.SetReact(true);
            throw new Exception("Animation \"" + id + "\" doesn't exist");
        }

        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            yield return null;
        }

        if (VOPlaying)
        {
            reactor.SetReact(true);
        }

		shouldBreathe = true;
    }

    void OnNodeStart(string id)
    {
        DUI.firstLineOfNode = true;
        currentNode = NodeDB.FindNode(id);
        currentEmoteIndex = 0;
        currentSubtitleIndex = 0;

        //if it's not an askme node start the sequence
        if (!id.ToLower().StartsWith("ask"))
        {
            StartCoroutine(HandleVOSequence(id));
		}
    }

    IEnumerator HandleVOSequence(string ID)
    {
        //wait for breathing or whatever else to finish
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            yield return null;
        }

		shouldBreathe = false;

        //play intro
        reactor.SetReact(false);
        animator.Play("intro");
        yield return null;

        //wait until the intro has played until you actually play the intended line
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            yield return null;
        }
        
        PlayDialogueVO(ID);       
        reactor.SetReact(true);

        while (VOPlaying)
        {
            yield return null;
        }

        reactor.SetReact(false);
		Debug.Log ("Outro is supposed to play");
        animator.Play("outro");

		shouldBreathe = true;
    }

    void PlayDialogueVO(string ID)
    {
        AudioClip clip = (AudioClip)Resources.Load("VO\\" + ID);

        if (clip == null) throw new Exception("VO File \"" + ID + "\" doesn't exist");

        audioSource.clip = clip;
        audioSource.Play();
        firstBreathe = true;
    }
        
    public void PlayNextLine()
    {
        DUI.MoveToNextLine = true;
    }

    public string RemoveWhitespace(string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }

    //this function is intended to help create a new database when a new chapter is added. 
    //run it once then save the object it generates as a prefab to create a new database from all loaded nodes.
    private void PopulateNewDatabase()
    {
        Yarn.Unity.DialogueRunner runner = GameObject.Find("DialogueOverlord").GetComponent<Yarn.Unity.DialogueRunner>();
        GameObject tempGO;
        NodeDatabase tempDB;
        tempGO = new GameObject();
        tempGO.name = "New NodeDatabase";
        tempDB = tempGO.AddComponent<NodeDatabase>();
        foreach (string name in runner.dialogue.allNodes)
        {
            Debug.Log(name);
            tempDB.Nodes.Add(new NodeData(name));
        }
    }

    private void OnDestroy()
    {
        if (GameObject.Find("DialogueOverlord"))
        {
            GameObject.Find("DialogueOverlord").GetComponent<DialogueUI>().UnsubscribeToNodeStartCallback(OnNodeStart);
        }
    }
}

