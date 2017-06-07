using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDaphne : MonoBehaviour {

	bool canTouch = true;

	public void TouchDaphne(){
		if (!canTouch) {
			return;
		}

		Debug.Log ("AAAA");

	}
		
}
