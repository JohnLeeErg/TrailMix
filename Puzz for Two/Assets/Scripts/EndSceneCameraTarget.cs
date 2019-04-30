using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneCameraTarget : MonoBehaviour {

    public Transform p1, p2;

    public float xPointToPass = 0;
    float yIncrease;

    public AnimationCurve easein;

    public float easeInFullLengthMult = .01f;
    public float easeInSpeed = 30;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(p1.position, p2.position, .5f);
        if (transform.position.x > xPointToPass)
        {
            transform.position += new Vector3(0, easein.Evaluate((transform.position.x - xPointToPass) * easeInFullLengthMult)*easeInSpeed, 0);
        }
	}
}
