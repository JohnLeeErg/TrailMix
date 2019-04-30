using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopPan : MonoBehaviour {

    public float speed = .01f;
    public GameObject startObject, endObject;
    Vector3 startPosition;
    float startObjectXPosition;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        startObjectXPosition = startObject.transform.position.x;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (speed < 0 && (endObject.transform.position.x + speed) < startObjectXPosition)
        {
            // reset
            transform.position = startPosition;
        }
        else if (speed > 0 && (endObject.transform.position.x + speed) > startObjectXPosition)
        {
            // reset
            transform.position = startPosition;
        }

        transform.position += new Vector3(speed, 0,0);
	}
}
