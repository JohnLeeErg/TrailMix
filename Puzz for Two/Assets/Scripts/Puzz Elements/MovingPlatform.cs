using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public Transform[] points;
    int currentPoint = 0;
    int nextPoint = -1;
    Transform currentTarget;
    public float moveSpeed = .1f;
    public bool autoMove;
    bool currentlyMoving;
    // Use this for initialization
    void Start()
    {
        if (points.Length > 0)
        {
            if (autoMove)
            {
                StartMoving();
            }
        }
    }
    public void StartMoving()
    {
        StartMoving(null);
    }
    public void StartMoving(Transform target)
    {
        if (currentTarget == null)
        {
            if (target != null)
            {
                currentTarget = target;
                currentlyMoving = true;
            }
            else
            {
                //default to the list I guess
                UpdateAutoMove();

                currentlyMoving = true;
            }
        }
        else
        {
            if (target != null)
            {
                currentTarget = target;
            }
            currentlyMoving = true;

        }
    }
    public void StopMoving()
    {
        currentlyMoving = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentlyMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
            if (transform.position == currentTarget.position)
            {
                currentlyMoving = false;

                UpdateAutoMove();

            }
        }
    }

    public void UpdateAutoMove()
    {

        
        nextPoint++;
        if (nextPoint >= points.Length-1)
        {
            nextPoint = 0;
        }

        if ( points.Length>0 && points[nextPoint] != null)
        {
            currentTarget = points[nextPoint];
        }
        else
        {
            //no point to move to!
        }
        currentPoint = nextPoint;
    }
}
