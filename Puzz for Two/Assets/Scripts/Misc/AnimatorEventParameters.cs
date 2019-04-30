using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventParameters : MonoBehaviour {

    Animator animatorComponent;

	// Use this for initialization
	void Start () {
        animatorComponent = GetComponent<Animator>();
    }

    public void CallSetBoolFalse(string parameterName)
    {
        animatorComponent.SetBool(parameterName, false);
    }

    public void DisableAnimator()
    {
        animatorComponent.enabled = false;
    }
}
