using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorActiveControl : MonoBehaviour
{

    Animator animatorComponent;
    InteractiveMenu interactiveMenuComponent;


    // Use this for initialization
    void Start()
    {
        animatorComponent = GetComponent<Animator>();
        interactiveMenuComponent = GetComponentInParent<InteractiveMenu>();
    }

    public void DisableAnimator()
    {
        animatorComponent.enabled = false;
    }

    public void EnableAnimator()
    {
        animatorComponent.enabled = true;
    }

    public void DisbaleParent()
    {
        animatorComponent.gameObject.SetActive(false);
    }

    public void ChangeMenuState(InteractiveMenu.PauseState nextPuaseState)
    {
        interactiveMenuComponent.currentState = nextPuaseState;
    }

    public void SetHandsToStartingPos()
    {
        interactiveMenuComponent.SetHandStartingPositions();
    }
}
