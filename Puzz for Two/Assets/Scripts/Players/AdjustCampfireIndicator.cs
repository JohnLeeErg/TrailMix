using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCampfireIndicator : MonoBehaviour {

    PlayerIndicator playerIndicatorComponent;
    SpriteRenderer spriteRendererComponent;
    Movement playerMovmentComponent;

    public Sprite xButton, xKey, sKey, downArrow;

	// Use this for initialization
	void Start () {
        playerIndicatorComponent = GetComponent<PlayerIndicator>();
        playerMovmentComponent = GetComponentInParent<Movement>();
        spriteRendererComponent = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (playerIndicatorComponent.isActivated && playerIndicatorComponent.changedSprite)
        {
            ChangeImage();
        }
        else
        {
            if (NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
            {
                if (playerMovmentComponent.playerNumber == 1)
                {
                    playerIndicatorComponent.transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
        }
    }

    public void ChangeImage()
    {
        Sprite newSprite = null;

        if (NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.OnePlayerController ||
            NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController)
        {
            playerIndicatorComponent.activeSprite = xButton;
            newSprite = xButton;
        }
        else if (NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard ||
                NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.Undecided)
        {
            if (playerMovmentComponent.playerNumber == 0)
            {
                playerIndicatorComponent.activeSprite = xButton;
                newSprite = xButton;
            }
            else if (playerMovmentComponent.playerNumber == 1)
            {
                playerIndicatorComponent.activeSprite = xKey;
                newSprite = xKey;
            }
        }
        else if (NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
        {
            if (playerMovmentComponent.playerNumber == 0)
            {
                playerIndicatorComponent.activeSprite = sKey;
                newSprite = sKey;
            }
            else if (playerMovmentComponent.playerNumber == 1)
            {
                playerIndicatorComponent.activeSprite = downArrow;
                newSprite = downArrow;
                playerIndicatorComponent.transform.eulerAngles = new Vector3(0, 0, 75);
            }
        }
        // For one player keyboard
        else
        {
            playerIndicatorComponent.activeSprite = xKey;
            newSprite = xKey;
        }

        spriteRendererComponent.sprite = newSprite;
    }
}
