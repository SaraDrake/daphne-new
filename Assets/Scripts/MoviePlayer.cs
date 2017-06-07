using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoviePlayer : MonoBehaviour
{
    private MovieTexture Video;
    private RawImage ri;
    private AudioSource sound;

    public bool IsPlaying()
    {
        return Video.isPlaying;        
    }

	// Use this for initialization
	void Start ()
    {
        ri = GetComponent<RawImage>();
        sound = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Video != null)
        {
            if (!IsPlaying() && ri.enabled)
            {
                ri.enabled = false;
            }
        }
	}

    public void Play(MovieTexture vid)
    {
        Video = vid;
        ri.texture = Video;
        sound.clip = Video.audioClip;
        sound.Play();
        Video.Play();
        ri.enabled = true;
    }

    public void Clear()
    {
        Video = null;
    }

}
