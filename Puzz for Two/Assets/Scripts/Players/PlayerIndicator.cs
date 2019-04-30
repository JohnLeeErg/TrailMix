using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{

    NewControllerManager controllerManagerInstance;

    Movement playerMovmentScript;

    Sprite originalSprite;
    public Sprite activeSprite;

    SpriteRenderer spriteRendererComp;

    Animator AnimatorComp;

    public bool isActivated = false;
    public bool changedSprite = false;
    [HideInInspector] public bool playerIsHoldingDownButton = false;
    bool indicatorIsStopped = false;

    // Use this for initialization
    void Start()
    {
        controllerManagerInstance = NewControllerManager.instance;

        playerMovmentScript = GetComponentInParent<Movement>();
        AnimatorComp = GetComponent<Animator>();

        spriteRendererComp = GetComponent<SpriteRenderer>();
        originalSprite = spriteRendererComp.sprite;
        spriteRendererComp.enabled = false;
    }

    private void Update()
    {
        if (indicatorIsStopped == false)
        {
            // ON BUTTON DOWN
            if (isActivated && changedSprite && playerMovmentScript.playerInput.confirmAction.IsPressed)
            {
                //spriteRendererComp.enabled = false;
                spriteRendererComp.color = new Color(spriteRendererComp.color.r, spriteRendererComp.color.g, spriteRendererComp.color.b, 0.5f);
                AnimatorComp.SetBool("IsPressed", true);

                playerIsHoldingDownButton = true;
            }

            // ON BUTTON UP
            if (isActivated && changedSprite && playerMovmentScript.playerInput.confirmAction.WasReleased && spriteRendererComp.color.a == 0.5f)
            {
                //spriteRendererComp.enabled = true;
                spriteRendererComp.color = new Color(spriteRendererComp.color.r, spriteRendererComp.color.g, spriteRendererComp.color.b, 1f);
                AnimatorComp.SetTrigger("IsActive");
                AnimatorComp.SetBool("IsPressed", false);

                playerIsHoldingDownButton = false;
            }
        }
    }

    public void StopIndicatorFunctionality()
    {
        indicatorIsStopped = true;
    }

    public void StartIndicatorFunctionality()
    {
        indicatorIsStopped = false;

        // if the player released the button then make the indicator appear again
        if (playerIsHoldingDownButton == true && playerMovmentScript.playerInput.confirmAction.IsPressed == false)
        {
            // Reset the indicator 
            spriteRendererComp.enabled = true;
            spriteRendererComp.color = new Color(spriteRendererComp.color.r, spriteRendererComp.color.g, spriteRendererComp.color.b, 1f);

            AnimatorComp.SetTrigger("IsActive");
            AnimatorComp.SetBool("IsPressed", false);
            playerIsHoldingDownButton = false;
        }

        // a bug fix for the instance of when a starts player holds down the x button during the menu -- the button should be activated by this
        if (playerIsHoldingDownButton == false && playerMovmentScript.playerInput.confirmAction.IsPressed == true)
        {
            spriteRendererComp.enabled = false;
            playerIsHoldingDownButton = true;
        }
    }

    public void ActivateImage()
    {
        //spriteRendererComp.color = new Color(spriteRendererComp.color.r, spriteRendererComp.color.g, spriteRendererComp.color.b, 1f);
        spriteRendererComp.enabled = true;
        isActivated = true;

        AnimatorComp.SetTrigger("IsActive");
    }

    public void DisableImage()
    {
        spriteRendererComp.color = new Color(spriteRendererComp.color.r, spriteRendererComp.color.g, spriteRendererComp.color.b, 1f);
        spriteRendererComp.enabled = false;
        playerIsHoldingDownButton = false;
        AnimatorComp.SetBool("IsPressed", false);
        isActivated = false;
    }

    public void ChangeImage()
    {
        spriteRendererComp.sprite = activeSprite;
        changedSprite = true;
    }

    public void RevertImage()
    {
        if (spriteRendererComp.enabled == false)
        {
            spriteRendererComp.color = new Color(spriteRendererComp.color.r, spriteRendererComp.color.g, spriteRendererComp.color.b, 1f);
            spriteRendererComp.enabled = true;
        }

        if (spriteRendererComp.color.a == 0.5f)
        {
            spriteRendererComp.color = new Color(spriteRendererComp.color.r, spriteRendererComp.color.g, spriteRendererComp.color.b, 1f);
            AnimatorComp.SetBool("IsPressed", false);
            AnimatorComp.SetTrigger("IsActive");
            playerIsHoldingDownButton = false;
        }

        spriteRendererComp.sprite = originalSprite;
        changedSprite = false;
    }
}
