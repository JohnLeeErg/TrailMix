using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeAnimOffset : MonoBehaviour {
    Animator animComp;
	// Use this for initialization
	void Start () {
        animComp = GetComponent<Animator>();
        animComp.Play(0, -1, Random.value);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
