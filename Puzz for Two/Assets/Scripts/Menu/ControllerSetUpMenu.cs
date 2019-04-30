using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ControllerSetUpMenu : MonoBehaviour
{
    // ENUMS ===============================
    public enum MenuClickState
    {
        UnConfirmed,
        Confirmed
    }

    public enum ControllerDevicesInputed
    {
        keyboard,
        oneController,
        twoOrMoreControllers
    }

    //--------------------------------------------------------------------------------------------
    // TITLE MENU VARIABLES 
    //--------------------------------------------------------------------------------------------
    [Header("Title Menu Variables")]
    public GameObject titleMenuParent;
    Animator titleMenuAnimator;
    public bool enableSwapOfMenus = false;
    bool menusHaveBeenSwaped = false;

    //--------------------------------------------------------------------------------------------
    // PAUSE MENU VARIABLES 
    //--------------------------------------------------------------------------------------------
    MenuClickState p1CurrentClickState = MenuClickState.UnConfirmed;
    MenuClickState p2CurrentClickState = MenuClickState.UnConfirmed;

    [Header("Pause Menu Variables")]
    [SerializeField] GameObject p1Hand;
    [SerializeField] GameObject p2Hand;
    Animator p1HandAnimator, p2HandAnimator;

    // Variables for the cursors on the pause menu
    int player1MenuIndex = 0, player2MenuIndex = 0;
    float p1MenuCursorTime = 0, p2MenuCursorTime = 0;
    float maxCursorTime = 0.25f;
    bool p1ConfirmedOption = false, p2ConfirmedOption = false;
    bool optionPicked = false;

    [Header("Button in Menu")]
    public MenuButton menuButton;
    Animator menuButtonAnimator;

    //-------------------------------------------------------------------------------------
    // CONTROLLER SETUP VARIABLES 
    //-------------------------------------------------------------------------------------
    [Header("Controller Setup Variables")]
    public ControllerDevicesInputed currentState = ControllerDevicesInputed.keyboard;

    public GameObject controllerMenuParent;
    public GameObject rightArrow, leftArrow;    // Input Arrows
    Animator rightArrowAnimator, leftArrowAnimator;

    // variable for what options are availible in the menu
    public int maxInputOptionsAvalible = 0;

    // Variables for the cursors on the pause menu
    public int menuIndex = 0;
    bool p1HoldingDirection = false, p2HoldingDirection = false;
    bool optionConfirmed = false;

    [Header("Controller Options in Menu")]
    public GameObject[] controllerInputOptions;
    public ControllerDeviceOption[] controllerDeviceOptions;

    //-------------------------------------------------------------------------------------
    // SHEARED VALUES 
    //-------------------------------------------------------------------------------------
    [Header("Player Input")]
    public Movement player1Movement;
    public Movement player2Movement;
    //MyCharacterActions player1Input;
    //MyCharacterActions player2Input;
    NewControllerManager newControllerManager;

    [Header("FMOD EVENTS")]
    [FMODUnity.EventRef] public string closeMenu;
    [FMODUnity.EventRef] public string PlayerConfirm;
    [FMODUnity.EventRef] public string ContorllerSelect;
    FMOD.Studio.EventInstance closeMenuEvent, PlayerConfirmEvent, ControllerSelectEvent;

    // Use this for initialization
    void Start()
    {
        if (controllerMenuParent.activeSelf == false)
        {
            controllerMenuParent.SetActive(true);
        }

        // Input Set Up
        newControllerManager = NewControllerManager.instance;

        // Get Components
        p1HandAnimator = p1Hand.GetComponentInChildren<Animator>();
        p2HandAnimator = p2Hand.GetComponentInChildren<Animator>();
        menuButtonAnimator = menuButton.GetComponent<Animator>();
        rightArrowAnimator = rightArrow.GetComponent<Animator>();
        leftArrowAnimator = leftArrow.GetComponent<Animator>();
        titleMenuAnimator = titleMenuParent.GetComponent<Animator>();

        // Fmod stuff
        closeMenuEvent = FMODUnity.RuntimeManager.CreateInstance(closeMenu);
        PlayerConfirmEvent = FMODUnity.RuntimeManager.CreateInstance(PlayerConfirm);
        ControllerSelectEvent = FMODUnity.RuntimeManager.CreateInstance(ContorllerSelect);

        //Set the canvas to active if it is not
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        // SET THE MENU ICON POSITION
        p1Hand.transform.position = new Vector2(p1Hand.transform.position.x, menuButton.transform.position.y);
        p2Hand.transform.position = new Vector2(p2Hand.transform.position.x, menuButton.transform.position.y);

        optionConfirmed = false;    // If brought back to this scene then be able to reset the controller options


        // Check if options in the menu need to be disabled
        if (InputManager.Devices.Count == 0)
        {
            maxInputOptionsAvalible = 1;
        }
        else if (InputManager.Devices.Count == 1)
        {
            maxInputOptionsAvalible = 3;
        }
        else if (InputManager.Devices.Count >= 2)
        {
            maxInputOptionsAvalible = 4;
        }

        if (maxInputOptionsAvalible == 1)
        {
            menuIndex = 0;
        }
        else
        {
            menuIndex = maxInputOptionsAvalible;
        }

        // set the correct options to be active depending on the number of controllers plugged in
        ActivateProperControllerOptions();

        // set the menu options -- DISBALED FOR NEW FEATURE
        //for (int i = 0; i < controllerInputOptions.Length; i++)
        //{
        //    if (i == menuIndex)
        //    {
        //        controllerInputOptions[menuIndex].SetActive(true);
        //    }
        //    else
        //    {
        //        controllerInputOptions[i].SetActive(false);
        //    }
        //}

        //if (InputManager.Devices.Count == 0)
        //{
        //    rightArrow.SetActive(false);
        //    leftArrow.SetActive(false);
        //}
        if(NewControllerManager.instance && NewControllerManager.instance.demoMode)
        {
            enableSwapOfMenus = false;
        }
        else
        {
            enableSwapOfMenus = true;
        }
        if (enableSwapOfMenus == true && controllerMenuParent.activeSelf == true)
        {
            controllerMenuParent.SetActive(false);
        }
        if (enableSwapOfMenus == false && controllerMenuParent.activeSelf == true && titleMenuParent.activeSelf == true)
        {
            controllerMenuParent.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        FindInputProfiles();    // Find the player's input
        WaitForBothPlayerConfirmOnMenu();

        ControllerMenuCursorMovement(player1Movement.playerInput, p1HoldingDirection);
        ControllerMenuCursorMovement(player2Movement.playerInput, p2HoldingDirection);

        if (menuIndex > maxInputOptionsAvalible && (p1ConfirmedOption == true || p2ConfirmedOption == true))
        {
            ResetHands();
        }
    }

    void FindInputProfiles()
    {
        // figure out what types of controllers are in the scene
        if (InputManager.Devices.Count == 0 && currentState != ControllerDevicesInputed.keyboard)
        {
            // only a keyboard is plugged in
            currentState = ControllerDevicesInputed.keyboard;
            maxInputOptionsAvalible = 1;

            CheckAvalibleControllerMenuOptions(); // Check if options in the menu need to be disabled

            // Animation stuff
            ArrowAnimations();

            // Figure out what contorller options need to be activated
            ActivateProperControllerOptions();
            ResetHands();
            //rightArrow.SetActive(false);
            //leftArrow.SetActive(false);

            //if (rightArrow.activeSelf == true && leftArrow.activeSelf == true
            //    && rightArrowAnimator.GetBool("IsUnactive") == false && leftArrowAnimator.GetBool("IsUnactive") == false)
            //{
            //    //print("this is happening");

            //    rightArrowAnimator.SetBool("IsUnactive", true);
            //    leftArrowAnimator.SetBool("IsUnactive", true);
            //    rightArrowAnimator.SetTrigger("Unactive");
            //    leftArrowAnimator.SetTrigger("Unactive");
            //}
        }
        else if (InputManager.Devices.Count == 1 && currentState != ControllerDevicesInputed.oneController)
        {
            // one contorller and a keyboard are plugged in
            currentState = ControllerDevicesInputed.oneController;
            maxInputOptionsAvalible = 3;

            CheckAvalibleControllerMenuOptions(); // Check if options in the menu need to be disabled

            // Animation stuff
            ArrowAnimations();

            // Figure out what contorller options need to be activated
            ActivateProperControllerOptions();
            ResetHands();
        }
        else if (InputManager.Devices.Count >= 2 && currentState != ControllerDevicesInputed.twoOrMoreControllers)
        {
            // two or more keyboards are plugged in and a keyboard
            currentState = ControllerDevicesInputed.twoOrMoreControllers;
            maxInputOptionsAvalible = 4;

            CheckAvalibleControllerMenuOptions(); // Check if options in the menu need to be disabled

            // Animation stuff
            ArrowAnimations();

            // Figure out what contorller options need to be activated
            ActivateProperControllerOptions();
            ResetHands();
        }
    }

    void ArrowAnimations()
    {
        if (rightArrow.activeSelf == false || leftArrow.activeSelf == false)
        {
            rightArrow.SetActive(true);
            leftArrow.SetActive(true);
            rightArrowAnimator.SetTrigger("IsActive");
            leftArrowAnimator.SetTrigger("IsActive");
        }

        if (rightArrowAnimator.GetBool("IsUnactive") == true || leftArrowAnimator.GetBool("IsUnactive") == true)
        {
            rightArrowAnimator.SetBool("IsUnactive", false);
            leftArrowAnimator.SetBool("IsUnactive", false);
            rightArrowAnimator.SetTrigger("IsActive");
            leftArrowAnimator.SetTrigger("IsActive");
        }

        if (rightArrowAnimator.enabled == false || leftArrowAnimator.enabled == false)
        {
            rightArrowAnimator.enabled = true;
            leftArrowAnimator.enabled = true;
        }
    }

    void WaitForBothPlayerConfirmOnMenu()
    {
        PauseMenuCursorConfirm(p1CurrentClickState, player1Movement.playerInput, p1ConfirmedOption, p1HandAnimator);   // Check for the Confirm Input for the Player's Menu Cursor
        PauseMenuCursorConfirm(p2CurrentClickState, player2Movement.playerInput, p2ConfirmedOption, p2HandAnimator);

        // if both player's have confirmed an action and are on the same option in the menu then execute it
        if (p1ConfirmedOption == true && p2ConfirmedOption == true && player1MenuIndex == player2MenuIndex && optionPicked == false)
        {
            if (menuIndex < 2)
            {
                player1Movement.CinematicJump();
                player2Movement.CinematicJump();
            }

            optionPicked = true;
            //print(menuButton.gameObject.name);

            p1HandAnimator.SetBool("IsSelected", true);
            p2HandAnimator.SetBool("IsSelected", true);

            menuButtonAnimator.SetBool("IsConfirmed", true);

            // if a menu needs to be swaped wait a given time then swap the menus
            if (enableSwapOfMenus == true && titleMenuParent.activeSelf == true)
            {
                if (menusHaveBeenSwaped == false)
                {
                    IEnumerator waitTimeEnum = WaitGivenTimeToChangeManus(1f);
                    StartCoroutine(waitTimeEnum);

                    menusHaveBeenSwaped = true;
                    closeMenuEvent.start(); // play audio
                }
            }

            if (controllerMenuParent.activeSelf == true || enableSwapOfMenus == false)
            {
                if (menuIndex <= maxInputOptionsAvalible)
                {
                    //INVOKE THE BUTTON'S FUNCTION
                    menuButton.onConfirm.Invoke();

                    //AND CONFIRM THE CONTROLLERS
                    ExecuteControllerInputType();
                }
            }
        }
    }

    #region INPUT ON MENUS ========================================================================================
    void ControllerMenuCursorMovement(MyCharacterActions playerInput, bool holdingDirection)
    {
        if (titleMenuParent.activeSelf == false)
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
                menuIndex = (menuIndex + rawInput) % (controllerDeviceOptions.Length);

                // If the player is holding the opposite direction then make the index loop properly
                if (Mathf.Sign(menuIndex) == -1)
                {
                    menuIndex = (controllerDeviceOptions.Length) + menuIndex;
                }

                // set the menu options
                for (int i = 0; i < controllerInputOptions.Length; i++)
                {
                    if (i == menuIndex)
                    {
                        controllerInputOptions[menuIndex].SetActive(true);
                        ControllerSelectEvent.start();  //play audio

                        // Animate the directional arrows
                        if (Mathf.Sign(rawInput) == -1 && leftArrowAnimator.enabled == true && leftArrow.activeSelf == true)
                        {
                            leftArrowAnimator.SetTrigger("IsUsed");
                        }
                        if (Mathf.Sign(rawInput) == 1 && rightArrowAnimator.enabled == true && rightArrow.activeSelf == true)
                        {
                            rightArrowAnimator.SetTrigger("IsUsed");
                        }
                    }
                    else
                    {
                        controllerInputOptions[i].SetActive(false);
                    }
                }
            }

            if (playerInput == player1Movement.playerInput)
            {
                p1HoldingDirection = holdingDirection;
            }
            else if (playerInput == player2Movement.playerInput)
            {
                p2HoldingDirection = holdingDirection;
            }
        }
    }

    void PauseMenuCursorConfirm(MenuClickState currentClickState, MyCharacterActions playerInput, bool confirmAction, Animator handAnimator)
    {
        if (optionPicked == false && menuIndex <= maxInputOptionsAvalible)
        {
            // if the player presses A then confirm their cursor and prevent them from moveing
            switch (currentClickState)
            {
                // FOR SELECTING AN OPTION--------------------
                case (MenuClickState.UnConfirmed):
                    if ((playerInput.jumpAction.WasPressed || playerInput.confirmAction.WasPressed) && confirmAction == false)
                    {
                        confirmAction = true;
                        handAnimator.SetBool("IsSelected", true);

                        currentClickState = MenuClickState.Confirmed;

                        menuButtonAnimator.SetTrigger("Select");

                        PlayerConfirmEvent.start(); // play audio
                    }
                    break;
                // FOR UN-SELECTING AN OPTION--------------------
                case (MenuClickState.Confirmed):
                    if ((playerInput.jumpAction.WasPressed || playerInput.confirmAction.WasPressed) && confirmAction == true)
                    {
                        confirmAction = false;
                        handAnimator.SetBool("IsSelected", false);

                        currentClickState = MenuClickState.UnConfirmed;

                        menuButtonAnimator.SetTrigger("Unselect");
                    }
                    break;
            }

            // Set the respective values depending on the player 
            if (playerInput == player1Movement.playerInput)
            {
                p1ConfirmedOption = confirmAction;
                p1CurrentClickState = currentClickState;
            }
            else if (playerInput == player2Movement.playerInput)
            {
                p2ConfirmedOption = confirmAction;
                p2CurrentClickState = currentClickState;
            }

            SpecialCaseMenuConfirm();
        }
    }

    void SpecialCaseMenuConfirm()
    {
        // If the player is hovering over single player keyboard and hits confirm on the keyboard
        // no matter the amount of controllers plugged in it should confirm for both players 

        // FOR 1 KEYBOARD CONFIRM
        if (InputManager.Devices.Count == 0)
        {
            if (player1Movement.playerInput.jumpAction.WasPressed && player1Movement.playerInput.jumpAction.LastInputType == BindingSourceType.KeyBindingSource)
            {
                if (menuIndex <= 1)
                {
                    //print("ACTIVATE BOTH PLAYER CONFIRM BUTTONS");
                    ConfirmBothMenuOptions();
                }
            }
        }
        else if (InputManager.Devices.Count > 0)
        {
            if (player2Movement.playerInput.jumpAction.WasPressed && player2Movement.playerInput.jumpAction.LastInputType == BindingSourceType.KeyBindingSource)
            {
                if (menuIndex <= 1)
                {
                    //print("ACTIVATE BOTH PLAYER CONFIRM BUTTONS");
                    ConfirmBothMenuOptions();
                }
            }
        }


        // If the player is hovering over single player controller and hits confirm on the first player controller
        // no matter the amount of controllers plugged in it should confirm for both players 

        // FOR 1 CONTROLLER CONFIRM
        if (player1Movement.playerInput.jumpAction.WasPressed)
        {
            if (InputManager.Devices.Count > 0 && menuIndex == 1)
            {
                //print("ACTIVATE BOTH PLAYER CONFIRM BUTTONS");
                ConfirmBothMenuOptions();
            }
        }
    }
    #endregion

    void ConfirmBothMenuOptions()
    {
        // check if the other player's confirm option is true and activate the other
        if (p1ConfirmedOption == true && p2ConfirmedOption == false)
        {
            p2ConfirmedOption = true;
            p2CurrentClickState = MenuClickState.Confirmed;
            p2HandAnimator.SetBool("IsSelected", true);
        }
        if (p2ConfirmedOption == true && p1ConfirmedOption == false)
        {
            p1ConfirmedOption = true;
            p1CurrentClickState = MenuClickState.Confirmed;
            p1HandAnimator.SetBool("IsSelected", true);
        }
        if (p1ConfirmedOption == false && p2ConfirmedOption == false)
        {
            p1ConfirmedOption = true;
            p1CurrentClickState = MenuClickState.Confirmed;
            p1HandAnimator.SetBool("IsSelected", true);
            p2ConfirmedOption = true;
            p2CurrentClickState = MenuClickState.Confirmed;
            p2HandAnimator.SetBool("IsSelected", true);
        }
    }

    void CheckAvalibleControllerMenuOptions()
    {
        if (menuIndex != maxInputOptionsAvalible)
        {
            if (maxInputOptionsAvalible == 1)
            {
                menuIndex = 0;
            }
            else
            {
                menuIndex = maxInputOptionsAvalible;
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
    }

    public void ExecuteControllerInputType()
    {
        // only call this option once
        if (optionConfirmed == false)
        {
            optionConfirmed = true;

            for (int i = 0; i < controllerInputOptions.Length; i++)
            {
                // if the only active option is active then set those controller type(s)
                GameObject controllerOption = controllerInputOptions[i];

                if (controllerOption.activeSelf == true && controllerMenuParent.activeSelf == true)
                {
                    ControllerDeviceOption controllerDevice = controllerOption.GetComponent<ControllerDeviceOption>();
                    controllerDevice.OnChosenInputDevice.Invoke();
                    break;
                }

                // create a instance where the game defaults to 2 player controllers when no option is selected
                if (i == controllerInputOptions.Length - 1 && controllerMenuParent.activeSelf == false)
                {
                    SetControllersToTwoPlayerControllers();
                }
            }
        }
    }

    void SetControllersToTwoPlayerControllers()
    {
        newControllerManager.swapPermitted = false;
        newControllerManager.ResetBothIncontrolThings();

        if (InputManager.Devices.Count >= 2)
        {
            newControllerManager.controllerTypeInputted = CurrentControllerSetup.TwoPlayerController;
        }
        else if (InputManager.Devices.Count == 1)
        {
            newControllerManager.controllerTypeInputted = CurrentControllerSetup.OnePlayerController;
        }
        else if (InputManager.Devices.Count == 0)
        {
            newControllerManager.controllerTypeInputted = CurrentControllerSetup.OnePlayerKeyboard;
        }
    }

    public void ResetHands()
    {
        p1ConfirmedOption = false;
        p1HandAnimator.SetBool("IsSelected", false);
        p1CurrentClickState = MenuClickState.UnConfirmed;

        p2ConfirmedOption = false;
        p2HandAnimator.SetBool("IsSelected", false);
        p2CurrentClickState = MenuClickState.UnConfirmed;

        menuButtonAnimator.SetTrigger("Unselect");
    }

    public void ActivateSecondMenu()
    {
        optionPicked = false;
        controllerMenuParent.SetActive(true);       
    }

    void ActivateProperControllerOptions()
    {
        for (int i = 0; i < controllerDeviceOptions.Length; i++)
        {
            if (i <= maxInputOptionsAvalible)
            {
                if (controllerInputOptions[i].activeSelf == false)
                {
                    controllerInputOptions[i].SetActive(true);
                }

                controllerDeviceOptions[i].EnableOption();

                if (maxInputOptionsAvalible == 1 )
                {
                    if (i == 1)
                    {
                        controllerInputOptions[i].SetActive(false);
                    }
                }
                else
                {
                    if (i < maxInputOptionsAvalible)
                    {
                        controllerInputOptions[i].SetActive(false);
                    }
                }
            }
            else
            {
                if (controllerInputOptions[i].activeSelf == false)
                {
                    controllerInputOptions[i].SetActive(true);
                }

                controllerDeviceOptions[i].DisbaleOption();
                controllerInputOptions[i].SetActive(false);
            }
        }
    }

    IEnumerator WaitGivenTimeToChangeManus(float waitTime)
    {
        titleMenuAnimator.SetBool("Deactivate", true);

        yield return new WaitForSeconds(waitTime);

        titleMenuParent.SetActive(false);
        ResetHands();
        ActivateSecondMenu();
    }
}
