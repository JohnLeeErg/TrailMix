using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneSpecialCam : MonoBehaviour
{
    public float xPointToPass;
    float startSize;
    public float xIncrease;
    public Transform camPosition;
    public Camera orthoCam;

    public Transform[] objectsUnderYPoint;
    public float yPointToStayUnder = 10000;

    bool startedMoving = false;

    // Use this for initialization
    void Start()
    {

        startSize = orthoCam.orthographicSize;
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
        }

        if (camPosition.position.x > xPointToPass && camPosition.position.x < (xPointToPass+2))
        {
            startedMoving = true;
        }

        if (updating && startedMoving)
        {
            if (camPosition.position.x > xPointToPass)
            {
                orthoCam.orthographicSize = startSize + ((camPosition.position.x - xPointToPass) * xIncrease) * (Mathf.Clamp01((camPosition.position.x - xPointToPass) * .02f));
            }
        }
    }
}
