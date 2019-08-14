using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MartysTrickyStickyButtons : MonoBehaviour {

    public Transform myParent;
    public bool firstGuy;
    //
    bool currentlyMoving;
    public float vDistance = 18;
    public float moveSpeed;
    Vector3 targetPosition;
    //
    public ButtonSubcollider[] subColliders;
    Button button;
    public Collider2D[] colliders;


    private void Awake()
    {
        button = GetComponent<Button>();
    }
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

        // subcollider stuff:

        if (subColliders[0].isActivated && subColliders[1].isActivated && subColliders[2].isActivated && subColliders[3].isActivated)
        {
            if (subColliders[0].owner == subColliders[1].owner && subColliders[0].owner == subColliders[2].owner && subColliders[0].owner == subColliders[3].owner)
            {
                button.ActivateButton(subColliders[0].owner);
            } else if (button.isActivated)
            {
                button.DeactivateButton();
            }
        }
        else if (button.isActivated)
        {
            //button.DeactivateButton();
        }

    }

    void Update()
    {
        if (!colliders[0].IsTouchingLayers() && !colliders[1].IsTouchingLayers() && !colliders[2].IsTouchingLayers() && !colliders[3].IsTouchingLayers())
        {
            button.DeactivateButtonNoLayers();
        }
    }

}
