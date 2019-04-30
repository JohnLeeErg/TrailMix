using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ButtonHellColorChanger : MonoBehaviour {

    Color endColor;
    Color startColor;
    Tilemap myTileMap;

    bool solved = false;
    float colorChangeTime = 0;
    float colorChangeSpeed = .04f;

	// Use this for initialization
	void Start () {

        myTileMap = GetComponent<Tilemap>();
        endColor = myTileMap.color;
        startColor = new Color(.3f, .35f, .4f);
        myTileMap.color = startColor;
	}
	
	// Update is called once per frame
	void Update () {
		if(solved && colorChangeTime < 1)
        {
            colorChangeTime += colorChangeSpeed;
            myTileMap.color = Color.Lerp(startColor, endColor, colorChangeTime);
        }
	}

    public void StartColorLerp()
    {
        solved = true;
    }
}
