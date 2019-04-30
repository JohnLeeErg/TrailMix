using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInControl : MonoBehaviour {

    InControl.InControlManager manager;

	// Use this for initialization
	void Start () {
        manager = GetComponent<InControl.InControlManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (manager.enabled == false)
        {
            //print("Destroy Extra InControl Manager");
            Destroy(gameObject);
        }
	}
}
