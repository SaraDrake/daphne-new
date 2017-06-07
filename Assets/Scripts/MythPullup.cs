using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MythPullup : MonoBehaviour
{
    public ObjectController oc;
    public FadeController fc;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ClickX()
    {
        fc.FadeOut(oc.Done);
    }
}
