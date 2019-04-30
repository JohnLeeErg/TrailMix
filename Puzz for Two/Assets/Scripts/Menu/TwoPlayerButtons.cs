using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InControl;

public class TwoPlayerButtons : MonoBehaviour
{

    public enum ActionState
    {
        InGame,
        Paused
    }

    public ActionState currentState;

    [Header("Action Menu Graphics")]
    [SerializeField] GameObject p1ActionBubble;
    [SerializeField] GameObject p2ActionBubble;
    Animator p1ActionAnimator, p2ActionAnimator;
    bool p1ActionCalled = false;
    bool p2ActionCalled = false;

    NewControllerManager controllerManager;
    Movement player1Movement;
    Movement player2Movement;

    [Header("Exicutable Unity Event")]
    public UnityEvent OnBothPlayerInputDown;

    IEnumerator waitEnum;
    float pauseTime = 0.5f;

    // Use this for initialization
    void Start()
    {
        // Input Set Up
        controllerManager = NewControllerManager.instance;
        player1Movement = controllerManager.player1Movement;
        player2Movement = controllerManager.player2Movement;

        p1ActionAnimator = p1ActionBubble.GetComponent<Animator>();
        p2ActionAnimator = p2ActionBubble.GetComponent<Animator>();

        p1ActionBubble.SetActive(true);
        p2ActionBubble.SetActive(true);

        waitEnum = WaitTime(pauseTime, ActionState.Paused);
    }

    // Update is called once per frame
    void Update()
    {
        //// Input Set Up
        //if (controllerManager.controllerTypeInputted == CurrentControllerSetup.Undecided)
        //{
        //    if (player1Input != controllerManager.player1Input)
        //    {
        //        player1Input = controllerManager.player1Input;
        //    }
        //    if (player2Input != controllerManager.player2Input)
        //    {
        //        player2Input = controllerManager.player2Input;
        //    }
        //}

        switch (currentState)
        {
            case (ActionState.InGame):
                // Prevent the multi-call of the IEnumerator 
                if (p1ActionCalled == true || p2ActionCalled == true)
                {
                    if (player1Movement.playerInput.resetAction.WasReleased)
                    {
                        p1ActionCalled = false;
                    }
                    if (player2Movement.playerInput.resetAction.WasReleased)
                    {
                        p2ActionCalled = false;
                    }
                }

                if (controllerManager.controllerTypeInputted == CurrentControllerSetup.OnePlayerKeyboard
                    || controllerManager.controllerTypeInputted == CurrentControllerSetup.OnePlayerController
                    || controllerManager.controllerTypeInputted == CurrentControllerSetup.Undecided)
                {
                    CheckForOnePlayerDoubleInputs();

                    if ((player1Movement.playerInput.resetAction.IsPressed && p1ActionCalled == false) || (player2Movement.playerInput.resetAction.IsPressed && p2ActionCalled == false))
                    {
                        //print("Call Action");

                        // Invoke the IEnumerator that calls the Unity Event after a delay
                        StartCoroutine(waitEnum);
                    }
                }
                else if (controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard
                    || controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController
                    || controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
                {
                    // Check for the pause Inputs
                    CheckInitalPauseMenuInputs();

                    if (player1Movement.playerInput.resetAction.IsPressed && player2Movement.playerInput.resetAction.IsPressed && p1ActionCalled == false && p2ActionCalled == false)
                    {
                        //print("Call Action");

                        // Invoke the IEnumerator that calls the Unity Event after a delay
                        StartCoroutine(waitEnum);
                    }
                }
                break;
            case (ActionState.Paused):
                // Do nothing but wait
                break;
        }
    }

    void CheckForOnePlayerDoubleInputs()
    {
        if (player1Movement.playerInput.resetAction.WasPressed || player2Movement.playerInput.resetAction.WasPressed)
        {
            p1ActionAnimator.SetBool("Open", true);
            p1ActionAnimator.SetBool("Close", false);
            p2ActionAnimator.SetBool("Open", true);
            p2ActionAnimator.SetBool("Close", false);
        }
    }

    /// <summary>
    /// waits for there to be a device corresponding with this player's id (ie there are enough plugged in)
    /// </summary>
    void CheckInitalPauseMenuInputs()
    {
        if (player1Movement.playerInput.resetAction.WasPressed)
        {
            p1ActionAnimator.SetBool("Open", true);
            p1ActionAnimator.SetBool("Close", false);
        }
        else if (player1Movement.playerInput.resetAction.WasReleased)
        {
            p1ActionAnimator.SetBool("Close", true);
            p1ActionAnimator.SetBool("Open", false);
        }

        if (player2Movement.playerInput.resetAction.WasPressed)
        {
            p2ActionAnimator.SetBool("Open", true);
            p2ActionAnimator.SetBool("Close", false);
        }
        else if (player2Movement.playerInput.resetAction.WasReleased)
        {
            p2ActionAnimator.SetBool("Close", true);
            p2ActionAnimator.SetBool("Open", false);
        }
    }

    IEnumerator WaitTime(float waitAmount, ActionState nextState)
    {
        currentState = nextState;
        p1ActionCalled = true;
        p2ActionCalled = true;

        yield return new WaitForSeconds(waitAmount);    // Delay the call of the Unity Event

        // reset the puase icon animations
        p1ActionAnimator.SetBool("Close", true);
        p2ActionAnimator.SetBool("Close", true);
        p1ActionAnimator.SetBool("Open", false);
        p2ActionAnimator.SetBool("Open", false);

        OnBothPlayerInputDown.Invoke(); // Call Unity Event
    }

    public void ResetAction()
    {
        // reset the script
        waitEnum = WaitTime(pauseTime, ActionState.Paused);
        currentState = ActionState.InGame;
    }
}
