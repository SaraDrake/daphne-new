using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QuestionaireEnd : MonoBehaviour
{
    public CanvasGroup cgroup;
    public FadeController fade;

	// Use this for initialization
	void Start ()
    {
       fade.FadeOut(GetComponentInParent<ObjectController>().Done, 1f);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

}
