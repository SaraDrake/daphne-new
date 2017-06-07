using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public GameObject target;

    public UnityEngine.Events.UnityEvent OnEvent;
    public UnityEngine.Events.UnityEvent OffEvent;

    private bool done = false;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Done()
    {
        done = true;
    }

    public bool IsDone()
    {
        return done;
    }

    public void TurnON()
    {
        target.SetActive(true);
        if (OnEvent != null)
        {
            OnEvent.Invoke();
        }
    }

    public void TurnOFF()
    {
        target.SetActive(false);
        if (OffEvent != null)
        {
            OffEvent.Invoke();
        }
        done = false;
    }
}
