using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeController : MonoBehaviour
{
    public CanvasGroup cgroup;
    public float InDuration;
    public float OutDuration;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void FadeIn()
    {
        cgroup.alpha = 0;
        cgroup.DOFade(1f, InDuration);
    }

    public void FadeOut(TweenCallback action = null, float delay = 0f)
    {
        if (action != null)
        {
            cgroup.DOFade(0f, InDuration).OnComplete(action).SetDelay(delay);
        }
        else
        {
            cgroup.DOFade(0f, InDuration).SetDelay(delay);
        }
        
    }
}
