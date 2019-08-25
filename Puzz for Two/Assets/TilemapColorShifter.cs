using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapColorShifter : MonoBehaviour {

    Tilemap tilemap;

    public List<Color> colors;
    Color baseColor;
    int currentToColor = 0;
    float lerpPosition = 0;
    [Range(0,5f)]
    public float lerpSpeed = 1f;
    Color fromColor;
    Color toColor;
    public AnimationCurve lerpCurve;
    // consider an animation curve?
    bool animating;
    bool transitioningOut;

    Transform camTransform;
    Vector2 lastCamPosition;

	// Use this for initialization
	void Start () {
        tilemap = GetComponent<Tilemap>();
        baseColor = tilemap.color;
        colors.Add(baseColor);
        fromColor = baseColor;
        toColor = colors[0];
        camTransform = Camera.main.transform;
        lastCamPosition = new Vector2(camTransform.position.x, camTransform.position.y);
    }
	
	// Update is called once per frame
	void Update () {

        // see if the camera is moving
        Vector2 currentCamPosition = new Vector2(camTransform.position.x, camTransform.position.y);
        bool cameraMoving = (currentCamPosition != lastCamPosition);
        lastCamPosition = currentCamPosition;

        if (!animating)
        {
            if (!cameraMoving)
            {
                animating = true;
                lerpPosition = 0;
                fromColor = tilemap.color;
                toColor = colors[currentToColor];
            }
            
        } else
        {
            if (cameraMoving)
            {
                animating = false;
                lerpPosition = 0;
                fromColor = tilemap.color;
                toColor = baseColor;
                transitioningOut = true;
            }
        }

        if (animating || transitioningOut)
        {
            lerpPosition += lerpSpeed*Time.deltaTime;
            float lerpPositionCurved = lerpCurve.Evaluate(lerpPosition);
            tilemap.color = Color.Lerp(fromColor, toColor, lerpPositionCurved);

            if (lerpPosition >= 1)
            {
                lerpPosition = 0;
                if (animating)
                {
                    currentToColor++;
                    if (currentToColor >= colors.Count)
                    {
                        currentToColor = 0;
                    }
                    fromColor = toColor;
                    toColor = colors[currentToColor];
                }
                else if (transitioningOut)
                {
                    currentToColor = 0;
                    fromColor = toColor;
                    toColor = colors[currentToColor];
                    transitioningOut = false;
                }
            }
        }

	}
}
