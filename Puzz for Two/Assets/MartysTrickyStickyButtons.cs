using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartysTrickyStickyButtons : MonoBehaviour {

    public Transform myParent;
    public bool firstGuy;
    //
    bool currentlyMoving;
    float vDistance = 18;
    public float moveSpeed;
    Vector3 targetPosition;

	// Use this for initialization
	void Start () {
        Transform tempParent = transform.parent;
        Vector3 tempPosition = transform.parent.position;
        tempParent.position = myParent.position;
        transform.parent = myParent;
        tempParent.position = tempPosition;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        if (!firstGuy)
        {
            transform.position += new Vector3(0, vDistance, 0);
        }

	}

    // BUTTON MOVEMENT STUFF

    public void StartMoving()
    {
        targetPosition = transform.localPosition + new Vector3(0, -vDistance);
        currentlyMoving = true;

    }
    public void StopMoving()
    {
        currentlyMoving = false;
    }
    void FixedUpdate()
    {
        if (currentlyMoving)
        {
            transform.localPosition -= new Vector3(0, moveSpeed * Time.deltaTime,0);
            if (transform.localPosition.y <= targetPosition.y)
            {
                currentlyMoving = false;
                transform.localPosition = new Vector3 (transform.localPosition.x,targetPosition.y, transform.localPosition.z);
            }
        }
    }

}
