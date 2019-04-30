using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ControllerSetUp : MonoBehaviour
{

    public enum ControllerDevicesInputed
    {
        keyboard,
        oneController,
        twoOrMoreControllers
    }

    public ControllerDevicesInputed currentState = ControllerDevicesInputed.keyboard;
    
    Movement player1Input;
    Movement player2Input;

    public GameObject rightArrow, leftArrow;    // Input Arrows

    [Header("Menu Controller Options")]
    public GameObject[] controllerInputOptions;

    // variable for what options are availible in the menu
    int maxInputOptionsAvalible = 0;
    int currentInputOptionIndex = 0;

    // Variables for the cursors on the pause menu
    int menuIndex = 0;
    bool p1HoldingDirection = false;
    bool p2HoldingDirection = false;

    bool optionConfirmed = false;

    // Use this for initialization
    void Start()
    {
        // Input Set Up
        player1Input = NewControllerManager.instance.player1Movement;
        player2Input = NewControllerManager.instance.player2Movement;
        

        optionConfirmed = false;    // If brought back to this scene then be able to reset the controller options
    }

    // Update is called once per frame
    void Update()
    {
        FindInputProfiles();

        ControllerMenuCursorMovement(player1Input.playerInput, p1HoldingDirection);
        ControllerMenuCursorMovement(player2Input.playerInput, p2HoldingDirection);

        // when the number of controllers changes, change the number of avalible inputs to chose from
    }

    void FindInputProfiles()
    {
        // figure out what types of controllers are in the scene
        if (InputManager.Devices.Count == 0 && currentState != ControllerDevicesInputed.keyboard)
        {
            // only a keyboard is plugged in
            currentState = ControllerDevicesInputed.keyboard;
            maxInputOptionsAvalible = 0;

            CheckAvalibleMenuOptions(); // Check if options in the menu need to be disabled

            rightArrow.SetActive(false);
            leftArrow.SetActive(false);
        }
        else if (InputManager.Devices.Count == 1 && currentState != ControllerDevicesInputed.oneController)
        {
            // one contorller and a keyboard are plugged in
            currentState = ControllerDevicesInputed.oneController;
            maxInputOptionsAvalible = 2;

            CheckAvalibleMenuOptions(); // Check if options in the menu need to be disabled

            rightArrow.SetActive(true);
            leftArrow.SetActive(true);
        }
        else if (InputManager.Devices.Count >= 2 && currentState != ControllerDevicesInputed.twoOrMoreControllers)
        {
            // two or more keyboards are plugged in and a keyboard
            currentState = ControllerDevicesInputed.twoOrMoreControllers;
            maxInputOptionsAvalible = 3;

            CheckAvalibleMenuOptions(); // Check if options in the menu need to be disabled

            rightArrow.SetActive(true);
            leftArrow.SetActive(true);
        }
    }

    void CheckAvalibleMenuOptions()
    {
        if (menuIndex != maxInputOptionsAvalible)
        {
            menuIndex = maxInputOptionsAvalible;

            // set the menu options
            for (int i = 0; i < controllerInputOptions.Length; i++)
            {
                if (i == menuIndex)
                {
                    controllerInputOptions[menuIndex].SetActive(true);
                }
                else
                {
                    controllerInputOptions[i].SetActive(false);
                }
            }
        }
    }


    void ControllerMenuCursorMovement(MyCharacterActions playerInput, bool holdingDirection)
    {
        float input = playerInput.horizontalAxis.Value;
        int rawInput = 0;

        // Get the input from the stick and account for dead zones
        if (Mathf.Abs(input) < 0.35f)
        {
            rawInput = 0;
            holdingDirection = false;
        }
        else if (Mathf.Abs(input) > 0.35f)
        {
            rawInput = (int)Mathf.Sign(input) * 1;
        }

        // if the player is holding the stick make it so that the cursor dosn't move at super speeds
        if (Mathf.Abs(rawInput) > 0 && holdingDirection == false)
        {
            holdingDirection = true;
            menuIndex = (menuIndex + rawInput) % (maxInputOptionsAvalible + 1);

            // If the player is holding the opposite direction then make the index loop properly
            if (Mathf.Sign(menuIndex) == -1)
            {
                menuIndex = (maxInputOptionsAvalible + 1) + menuIndex;
            }

            // set the menu options
            for (int i = 0; i < controllerInputOptions.Length; i++)
            {
                if (i == menuIndex)
                {
                    controllerInputOptions[menuIndex].SetActive(true);
                }
                else
                {
                    controllerInputOptions[i].SetActive(false);
                }
            }
        }

        if (playerInput == player1Input.playerInput)
        {
            p1HoldingDirection = holdingDirection;
        }
        else if (playerInput == player2Input.playerInput)
        {
            p2HoldingDirection = holdingDirection;
        }
    }

    public void ExecuteControllerInputType()
    {
        // only call this option once
        if (optionConfirmed == false)
        {
            optionConfirmed = true;
            
            foreach(GameObject controllerOption in controllerInputOptions)
            {
                // if the only active option is active then set those controller type(s)
                if(controllerOption.activeSelf == true)
                {
                    ControllerDeviceOption controllerDevice = controllerOption.GetComponent<ControllerDeviceOption>();
                    controllerDevice.OnChosenInputDevice.Invoke();
                    break;
                }
            }
        }
    }
}
//TODO
/*
 * ! Make Arrows disappear when its only keyboard
 * - Add animations to the arrows for feedback when the stick is pressed
 * - Add animation when the speech bubbles appear
 * ! prevent players from being able to hold the stick down to cycle through the inputs
 * ! Set up player 2's input on the menu
 *      ! Allow each players to cycle through the menu in either direction
 * - Make it so when you "Confirm" it saves what input is saved to the controllers
 * ! Fix keyboard controls
 * ! BUG: If there is only the keyboard plugged in and then a single controller is plugged in then 
 *      the if the player with the keyboard presses space then both options on keyboard will confirm 
 *      the controllers need to be reset in the main menu script
 * ! Keyboard is disabled upon the plugging in of 2 controllers
 * - players can still jump with the keyboard even though keyboard is disabled when 2 or more controllers are plugged in 
 * - When an option has been confirmed then prevent the players from cycling through the options in the menu
 * ! The fader image needs be on top of the Controller set up graphics
 * - If the player has a controller plugging and they want to select an option that lets them play the game in 1 player mode
 *      Then they need to press the confirm button on both inputs to play with one input
 *      Should this be changed?
 */
