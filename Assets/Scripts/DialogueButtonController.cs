using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DialogueButtonController : MonoBehaviour
{ 
    public CanvasGroup Cgroup;
    public UnityEngine.UI.Image Background;
    public UnityEngine.UI.Text Text;
    public float Alpha
    {
        get
        {
            return Cgroup.alpha;
        }
        set
        {
            Cgroup.alpha = value;
        }
    }

    Color defaultTextColor;

	// Use this for initialization
	void Start ()
    {
        defaultTextColor = Text.color;
	}


    public void BackgroundActive(bool bl)
    {
        Debug.Log("background active " + bl);
        if (bl)
        {
            Background.enabled = true;
        }
        else
        {
            Background.enabled = false;
        }
    }

    public void Disable(bool FadeToWhite)
    {
        if (gameObject.activeSelf)
        { 
            if (FadeToWhite)
            {
                //Text.DOColor(Color.white, .3f).OnComplete(OnFadeComplete);
                OnFadeComplete();
            }
            else
            {
                OnFadeComplete();
            }
        }
    }

    void OnFadeComplete()
    {
        Text.color = defaultTextColor;
        gameObject.SetActive(false);
    }
}
