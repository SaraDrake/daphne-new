using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AppPermission : MonoBehaviour
{
    public GameObject Panel;
    public UnityEngine.Events.UnityEvent YesActions;
    public UnityEngine.Events.UnityEvent NoActions;

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Activate()
    {
		Panel.transform.DOScaleX(1f, .7f).SetEase(Ease.OutExpo);
    }

    public void Deactivate()
    {
        Panel.transform.DOScaleX(0f, .7f).SetEase(Ease.InExpo);
    }

    public void AnswerYes()
    {

		Panel.transform.DOMoveY (-100, 0.8f, false).SetEase (Ease.InExpo).OnComplete (InvokeYesActions);

        //Panel.transform.DOScaleX(0f, .7f).SetEase(Ease.InExpo).OnComplete(InvokeYesActions);
    }

    public void AnswerNo()
    {
		Panel.transform.DOMoveY (-100, 0.8f, false).SetEase (Ease.InExpo).OnComplete (InvokeNoActions);
    }

    void InvokeYesActions()
    {
        if (YesActions != null)
        {
            YesActions.Invoke();
        }
    }

    void InvokeNoActions()
    {
        if (NoActions != null)
        {
            NoActions.Invoke();
        }
    }
}
