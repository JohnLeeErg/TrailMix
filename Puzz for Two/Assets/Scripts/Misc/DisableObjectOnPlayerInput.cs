using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectOnPlayerInput : MonoBehaviour {

    public GameObject playerParent;
    Movement playerMovementScipt;

    [Header ("For Activating GameObject Instead")]
    public bool acitivateObjectInstead = false;
    public GameObject objectToActivate;

	// Use this for initialization
	void Start () {
        playerMovementScipt = playerParent.GetComponent<Movement>();
	}
	
	// Update is called once per frame
	void Update () {

        // if the player moves or jumps then disable the gameobject
        if (playerMovementScipt.movementIsStopped == false)
        {
            if ((Mathf.Abs(playerMovementScipt.movementInput.x) > 0 || playerMovementScipt.playerInput.jumpAction.IsPressed) && acitivateObjectInstead == false)
            {
                gameObject.SetActive(false);
                this.enabled = false;
            }

            if ((Mathf.Abs(playerMovementScipt.movementInput.x) > 0 || playerMovementScipt.playerInput.jumpAction.IsPressed) && acitivateObjectInstead == true)
            {
                objectToActivate.SetActive(true);
                this.enabled = false;
            }
        }
    }
}
