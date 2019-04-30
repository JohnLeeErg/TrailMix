using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class InteractiveMenu : MonoBehaviour
{

    public enum PauseState
    {
        InGame,
        WaitForPauseIn,
        Paused,
        WaitForPauseOut,
        ResetValues,
    }

    public enum MenuClickState
    {
        UnConfirmed,
        Confirmed
    }

    public PauseState currentState;

    MenuClickState p1CurrentClickState = MenuClickState.UnConfirmed;
    MenuClickState p2CurrentClickState = MenuClickState.UnConfirmed;

    public static InteractiveMenu instance;

    // State Transition Values
    IEnumerator waitEnum;
    bool callOnce = true;   // Start as true to prevent first "callOnce" if statement in "InGame" State

    GameObject player1, player2;
    Movement p1Movement, p2Movement;
    PlayerThrowing p1Throwing, p2Throwing;
    PlayerHealth p1Health, p2Health;
    PlayerIndicator p1Indicator, p2Indicator;
    [SerializeField] GameObject p1Pause, p2Pause;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject p1Hand, p2Hand;
    Animator p1PauseAnimator, p2PauseAnimator;
    Animator p1HandAnimator, p2HandAnimator;
    Animator pauseMenuAnimator;

    // Variables for the cursors on the pause menu
    int player1MenuIndex = 0;
    int player2MenuIndex = 0;
    float p1MenuCursorTime = 0;
    float p2MenuCursorTime = 0;
    float maxCursorTime = 0.25f;
    bool p1ConfirmedPauseOption = false;
    bool p2ConfirmedPauseOption = false;
    bool p1ConfirmedHandOption = false;
    bool p2ConfirmedHandOption = false;
    bool optionPicked = false;

    [Header("Buttons in Menu")]
    public MenuButton[] fullMenuButtons,demoMenuButtons;
    MenuButton[] menuButtons;
    Animator[] menuButtonAnimators;

    bool canPause = true;

    NewControllerManager controllerManager;

    public bool holdingPauseOnStart = true;

    RectTransform startingButtonPosition; // Create a variable that should store the position of the first option in the menu
    RectTransform p1RectTransform;
    RectTransform p2RectTransform;

    [FMODUnity.EventRef]
    public string closeMenu,FadeToBlack,Goodbye,InGameMenuSelect,OpenMenu,Player1Confirm,Player2Confirm,UIDeconfirm;
    FMOD.Studio.EventInstance closeMenuEvent,FadeToBlackEvent,GoodbyeEvent,InGameMenuSelectEvent,OpenMenuEvent,Player1ConfirmEvent,Player2ConfirmEvent,UIDeconfirmEvent;
    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        controllerManager = NewControllerManager.instance;

        // Find the Players
        if (!player1)
        {
            player1 = GameObject.Find("Player 1");
        }
        if (!player2)
        {
            player2 = GameObject.Find("Player 2");
        }

        // Find the various components on the players
        p1Movement = player1.GetComponent<Movement>();
        p1Throwing = player1.GetComponent<PlayerThrowing>();
        p1Health = player1.GetComponent<PlayerHealth>();
        p1Indicator = player1.GetComponentInChildren<PlayerIndicator>();
        p2Movement = player2.GetComponent<Movement>();
        p2Throwing = player2.GetComponent<PlayerThrowing>();
        p2Health = player2.GetComponent<PlayerHealth>();
        p2Indicator = player2.GetComponentInChildren<PlayerIndicator>();

        p1PauseAnimator = p1Pause.GetComponent<Animator>();
        p2PauseAnimator = p2Pause.GetComponent<Animator>();
        p1HandAnimator = p1Hand.GetComponentInChildren<Animator>();
        p2HandAnimator = p2Hand.GetComponentInChildren<Animator>();
        pauseMenuAnimator = pauseMenu.GetComponent<Animator>();


        //Set the canvas to active if it is not
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        p1Pause.SetActive(true);
        p2Pause.SetActive(true);
        //pauseMenu.SetActive(false);

        p1RectTransform = p1Hand.GetComponent<RectTransform>();
        p2RectTransform = p2Hand.GetComponent<RectTransform>();

        //instanciate FMOD sound variables/events
        closeMenuEvent =FMODUnity.RuntimeManager.CreateInstance(closeMenu);
        InGameMenuSelectEvent = FMODUnity.RuntimeManager.CreateInstance(InGameMenuSelect);
        OpenMenuEvent = FMODUnity.RuntimeManager.CreateInstance(OpenMenu);
        Player1ConfirmEvent = FMODUnity.RuntimeManager.CreateInstance(Player1Confirm);
        Player2ConfirmEvent = FMODUnity.RuntimeManager.CreateInstance(Player2Confirm);

        if(NewControllerManager.instance && NewControllerManager.instance.demoMode)
        {
            foreach(MenuButton eachButton in demoMenuButtons)
            {
                eachButton.gameObject.SetActive(true);
            }
            menuButtons = demoMenuButtons;
        }
        else
        {
            foreach (MenuButton eachButton in fullMenuButtons)
            {
                eachButton.gameObject.SetActive(true);
            }
            menuButtons = fullMenuButtons;
        }

        // find the button animators
        menuButtonAnimators = new Animator[menuButtons.Length];
        for (int i = 0; i < menuButtonAnimators.Length; i++)
        {
            menuButtonAnimators[i] = menuButtons[i].GetComponent<Animator>();
        }


        startingButtonPosition = menuButtons[0].GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canPause)   // Only allow players to pause during gameplay
        {
            // Current Pasue State
            switch (currentState)
            {
                case PauseState.InGame:
                    if (holdingPauseOnStart == false)
                    {
                        InGame();
                    }
                    break;
                case PauseState.WaitForPauseIn:
                    // When pause buttons are pressed - wait a bit for the bubbles to dispease
                    // then start the animation for the menu to come in
                    break;
                case PauseState.Paused:
                    Paused();
                    break;
                case PauseState.WaitForPauseOut:
                    // Delay the input so that both players understand whats going on
                    // Then play the animation
                    // when the animation ends reset all the values in the menu and then resume the game
                    break;
                case PauseState.ResetValues:
                    if (callOnce == false)
                    {
                        waitEnum = WaitTime(0.25f);
                        StartCoroutine(waitEnum);
                    }
                    break;      
            }
        }

        // To prevent the player from starting up the menu when a new scene is loaded
        if (holdingPauseOnStart == true)
        {
            if (controllerManager.controllerTypeInputted == CurrentControllerSetup.OnePlayerKeyboard
                || controllerManager.controllerTypeInputted == CurrentControllerSetup.OnePlayerController
                || controllerManager.controllerTypeInputted == CurrentControllerSetup.Undecided)
            {
                if (p1Movement.playerInput.resetAction.IsPressed || p2Movement.playerInput.resetAction.IsPressed)
                {
                    holdingPauseOnStart = false;
                }
                if (p1Movement.playerInput.resetAction.IsPressed == false && p2Movement.playerInput.resetAction.IsPressed == false)
                {
                    holdingPauseOnStart = false;
                }
            }
            else if (controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
            {
                if (p1Movement.playerInput.resetAction.IsPressed && p2Movement.playerInput.resetAction.IsPressed)
                {
                    holdingPauseOnStart = false;
                }
                if (p1Movement.playerInput.resetAction.IsPressed == false && p2Movement.playerInput.resetAction.IsPressed)
                {
                    holdingPauseOnStart = false;
                }
                if (p2Movement.playerInput.resetAction.IsPressed == false && p1Movement.playerInput.resetAction.IsPressed)
                {
                    holdingPauseOnStart = false;
                }
                if (p1Movement.playerInput.resetAction.IsPressed == false && p2Movement.playerInput.resetAction.IsPressed == false)
                {
                    holdingPauseOnStart = false;
                }
            }
        }
    }

    void InGame()
    {
        if (callOnce == true)
        {
            callOnce = false;
        }

        // For 1 player control options
        if (controllerManager.controllerTypeInputted == CurrentControllerSetup.OnePlayerKeyboard
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.OnePlayerController
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.Undecided)
        {
            CheckInitalOnePlayerInputs();

            if ((p1Movement.playerInput.resetAction.IsPressed && p1ConfirmedPauseOption == true) || (p2Movement.playerInput.resetAction.IsPressed && p2ConfirmedPauseOption == true))
            {
                // Lock player Movement
                StopPlayersFunctionality();

                //pauseMenuAnimator.SetBool("IsActive", true);

                // Invoke the IEnumerator that calls the Unity Event after a delay
                waitEnum = WaitForPauseIn(0.5f);
                StartCoroutine(waitEnum);
            }

        }
        // For 2 player control options
        else if (controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
        {
            // Check for the pause Inputs
            CheckInitalPauseMenuInputs();

            if (p1Movement.playerInput.resetAction.IsPressed && p2Movement.playerInput.resetAction.IsPressed && p1ConfirmedPauseOption == true && p2ConfirmedPauseOption == true)
            {
                // reset the hand positions
                p1Hand.transform.position = new Vector2(p1Hand.transform.position.x, menuButtons[0].transform.position.y);
                p2Hand.transform.position = new Vector2(p2Hand.transform.position.x, menuButtons[0].transform.position.y);

                // Lock player Movement
                StopPlayersFunctionality();

                //pauseMenuAnimator.SetBool("IsActive", true);

                // Change States after a delay
                waitEnum = WaitForPauseIn(0.5f);
                StartCoroutine(waitEnum);
            }
        }
    }

    void Paused()
    {
        if (callOnce == true)
        {
            pauseMenuAnimator.enabled = false;

            p1ConfirmedPauseOption = false;
            p2ConfirmedPauseOption = false;

            callOnce = false;
        }

        // For 1 player control options
        if (controllerManager.controllerTypeInputted == CurrentControllerSetup.OnePlayerKeyboard
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.OnePlayerController
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.Undecided)
        {
            PauseMenuOnePlayerCursorMovement(); // Move the Player's Menu Cursor
            PauseMenuOnePlayerCursorConfirm();  // Check for the Confirm Input for the Player's Menu Cursor

            // if both player's have confirmed an action and are on the same option in the menu then execute it
            if (p1ConfirmedHandOption == true && player1MenuIndex == player2MenuIndex && optionPicked == false)
            {
                optionPicked = true;
                //print(menuButtons[player1MenuIndex].gameObject.name);

                p1HandAnimator.SetBool("IsSelected", true);
                p2HandAnimator.SetBool("IsSelected", true);

                //INVOKE THE BUTTON'S FUNCTION
                menuButtons[player1MenuIndex].onConfirm.Invoke();
                menuButtonAnimators[player1MenuIndex].SetBool("IsConfirmed", true);
            }
        }
        // For 2 player control options
        else if (controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController
            || controllerManager.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
        {
            PauseMenuCursorMovement();  // Move the Player's Menu Cursor
            PauseMenuCursorConfirm();   // Check for the Confirm Input for the Player's Menu Cursor

            // if both player's have confirmed an action and are on the same option in the menu then execute it
            if (p1ConfirmedHandOption == true && p2ConfirmedHandOption == true && player1MenuIndex == player2MenuIndex && optionPicked == false)
            {
                optionPicked = true;
                //print(menuButtons[player1MenuIndex].gameObject.name);

                p1HandAnimator.SetBool("IsSelected", true);
                p2HandAnimator.SetBool("IsSelected", true);

                //INVOKE THE BUTTON'S FUNCTION
                menuButtons[player1MenuIndex].onConfirm.Invoke();
                menuButtonAnimators[player1MenuIndex].SetBool("IsConfirmed", true);
            }
        }
    }

    public void DisableMenu()
    {
        canPause = false;

        // disable any semi-confirmed menu options
        p1PauseAnimator.SetBool("Close", true);
        p2PauseAnimator.SetBool("Close", true);
        p1PauseAnimator.SetBool("Open", false);
        p2PauseAnimator.SetBool("Open", false);
        p1ConfirmedPauseOption = false;
        p2ConfirmedPauseOption = false;
    }

    public void EnableMenu()
    {
        canPause = true;
    }

    public void CloseMenu()
    {
        //p1Pause.SetActive(false);
        //p2Pause.SetActive(false);

        //pauseMenu.SetActive(false);  // Make the menu invisable

        // Change States after a delay
        waitEnum = WaitForPauseOut(0.25f);
        StartCoroutine(waitEnum);
    }

    //public void CloseMenu(float timeTillClose)
    //{
    //    // Change States after a delay
    //    waitEnum = WaitTime(timeTillClose, PauseState.InGame);
    //    StartCoroutine(waitEnum);
    //}

    void CheckInitalOnePlayerInputs()
    {
        if (p1Movement.playerInput.resetAction.WasPressed || p2Movement.playerInput.resetAction.WasPressed)
        {
            p1PauseAnimator.SetBool("Open", true);
            p1PauseAnimator.SetBool("Close", false);
            p2PauseAnimator.SetBool("Open", true);
            p2PauseAnimator.SetBool("Close", false);

            p1ConfirmedPauseOption = true;
            p2ConfirmedPauseOption = true;
        }
        else if (p1Movement.playerInput.resetAction.WasReleased || p2Movement.playerInput.resetAction.WasReleased)
        {
            p1PauseAnimator.SetBool("Close", true);
            p1PauseAnimator.SetBool("Open", false);
            p2PauseAnimator.SetBool("Close", true);
            p2PauseAnimator.SetBool("Open", false);

            p1ConfirmedPauseOption = false;
            p2ConfirmedPauseOption = false;
        }
    }

    // Create a function that stops the player's functionality
    void StopPlayersFunctionality()
    {
        p1Movement.StopMovementNotCollision();
        p1Throwing.StopThrowing();
        p1Health.StopCatching();
        p1Indicator.StopIndicatorFunctionality();
        p2Movement.StopMovementNotCollision();
        p2Throwing.StopThrowing();
        p2Health.StopCatching();
        p2Indicator.StopIndicatorFunctionality();
    }

    void StartPlayersFunctionality()
    {
        p1Movement.StartMovement();
        p1Throwing.StartThrowing();
        p1Health.StartCatching();
        p1Indicator.StartIndicatorFunctionality();
        p2Movement.StartMovement();
        p2Throwing.StartThrowing();
        p2Health.StartCatching();
        p2Indicator.StartIndicatorFunctionality();
    }

    // This is a function to open the pause menu
    void CheckInitalPauseMenuInputs()
    {
        if (p1Movement.playerInput.resetAction.WasPressed)
        {
            p1PauseAnimator.SetBool("Open", true);
            p1PauseAnimator.SetBool("Close", false);
            p1ConfirmedPauseOption = true;
        }
        else if (p1Movement.playerInput.resetAction.WasReleased)
        {
            p1PauseAnimator.SetBool("Close", true);
            p1PauseAnimator.SetBool("Open", false);
            p1ConfirmedPauseOption = false;
        }

        if (p2Movement.playerInput.resetAction.WasPressed)
        {
            p2PauseAnimator.SetBool("Open", true);
            p2PauseAnimator.SetBool("Close", false);
            p2ConfirmedPauseOption = true;
        }
        else if (p2Movement.playerInput.resetAction.WasReleased)
        {
            p2PauseAnimator.SetBool("Close", true);
            p2PauseAnimator.SetBool("Open", false);
            p2ConfirmedPauseOption = false;
        }
    }

    void PauseMenuCursorMovement()
    {
        // Player 1 MENU MOVEMENT --------------
        if (p1ConfirmedHandOption == false)
        {
            float p1Input = p1Movement.playerInput.verticalAxis.Value;
            int p1RawInput = 0;

            // Get the input from the stick and account for dead zones
            if (Mathf.Abs(p1Input) < 0.35f)
            {
                p1RawInput = 0;
            }
            else
            {
                p1RawInput = (int)Mathf.Sign(p1Input) * 1;
            }

            // if the player is holding the stick make it so that the cursor dosn't move at super speeds
            if (p1RawInput != 0)
            {
                if (p1MenuCursorTime <= 0)
                {
                    player1MenuIndex = (player1MenuIndex + -p1RawInput) % menuButtons.Length;

                    if (Mathf.Sign(player1MenuIndex) == -1)
                    {
                        player1MenuIndex = menuButtons.Length + player1MenuIndex;
                    }

                    // animate buttons
                    menuButtonAnimators[player1MenuIndex].SetBool("IsLeftHover", true);
                    menuButtonAnimators[player2MenuIndex].SetBool("IsRightHover", false);
                    InGameMenuSelectEvent.start();
                    p1MenuCursorTime = maxCursorTime;
                }

                p1MenuCursorTime -= Time.deltaTime;
            }
            else
            {
                p1MenuCursorTime = 0;
            }

            p1Hand.transform.position = new Vector2(p1Hand.transform.position.x, menuButtons[Mathf.Abs(player1MenuIndex)].transform.position.y);
            //print("Player 1 " + player1MenuIndex);
        }


        // Player 2 MENU MOVEMENT --------------
        if (p2ConfirmedHandOption == false)
        {
            float p2Input = p2Movement.playerInput.verticalAxis.Value;
            int p2RawInput = 0;

            // Get the input from the stick and account for dead zones
            if (Mathf.Abs(p2Input) < 0.35f)
            {
                p2RawInput = 0;
            }
            else
            {
                p2RawInput = (int)Mathf.Sign(p2Input) * 1;
            }

            // if the player is holding the stick make it so that the cursor dosn't move at super speeds
            if (p2RawInput != 0)
            {
                if (p2MenuCursorTime <= 0)
                {
                    player2MenuIndex = (player2MenuIndex + -p2RawInput) % menuButtons.Length;

                    //if the index is negative make it equal to the last value in the menu
                    if (Mathf.Sign(player2MenuIndex) == -1)
                    {
                        player2MenuIndex = menuButtons.Length + player2MenuIndex;
                    }

                    // animate buttons
                    menuButtonAnimators[player2MenuIndex].SetBool("IsRightHover", true);
                    menuButtonAnimators[player2MenuIndex].SetBool("IsLeftHover", false);
                    InGameMenuSelectEvent.start();
                    p2MenuCursorTime = maxCursorTime;
                }

                p2MenuCursorTime -= Time.deltaTime;
            }
            else
            {
                p2MenuCursorTime = 0;
            }

            p2Hand.transform.position = new Vector2(p2Hand.transform.position.x, menuButtons[Mathf.Abs(player2MenuIndex)].transform.position.y);
            //print("Player 2 " + player2MenuIndex);
        }
    }

    void PauseMenuOnePlayerCursorMovement()
    {
        // Player 1 MENU MOVEMENT --------------
        if (p1ConfirmedHandOption == false)
        {
            float p1Input = p1Movement.playerInput.verticalAxis.Value;
            float p2Input = p2Movement.playerInput.verticalAxis.Value;
            int p1RawInput = 0;
            int p2RawInput = 0;

            // Get the input from the stick and account for dead zones
            if (Mathf.Abs(p1Input) < 0.35f)
            {
                p1RawInput = 0;
            }
            else
            {
                p1RawInput = (int)Mathf.Sign(p1Input) * 1;
            }

            if (Mathf.Abs(p2Input) < 0.35f)
            {
                p2RawInput = 0;
            }
            else
            {
                p2RawInput = (int)Mathf.Sign(p2Input) * 1;
            }

            if (p1RawInput == 0 && p2RawInput != 0)
            {
                p1RawInput = p2RawInput;
            }

            // if the player is holding the stick make it so that the cursor dosn't move at super speeds
            if (p1RawInput != 0)
            {
                if (p1MenuCursorTime <= 0)
                {
                    player1MenuIndex = (player1MenuIndex + -p1RawInput) % menuButtons.Length;
                    player2MenuIndex = (player2MenuIndex + -p1RawInput) % menuButtons.Length;

                    if (Mathf.Sign(player1MenuIndex) == -1)
                    {
                        player1MenuIndex = menuButtons.Length + player1MenuIndex;
                        player2MenuIndex = menuButtons.Length + player2MenuIndex;
                    }

                    // animate buttons
                    menuButtonAnimators[player1MenuIndex].SetTrigger("BothHover");
                    InGameMenuSelectEvent.start();
                    p1MenuCursorTime = maxCursorTime;
                }

                p1MenuCursorTime -= Time.deltaTime;
            }
            else
            {
                p1MenuCursorTime = 0;
            }

            p1Hand.transform.position = new Vector2(p1Hand.transform.position.x, menuButtons[Mathf.Abs(player1MenuIndex)].transform.position.y);
            p2Hand.transform.position = new Vector2(p2Hand.transform.position.x, menuButtons[Mathf.Abs(player2MenuIndex)].transform.position.y);
            //print("Player 1 " + player1MenuIndex);
        }
    }

    void PauseMenuCursorConfirm()
    {
        if (optionPicked == false)
        {
            // if the player presses A then confirm their cursor and prevent them from moveing
            // FOR PLAYER 1 ------------------------------
            switch (p1CurrentClickState)
            {
                case (MenuClickState.UnConfirmed):
                    if ((p1Movement.playerInput.jumpAction.WasPressed || p1Movement.playerInput.confirmAction.WasPressed) && p1ConfirmedHandOption == false)
                    {
                        //confirming
                        p1ConfirmedHandOption = true;
                        p1HandAnimator.SetBool("IsSelected", true);

                        p1CurrentClickState = MenuClickState.Confirmed;

                        menuButtonAnimators[player1MenuIndex].SetTrigger("Select");
                        Player1ConfirmEvent.start();
                    }
                    break;
                case (MenuClickState.Confirmed):
                    if ((p1Movement.playerInput.jumpAction.WasPressed || p1Movement.playerInput.confirmAction.WasPressed) && p1ConfirmedHandOption == true)
                    {
                        //deconfirming
                        p1ConfirmedHandOption = false;
                        p1HandAnimator.SetBool("IsSelected", false);

                        p1CurrentClickState = MenuClickState.UnConfirmed;

                        menuButtonAnimators[player1MenuIndex].SetTrigger("Unselect");
                        menuButtonAnimators[player1MenuIndex].SetBool("IsLeftHover", false);
                        menuButtonAnimators[player2MenuIndex].SetBool("IsRightHover", false);
                    }
                    break;
            }

            // FOR PLAYER 2 ------------------------------
            switch (p2CurrentClickState)
            {
                case (MenuClickState.UnConfirmed):
                    if ((p2Movement.playerInput.jumpAction.WasPressed || p2Movement.playerInput.confirmAction.WasPressed) && p2ConfirmedHandOption == false)
                    {
                        //confirming
                        p2ConfirmedHandOption = true;
                        p2HandAnimator.SetBool("IsSelected", true);

                        p2CurrentClickState = MenuClickState.Confirmed;

                        menuButtonAnimators[player2MenuIndex].SetTrigger("Select");
                        Player2ConfirmEvent.start();
                    }
                    break;
                case (MenuClickState.Confirmed):
                    if ((p2Movement.playerInput.jumpAction.WasPressed || p2Movement.playerInput.confirmAction.WasPressed) && p2ConfirmedHandOption == true)
                    {
                        //deconfirming
                        p2ConfirmedHandOption = false;
                        p2HandAnimator.SetBool("IsSelected", false);

                        p2CurrentClickState = MenuClickState.UnConfirmed;

                        menuButtonAnimators[player2MenuIndex].SetTrigger("Unselect");
                    }
                    break;
            }
        }
    }

    void PauseMenuOnePlayerCursorConfirm()
    {
        if (optionPicked == false)
        {
            // if the player presses A then confirm their cursor and prevent them from moveing
            // FOR PLAYER 1 ------------------------------
            switch (p1CurrentClickState)
            {
                case (MenuClickState.UnConfirmed):
                    if (((p1Movement.playerInput.jumpAction.WasPressed || p1Movement.playerInput.confirmAction.WasPressed) && p1ConfirmedHandOption == false) ||
                        ((p2Movement.playerInput.jumpAction.WasPressed || p2Movement.playerInput.confirmAction.WasPressed) && p2ConfirmedHandOption == false))
                    {
                        p1ConfirmedHandOption = true;
                        p1HandAnimator.SetBool("IsSelected", true);
                        p2ConfirmedHandOption = true;
                        p2HandAnimator.SetBool("IsSelected", true);

                        //menuButtonAnimators[player1MenuIndex].SetTrigger("Select");

                        p1CurrentClickState = MenuClickState.Confirmed;
                        p2CurrentClickState = MenuClickState.Confirmed;
                        Player1ConfirmEvent.start();
                    }
                    break;
                case (MenuClickState.Confirmed):
                    if (((p1Movement.playerInput.jumpAction.WasPressed || p1Movement.playerInput.confirmAction.WasPressed) && p1ConfirmedHandOption == true) ||
                        ((p2Movement.playerInput.jumpAction.WasPressed || p2Movement.playerInput.confirmAction.WasPressed) && p2ConfirmedHandOption == true))
                    {
                        p1ConfirmedHandOption = false;
                        p1HandAnimator.SetBool("IsSelected", false);
                        p2ConfirmedHandOption = false;
                        p2HandAnimator.SetBool("IsSelected", false);

                        //menuButtonAnimators[player1MenuIndex].SetTrigger("UnSelect");

                        p1CurrentClickState = MenuClickState.UnConfirmed;
                        p2CurrentClickState = MenuClickState.UnConfirmed;
                    }
                    break;
            }
        }
    }

    public void GoodbyeWave()
    {
        p1HandAnimator.SetTrigger("Goodbye");
        p2HandAnimator.SetTrigger("Goodbye");
    }

    public void SetHandStartingPositions()
    {
        p1RectTransform.position = new Vector2(p1RectTransform.position.x, startingButtonPosition.position.y);
        p2RectTransform.position = new Vector2(p2RectTransform.position.x, startingButtonPosition.position.y);
    }

    IEnumerator WaitForPauseOut(float waitTime)
    {
        currentState = PauseState.WaitForPauseOut;

        yield return new WaitForSeconds(waitTime);

        pauseMenuAnimator.enabled = true;
        pauseMenuAnimator.SetBool("IsActive", false);
        closeMenuEvent.start();

    }

    IEnumerator WaitForPauseIn(float waitTime)
    {
        currentState = PauseState.WaitForPauseIn;

        //// reset the hand positions
        //p1Hand.transform.position = new Vector2(p1Hand.transform.position.x, menuButtons[0].transform.localPosition.y);
        //p2Hand.transform.position = new Vector2(p2Hand.transform.position.x, menuButtons[0].transform.localPosition.y);

        yield return new WaitForSeconds(waitTime);

        p1PauseAnimator.SetBool("Close", true);
        p2PauseAnimator.SetBool("Close", true);
        p1PauseAnimator.SetBool("Open", false);
        p2PauseAnimator.SetBool("Open", false);

        pauseMenu.SetActive(true);  // Make the menu visable
        pauseMenuAnimator.SetBool("IsActive", true);
        OpenMenuEvent.start();
        callOnce = true;

    }

    IEnumerator WaitTime(float waitAmount)
    {
        //print("AKSJFHKJASHKJSAHFJKA");
        callOnce = true;

        // Reset all the values for the menu before going back to in game
        p1MenuCursorTime = 0;
        p2MenuCursorTime = 0;
        player1MenuIndex = 0;
        player2MenuIndex = 0;
        p1ConfirmedHandOption = false;
        p2ConfirmedHandOption = false;
        optionPicked = false;
        p1CurrentClickState = MenuClickState.UnConfirmed;
        p2CurrentClickState = MenuClickState.UnConfirmed;
        p1HandAnimator.SetBool("IsSelected", false);
        p2HandAnimator.SetBool("IsSelected", false);
        pauseMenu.SetActive(false);
        // MAKE PLAYER'S ABLE TO MOVE AGAIN ------------------------------------
        StartPlayersFunctionality();

        yield return new WaitForSeconds(waitAmount);

        currentState = PauseState.InGame;
    }
}
//TODO
/* ! Lock Player Movement
 * ! Fix bug with highlight image being able to be pressed while paused
 * ! Add functionality to the buttons
 * ! Move the hand cursors
 * ! Have them change image when confirmed
 * ! have an ability to back out of selected menu option
 * ! Delay menu appearing or delay pause button fade
 * ! fix bug of not being able to move cursors after loading the menu twice
 * ! fix menu not disappearing on reset
 * ! Fix bug where the menu cursor dosn't change images when selecting an option
 *      - I think this is caused by an over input error need to create a timer to stop multiple inputs
 */
