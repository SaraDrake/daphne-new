using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Question : MonoBehaviour
{
    public Question NextQuestionNormal;
    public List<IDActionPair> SpecialStuff;
    public CanvasGroup cgroup;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ButtonPress(string id)
    {
        if (id == "")
        {
            GoToQuestion(NextQuestionNormal);
        }
        else
        {
            for (int i = 0; i < SpecialStuff.Count; i++)
            {
                if (SpecialStuff[i].ID == id)
                {
                    GoToQuestion(SpecialStuff[i].NextQuestion);
                }
            }
        }        
    }

    void GoToQuestion(Question q)
    {
        q.Activate();
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        DisableInteraction();
        cgroup.DOFade(1f, .5f).SetEase(Ease.OutQuart).OnComplete(EnableInteraction);
    }

    void Deactivate()
    {
        DisableInteraction();
        cgroup.DOFade(0f, .3f).SetEase(Ease.InQuart).OnComplete(DisableSelf);        
    }

    void EnableInteraction()
    {
        cgroup.interactable = true;
    }

    void DisableInteraction()
    {
        cgroup.interactable = false;
    }

    void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    [System.Serializable]
    public class IDActionPair
    {
        public string ID;
        public Question NextQuestion;
    }
}


