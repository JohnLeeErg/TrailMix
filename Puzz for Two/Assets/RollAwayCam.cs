using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollAwayCam : MonoBehaviour
{
    public float xPointToPass;
    float startSize;
    public float xIncrease;
    public Transform camPosition;
    public Camera orthoCam;

    public Transform[] objectsUnderYPoint;
    private Dictionary<Transform, float> objectsPreviousYTransform = new Dictionary<Transform, float>();
    public float yPointToStayUnder = 10000;

    bool startedMoving = false;
    public float timeInQuickOrthoIncrease;
    bool expandingCam = false;

    // Use this for initialization
    void Start()
    {
        startSize = orthoCam.orthographicSize;
        foreach (Transform guy in objectsUnderYPoint)
        {
            objectsPreviousYTransform.Add(guy, guy.position.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool updating = true;
        foreach (Transform guy in objectsUnderYPoint)
        {
            if (guy.position.y > yPointToStayUnder)
            {
                updating = false;
                startedMoving = false;
            }
            else
            {
                if (objectsPreviousYTransform[guy] > yPointToStayUnder && guy.position.x > 148) //if the last position was higher than the ypoint
                {
                    StartCoroutine(QuicklyAddToOrthoSize(timeInQuickOrthoIncrease));                     //means a player fell while trying to solve the puzzle
                }
            }
        }

        if (camPosition.position.x > xPointToPass)
        {
            startedMoving = true;
        }

        if (updating && startedMoving && !expandingCam)
        {
            if (camPosition.position.x > xPointToPass)
            {
                orthoCam.orthographicSize = startSize + ((camPosition.position.x - xPointToPass) * xIncrease) * (Mathf.Clamp01((camPosition.position.x - xPointToPass) * .02f));
            }
        }

        //done last b/c figure out position
        foreach (Transform guy in objectsUnderYPoint)
        {
            if (objectsPreviousYTransform.ContainsKey(guy))
            {
                objectsPreviousYTransform[guy] = guy.position.y;
            }
        }
    }

    IEnumerator QuicklyAddToOrthoSize(float t)
    {
        expandingCam = true;
        float startOrtho = orthoCam.orthographicSize;
        float intendedOrthoSize = startSize + ((camPosition.position.x - xPointToPass) * xIncrease) * (Mathf.Clamp01((camPosition.position.x - xPointToPass) * .02f));
        float timeInLerp = 0;

        while (orthoCam.orthographicSize != intendedOrthoSize)
        {
            intendedOrthoSize = startSize + ((camPosition.position.x - xPointToPass) * xIncrease) * (Mathf.Clamp01((camPosition.position.x - xPointToPass) * .02f));
            orthoCam.orthographicSize = Mathf.Lerp(startOrtho, intendedOrthoSize, timeInLerp / t);
            timeInLerp += Time.deltaTime;
            yield return null;
        }

        expandingCam = false;
    }
}
