using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowModulation : MonoBehaviour {

    SpriteRenderer mySR;
    float startingAlpha;
    bool goingDown;

	// Use this for initialization
	void Start () {
        mySR = GetComponent<SpriteRenderer>();
        startingAlpha = mySR.color.a;
	}
	
	// Update is called once per frame
	void Update () {
		if (goingDown)
        {
            mySR.color -= new Color(0, 0, 0, .001f);
            if (mySR.color.a < 0.05f)
            {
                goingDown = false;
            }
        } else
        {
            mySR.color += new Color(0, 0, 0, .001f);
            if (mySR.color.a > (startingAlpha + .03f))
            {
                goingDown = true;
            }
        }
	}
}
