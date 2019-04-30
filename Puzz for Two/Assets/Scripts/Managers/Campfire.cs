using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using InControl;
using UnityEngine.SceneManagement;

public class Campfire : MonoBehaviour
{
    //public static Campfire instance;

    public enum CampfireState
    {
        AtCampfire,
        MovingToDisbandSpot,
        Disbanding,
        MovingToSittingSpots,
        AtSittingSpots,
        AtSpotsWaitingForInput,
        WaitTime
    }
    [Header("Basic Variables")]
    public CampfireState currentState = CampfireState.AtCampfire;

    public GameObject buttonPromptText;
    public GameObject playerPieceCountText;
    public TextMesh timeText;
    public Transform[] startingSpots = new Transform[2];

    MovingCamera movingCameraScript;

    [Header("Variables Gotten Through Play")]
    public List<GameObject> players = new List<GameObject>();
    public List<Transform> sittingSpots = new List<Transform>();
    public List<GameObject> detachedPlayerPieces = new List<GameObject>();
    List<Movement> playerMovementScripts = new List<Movement>();
    List<PlayerThrowing> playerThrowingScripts = new List<PlayerThrowing>();
    List<PlayerHealth> playerHealthScripts = new List<PlayerHealth>();
    List<DetachedPiece> detactchedPlayerPieceScripts = new List<DetachedPiece>();
    [HideInInspector] public List<PlayerIndicator> playerIndicatorScripts = new List<PlayerIndicator>();

    [Header("Unity Events")]
    public UnityEvent onSittingAtCampfire;
    public UnityEvent onAtSpotsWaitingForInput;
    public UnityEvent onWaitingInputDown;

    bool callOnce = false;
    bool campfireFinished = false;
    Vector3[] startingPos;
    IEnumerator waitEnum;

    // Variables for the catch all squence to proceed without all of the player pieces on the log
    float waitTimeTillForcedProceed = 2.5f;
    int allBoys, caughtBoys;
    public bool cigFound,lipstickFound;
    List<Movement> playersWithCampers = new List<Movement>();
    List<Movement> playersWithoutCampers = new List<Movement>();

    // Create FMOD Sound Effect Variables
    [Header("FMOD Audio Events")]
    [FMODUnity.EventRef]
    public string confirmLevelSound, levelInteractionSound;
    FMOD.Studio.EventInstance fModLevelCompleteEvent, fModLevelInteractionEvent;
    [SerializeField] Color goldColor;
    //[SerializeField] string nextLevelName;

    InteractiveMenu interactiveMenuSingleton;   // Get access to the menu to disable it when players interact with the campfire
    NewControllerManager controllerManagerInstance;

    public FMODUnity.StudioEventEmitter musicEmitter, trailMixWistle;
    bool allCampersCollected = false;

    // Use this for initialization
    void Start()
    {
        // Get all of the sitting spots in the 
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "SittingSpot")
            {
                sittingSpots.Add(child);
            }

            // Randomize the sitting spots order
            for (int i = 0; i < sittingSpots.Count; i++)
            {
                Transform temp = sittingSpots[i];
                int randomeIndex = Random.Range(i, sittingSpots.Count);
                sittingSpots[i] = sittingSpots[randomeIndex];
                sittingSpots[randomeIndex] = temp;
            }
        }

        // Set up the other objects in the scene
        if (buttonPromptText.activeSelf == true)
        {
            buttonPromptText.SetActive(false);
        }

        movingCameraScript = Camera.main.GetComponent<MovingCamera>();

        // Get a referecne to the pause menu so that it can be disabled when players try to laod a level
        if (InteractiveMenu.instance != null)
        {
            interactiveMenuSingleton = InteractiveMenu.instance;
        }

        controllerManagerInstance = NewControllerManager.instance;

        // Create the Audio Events
        fModLevelCompleteEvent = FMODUnity.RuntimeManager.CreateInstance(confirmLevelSound);
        fModLevelInteractionEvent = FMODUnity.RuntimeManager.CreateInstance(levelInteractionSound);

        foreach (Transform child in Camera.main.transform)
        {
            if (child.name == "Music")
            {
                musicEmitter = child.GetComponent<FMODUnity.StudioEventEmitter>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case (CampfireState.AtCampfire):
                AtCampfire();
                break;
            case (CampfireState.MovingToDisbandSpot):
                MovingToDisbandSpot();
                break;
            case (CampfireState.Disbanding):
                Disbanding();
                break;
            case (CampfireState.MovingToSittingSpots):
                MovingToSittingSpots();
                break;
            case (CampfireState.AtSittingSpots):
                AtSittingSpots();
                break;
            case (CampfireState.AtSpotsWaitingForInput):
                AtSpotsWaitingForInput();
                break;
            case (CampfireState.WaitTime):
                // Do Nothing
                break;
        }
        // if the players are within the campfire bounds then make text appear
        // on button press 
        // move the players to the correct spot
        // then when they are all there --> dispand them

        // disband all of the boys
        // Give them a spot to move to
        // Make them move to that spot --> and jump if need be
    }

    #region Switch Statement Functions
    void AtCampfire()
    {
        if (players.Count == 2)
        {
            if (playerMovementScripts.Count <= 0)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    Movement move = players[j].GetComponent<Movement>();
                    playerMovementScripts.Add(move);
                    PlayerThrowing playerThrow = players[j].GetComponent<PlayerThrowing>();
                    playerThrowingScripts.Add(playerThrow);
                    PlayerHealth health = players[j].GetComponent<PlayerHealth>();
                    playerHealthScripts.Add(health);
                }
            }

            if (playerIndicatorScripts.Count == 2)
            {
                if (controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.OnePlayerKeyboard 
                    || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.OnePlayerController
                    || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.Undecided)
                {
                    if (playerIndicatorScripts[0].playerIsHoldingDownButton || playerIndicatorScripts[1].playerIsHoldingDownButton)
                    {
                        CalculateNumberOfCollectedPieces(); // Set the number of collected player pieces
                        OnAtCampfire();
                    }
                }
                else if (controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard ||
                    controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController ||
                    controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
                {
                    if (playerIndicatorScripts[0].playerIsHoldingDownButton && playerIndicatorScripts[1].playerIsHoldingDownButton)
                    {
                        CalculateNumberOfCollectedPieces(); // Set the number of collected player pieces
                        OnAtCampfire();
                    }
                }
            }
        }
    }
    void SaveLevel()
    {
        //save the stats from this level
        if (SaveManager.instance)
        {
            SaveManager.instance.FinishLevel(caughtBoys, allBoys, cigFound, lipstickFound);
            float time = SaveManager.instance.GetCurrentLevel().latestTime;
            if (time == SaveManager.instance.GetCurrentLevel().fastestTime)
            {
                //if its the fastest time so far make it golden
                timeText.color = goldColor;
            }
            timeText.text = string.Format("{0}:{1:00}:{2:00}", (int)time / 3600, ((int)time / 60) % 60, (int)time % 60);
            SaveManager.instance.BackToLevelSelect();
        }
        else
        {
            //if theres no save boi, dont show the timer text
            timeText.gameObject.SetActive(false);
        }
    }
    void OnAtCampfire()
    {
        SaveLevel();

        //onAtCampfire.Invoke();
        movingCameraScript.RemoveAllTargets();
        movingCameraScript.AddTarget(this.gameObject);

        // Stop all player movment
        for (int i = 0; i < players.Count; i++)
        {
            playerMovementScripts[i].StopMovement();
            playerThrowingScripts[i].StopThrowing();
            playerHealthScripts[i].StopCatching();
            playerIndicatorScripts[i].StopIndicatorFunctionality();
        }

        buttonPromptText.SetActive(false);

        // Disable the player speach bubbles
        for (int l = 0; l < playerIndicatorScripts.Count; l++)
        {
            playerIndicatorScripts[l].DisableImage();
            playerIndicatorScripts[l].StopIndicatorFunctionality();
        }

        // prevent players from accessing the menu
        if (interactiveMenuSingleton != null)
        {
            interactiveMenuSingleton.DisableMenu();
        }

        CalculateNumberOfPlayersWithCampers();  // Calculate what players have campers

        //currentState = CampfireState.MovingToDisbandSpot;
        waitEnum = WaitTime(0.1f, CampfireState.MovingToDisbandSpot);
        StartCoroutine(waitEnum);
    }

    // Move to the spots that the players will disband all the players pieces
    void MovingToDisbandSpot()
    {
        // set the players with pieces to move the the starting spot
        // players without will then move to their sitting spot
        if (callOnce == false)
        {
            for (int i = 0; i < playersWithCampers.Count; i++)
            {
                playersWithCampers[i].SetAutomatedMovement();
                playersWithCampers[i].SetMovementDestination(startingSpots[i]);
            }

            for (int i = 0; i < playersWithoutCampers.Count; i++)
            {
                playersWithoutCampers[i].SetAutomatedMovement();
                playersWithoutCampers[i].SetMovementDestination(PickSittingSpot());
            }

            callOnce = true;
        }

        // Check if all the players are at there correct spots
        for (int i = 0; i < playerMovementScripts.Count; i++)
        {
            if (playerMovementScripts[i].reachedAutoXPos == false)
            {
                break;
            }
            else
            {
                callOnce = false;
                waitEnum = WaitTime(0.25f, CampfireState.Disbanding);
                StartCoroutine(waitEnum);
            }
        }
        // set a destination for each of the players

        // figure out when all of them reach their destination
        // when all of them reach their destination then move on to disbanding
    }

    void Disbanding()
    {
        if (callOnce == false)
        {
            for (int i = 0; i < playerThrowingScripts.Count; i++)
            {
                // get the boys and launch them
                List<GameObject> thrownPieces = new List<GameObject>();
                thrownPieces = playerThrowingScripts[i].ThrowToSpecificSpot(sittingSpots);
                detachedPlayerPieces.AddRange(thrownPieces);
            }

            for (int k = 0; k < detachedPlayerPieces.Count; k++)
            {
                detactchedPlayerPieceScripts.Add(detachedPlayerPieces[k].GetComponent<DetachedPiece>());    // Add the detatched player script to a list
            }

            callOnce = true;
        }

        waitEnum = WaitTime(0.25f, CampfireState.MovingToSittingSpots);
        StartCoroutine(waitEnum);
        callOnce = false;
    }

    void MovingToSittingSpots()
    {
        if (callOnce == false)
        {
            for (int i = 0; i < playersWithCampers.Count; i++)
            {
                playersWithCampers[i].SetAutomatedMovement();
                playersWithCampers[i].SetMovementDestination(PickSittingSpot());
            }

            callOnce = true;
        }

        // Check if all the gameobjects are at their correct spots
        for (int i = 0; i < playerMovementScripts.Count; i++)
        {
            if (playerMovementScripts[i].reachedAutomatedDestination == false)
            {
                break;
            }
            else
            {
                callOnce = false;
                waitEnum = WaitTime(1f, CampfireState.AtSittingSpots);
                StartCoroutine(waitEnum);
            }
        }
    }

    void AtSittingSpots()
    {
        if (callOnce == false)
        {
            onSittingAtCampfire.Invoke();
            callOnce = true;

            // play the level completion sound
            fModLevelCompleteEvent.start();

            if (allCampersCollected == true)    // play music if all campers are collected
            {
                trailMixWistle.Play();
            }
        }

        // Call this if the player's did collect a player piece
        if (detachedPlayerPieces.Count > 0)
        {
            for (int i = 0; i < detachedPlayerPieces.Count; i++)
            {
                if (detactchedPlayerPieceScripts[i].foundStoppingPos == false)
                {
                    break;
                }
                else
                {
                    waitEnum = WaitTime(1.5f, CampfireState.AtSpotsWaitingForInput);
                    StartCoroutine(waitEnum);
                    callOnce = false;
                }
            }

            // start a counter that calls the next part of the capfire script if not all the players pieces reach their destination
            waitTimeTillForcedProceed -= Time.deltaTime;
            if (waitTimeTillForcedProceed <= 0)
            {
                print("NOT ALL BOYS REACHED SITTING SPOTS, PROCEEDING ANYWAY...");
                waitEnum = WaitTime(1.5f, CampfireState.AtSpotsWaitingForInput);
                StartCoroutine(waitEnum);
                callOnce = false;
                waitTimeTillForcedProceed = 2.5f;
            }
        }
        // Call this if the player's did not collect any player pieces
        else
        {
            waitEnum = WaitTime(1.75f, CampfireState.AtSpotsWaitingForInput);
            StartCoroutine(waitEnum);
            callOnce = false;
        }
    }

    void AtSpotsWaitingForInput()
    {
        if (callOnce == false)
        {
            //print("All boys at campfire!");
            onAtSpotsWaitingForInput.Invoke();
            callOnce = true;

            for (int i = 0; i < playerIndicatorScripts.Count; i++)
            {
                playerIndicatorScripts[i].ActivateImage();
                playerIndicatorScripts[i].StartIndicatorFunctionality();
            }
        }

        if (controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.OnePlayerKeyboard 
            || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.OnePlayerController
            || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.Undecided)
        {
            if (playerMovementScripts[0].playerInput.confirmAction.IsPressed || playerMovementScripts[1].playerInput.confirmAction.IsPressed)
            {
                // Then move onto the next scene
                if (campfireFinished == false)
                {
                    OnAtSpotsWaitingForInput();
                }
            }
        }
        else if (controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard 
            || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController
            || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
        {
            if (playerMovementScripts[0].playerInput.confirmAction.IsPressed && playerMovementScripts[1].playerInput.confirmAction.IsPressed)
            {
                // Then move onto the next scene
                if (campfireFinished == false)
                {
                    OnAtSpotsWaitingForInput();
                }
            }
        }
    }

    void OnAtSpotsWaitingForInput()
    {
        campfireFinished = true;
        onWaitingInputDown.Invoke();

        for (int l = 0; l < playerIndicatorScripts.Count; l++)
        {
            playerIndicatorScripts[l].DisableImage();
            playerIndicatorScripts[l].StopIndicatorFunctionality();

        }

        //play sound for level confirm completion
        fModLevelCompleteEvent.start();
        
    }

    #endregion

    // Create a function that returns a sitting spot form the list and deletes it
    public Transform PickSittingSpot()
    {
        Transform spot;

        spot = sittingSpots[sittingSpots.Count - 1];
        sittingSpots.RemoveAt(sittingSpots.Count - 1);

        return spot;
    }

    void CalculateNumberOfPlayersWithCampers()
    {
        playersWithCampers.Clear();
        playersWithoutCampers.Clear();

        // get the player health from the movment script
        for (int i = 0; i < playerHealthScripts.Count; i++)
        {
            if (playerHealthScripts[i].health > 1)
            {
                playersWithCampers.Add(playerMovementScripts[i]);
            }
            else
            {
                playersWithoutCampers.Add(playerMovementScripts[i]);
            }
        }
    }

    void CalculateNumberOfCollectedPieces()
    {
        int totalPlayerPieces = 0;

        // get the player health from the movment script
        for (int i = 0; i < playerHealthScripts.Count; i++)
        {
            totalPlayerPieces += (int)playerHealthScripts[i].health;
        }

        totalPlayerPieces = totalPlayerPieces - 2;  // Subract two to account for the player characters
        //these are for the save manager
        caughtBoys = totalPlayerPieces;

        TotalPlayerPieces pieceCounter = TotalPlayerPieces.instance;
        allBoys = pieceCounter.totalPlayerPieces - 2;
        TextMesh boyCountText = playerPieceCountText.GetComponent<TextMesh>();

        if (totalPlayerPieces < pieceCounter.totalPlayerPieces - 2 && totalPlayerPieces != 1)   // for > 1 camper
        {
            boyCountText.text = "You Collected " + totalPlayerPieces + " Campers!";
        }
        else if (totalPlayerPieces < pieceCounter.totalPlayerPieces - 2 && totalPlayerPieces == 1)  // for 1 camper
        {
            boyCountText.text = "You Collected " + totalPlayerPieces + " Camper!";
        }
        else if (totalPlayerPieces >= pieceCounter.totalPlayerPieces - 2)       // for all campers collected
        {
            boyCountText.text = "You Collected all the Campers!";

            // set the music to the wistle theme
            if (musicEmitter != null)
            {
                musicEmitter.Stop();
            }
            allCampersCollected = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && currentState == CampfireState.AtCampfire)
        {
            //print("player is in");

            GameObject enteredPlayer = collision.gameObject.transform.parent.root.gameObject;

            // check if the player that entered te fire is the same as one that just entered
            if (players.Contains(enteredPlayer) == false)
            {
                players.Add(enteredPlayer);

                PlayerIndicator indicator = enteredPlayer.GetComponentInChildren<PlayerIndicator>();
                playerIndicatorScripts.Add(indicator);
                indicator.ActivateImage();  // Make the indicators on the player visible when they enter the campfire range
            }

            // Check if both players are in the campfire
            if (players.Count >= 2)
            {
                buttonPromptText.SetActive(true); // Activate the text when both players are present 
                fModLevelInteractionEvent.start();

                for (int i = 0; i < playerIndicatorScripts.Count; i++)
                {
                    playerIndicatorScripts[i].ChangeImage();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && currentState == CampfireState.AtCampfire)
        {
            //print("player is out");
            GameObject exitedPlayer = collision.gameObject.transform.parent.root.gameObject;

            if (players.Contains(exitedPlayer) == true)
            {
                int exitIndex = players.IndexOf(exitedPlayer);

                playerIndicatorScripts[exitIndex].RevertImage();
                playerIndicatorScripts[exitIndex].DisableImage();   // make the campfire indicators disappear when a player leaves the campfire

                players.RemoveAt(exitIndex);
                playerIndicatorScripts.RemoveAt(exitIndex);
            }

            if (players.Count < 2)
            {
                buttonPromptText.SetActive(false);

                for (int i = 0; i < playerIndicatorScripts.Count; i++)
                {
                    playerIndicatorScripts[i].RevertImage();
                }
            }
        }
    }

    IEnumerator WaitTime(float waitAmount, CampfireState nextState)
    {
        currentState = CampfireState.WaitTime;
        yield return new WaitForSeconds(waitAmount);
        currentState = nextState;

    }
    //void LoadNextScene()
    //{
    //    SceneManager.LoadScene(nextLevelName);
    //}

    public void LoadFader(float time)
    {
        Fader.instance.FadeToColor(Color.black, time);
    }
}

//TODO
// Add the animations
// ! Code next button press to continue/number of boys rescued
// ! Display the number of saved bois+players in a graphics
// ! Make the detatched player pieces stop when they reach their spot
// ?? Make loose bois within the campfire auto throw to their spots
// -----------------------------------
// Make it so that a camperless player will go directally to the spot at log
// shoot campers at an arc utalizing empty gameobjects and animations curves
// code a random change thing that makes it so that ramdomly the campers will miss the log
