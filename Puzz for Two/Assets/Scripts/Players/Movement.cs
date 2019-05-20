using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.Tilemaps;


public class Movement : MonoBehaviour
{
    [SerializeField] SpeedProfile PlayerSettings;
    [SerializeField] InputProfile inputSettings;
    public GroundMaterialProfile groundMatSettings;
    [HideInInspector] public MyCharacterActions playerInput;
    [SerializeField] string gridParentName;
    NewControllerManager controllerManagerInstance;

    [Tooltip("This should be 0 for player 1 and 1 for player 2")]
    public int playerNumber; //this is all thats switched by the debug player switcher now

    public List<PlayerPiece> pieces;
    [HideInInspector] public List<Collider2D> pieceColliders;
    [HideInInspector] public List<SpriteRenderer> pieceSpriteRenderers;
    float xSpeedGrounded, xSpeedAerial, jumpHeight, riseTime, hangTime, fallAccel, maxFallSpeed, shortHopDeAccel, maxHorizontalJump, walkOffLedgeJumpWindow;
    float startingV, risingGravity, yVelocity;
    float shortHopWindow;
    float timeJumping = 0;
    float timeUngrounded = 9999;
    float xPointJumpedFrom = 0;
    float xJumpSlowingPoint = 0;
    float slowingPointPosition = 0;
    [Header("Input")]
    public string hAxis, vAxis, jumpButton, lockHorizontal, resetButton;
    public bool grounded = false, rising = false;
    bool castevaniaJumps, shortHops, pushing, jumpHorizontalLimit, cantJumpWithRoof, confinedSpaceAssist, jumpAfterWalkOffLedge;
    public Vector2 movementInput, throwInput;
    public Vector2 movementVelocity;
    Rigidbody2D rigidBodyComp;
    [Header("squishing variables")]
    [SerializeField] float minimumScale;
    float bonkTimer;
    [SerializeField] bool squarsh;
    [SerializeField] bool footSounds, particlesEnabled;
    static bool horizontalAdjust = true, verticalAdjust = true;
    Vector3 defaultScale;
    float stickToRoofTime;
    bool bonkedPrevFrame;
    int cornerSnagCheck = 0;
    int horizontalSnagCheck = 0;
    GameObject particlesToSpawn;

    // Stop Movement Variables
    public bool movementIsStopped = false;
    Transform targetAutoMoveDestination;
    [HideInInspector]
    public enum MovementTypes
    {
        PlayerMovement,
        AutomatedMovement
    }
    MovementTypes currentMovementType = MovementTypes.PlayerMovement;
    public bool reachedAutomatedDestination = false;
    //bool stopGravity = false;
    public bool reachedAutoXPos = false, reachedAutoYPos = false;

    // Create FMOD Sound Effect Variables
    [Header("FMOD Audio Events")]
    [FMODUnity.EventRef]
    public string jumpsound, stepSound, impactSound;
    FMOD.Studio.EventInstance fModJumpEvent, fModStepEvent, fmodImpactEvent;
    float stepTime = 1.5f;
    float originalStepTime;

    public reverbType reverbZone;

    public static int sortPeak;
    //this is just for some niche checks
    public Movement otherPlayer;
    PlayerThrowing throwComp;

    public List<Tilemap> allTileMapsInScene = new List<Tilemap>(); //for layered "collisions"

    float particleTiltMult = 2f;
    public bool triedControls;
    GameObject pivotParticles;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "AudioHouse")
        {
            reverbZone = reverbType.house;
            fModStepEvent.setParameterValue("In House", 1);
            fModStepEvent.setParameterValue("In Cave", 0);
            fModStepEvent.setParameterValue("Outside", 0);

            fmodImpactEvent.setParameterValue("In House", 1);
            fmodImpactEvent.setParameterValue("In Cave", 0);
            fmodImpactEvent.setParameterValue("Outside", 0);

            throwComp.fModThrowPieceEvent.setParameterValue("In House", 1);
            throwComp.fModThrowPieceEvent.setParameterValue("In Cave", 0);
            throwComp.fModThrowPieceEvent.setParameterValue("Outside", 0);

        }
        else if (collision.tag == "AudioCave")
        {
            reverbZone = reverbType.cave;
            fModStepEvent.setParameterValue("In House", 0);
            fModStepEvent.setParameterValue("In Cave", 1);
            fModStepEvent.setParameterValue("Outside", 0);


            fmodImpactEvent.setParameterValue("In House", 0);
            fmodImpactEvent.setParameterValue("In Cave", 1);
            fmodImpactEvent.setParameterValue("Outside", 0);

            throwComp.fModThrowPieceEvent.setParameterValue("In House", 0);
            throwComp.fModThrowPieceEvent.setParameterValue("In Cave", 1);
            throwComp.fModThrowPieceEvent.setParameterValue("Outside", 0);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<InteriorTrigger>())
        {
            reverbZone = reverbType.outside;
            fModStepEvent.setParameterValue("In House", 0);
            fModStepEvent.setParameterValue("In Cave", 0);
            fModStepEvent.setParameterValue("Outside", 1);

            fmodImpactEvent.setParameterValue("In House", 0);
            fmodImpactEvent.setParameterValue("In Cave", 0);
            fmodImpactEvent.setParameterValue("Outside", 1);

            throwComp.fModThrowPieceEvent.setParameterValue("In House", 0);
            throwComp.fModThrowPieceEvent.setParameterValue("In Cave", 0);
            throwComp.fModThrowPieceEvent.setParameterValue("Outside", 1);
        }
    }
    void Awake()
    {
        
        xSpeedGrounded = PlayerSettings.speedXGrounded;
        xSpeedAerial = PlayerSettings.speedXAerial;
        jumpHeight = PlayerSettings.jumpHeight;
        riseTime = PlayerSettings.riseTime;
        hangTime = PlayerSettings.hangTime;
        fallAccel = PlayerSettings.fallAccel;
        maxFallSpeed = PlayerSettings.maxFallSpeed;
        shortHopDeAccel = PlayerSettings.shortHopDeAccel;
        maxHorizontalJump = PlayerSettings.maxHorizontalJump;
        walkOffLedgeJumpWindow = PlayerSettings.walkOffLedgeJumpWindow;
        xJumpSlowingPoint = PlayerSettings.xJumpSlowingPoint;
        castevaniaJumps = PlayerSettings.castlevaniaJumps;
        cantJumpWithRoof = PlayerSettings.cantJumpWithRoof;
        confinedSpaceAssist = PlayerSettings.confinedSpaceAssist;
        jumpAfterWalkOffLedge = PlayerSettings.jumpAfterWalkOffLedge;
        jumpHorizontalLimit = PlayerSettings.jumpHorizontalLimit;
        stickToRoofTime = PlayerSettings.bonkAssist;
        shortHops = PlayerSettings.shortHops;
        pushing = PlayerSettings.pushing;
        pieces.AddRange(GetComponentsInChildren<PlayerPiece>());
        rigidBodyComp = GetComponent<Rigidbody2D>();
        throwComp = GetComponent<PlayerThrowing>();
        xPointJumpedFrom = transform.position.x;
        slowingPointPosition = maxHorizontalJump * xJumpSlowingPoint;

        //Math to calculate jump accel and initial v
        risingGravity = -((2 * jumpHeight) / (riseTime * riseTime));
        startingV = (jumpHeight - risingGravity * (riseTime * riseTime) / 2) / riseTime;

        foreach (PlayerPiece eachPiece in pieces)
        {
            pieceColliders.Add(eachPiece.GetComponentInChildren<Collider2D>());
        }

        for (int i = 0; i < pieces.Count; i++)
        {
            pieceSpriteRenderers.Add(pieces[i].gameObject.GetComponentInChildren<SpriteRenderer>());
        }

        defaultScale = pieces[0].transform.localScale;


        playerInput = new MyCharacterActions();

        otherPlayer = GameObject.Find("Player " + ((playerNumber + 1) % 2 + 1)).GetComponent<Movement>();

    }

    private void Start()
    {
        controllerManagerInstance = NewControllerManager.instance;
        InputManager.OnDeviceDetached += OnControllerDettached;
        InputManager.OnDeviceAttached += OnControllerAttached;
        CheckToSaveContrllerType();

        //input setup
        if (playerNumber == 0)
        {
            controllerManagerInstance.player1Movement = this;
        }
        else
        {
            controllerManagerInstance.player2Movement = this;
        }

        //instanciate FMOD sound variables/events
        fModJumpEvent = FMODUnity.RuntimeManager.CreateInstance(jumpsound);
        fModStepEvent = FMODUnity.RuntimeManager.CreateInstance(stepSound);
        fmodImpactEvent = FMODUnity.RuntimeManager.CreateInstance(impactSound);
        originalStepTime = stepTime;
        stepTime = 0.01f;

        fModStepEvent.setParameterValue("Outside", 1); //outside by default


        allTileMapsInScene.AddRange(GameObject.Find(gridParentName).GetComponentsInChildren<Tilemap>());
        //sort by sorting layer:
        allTileMapsInScene.Sort(SortBySortingOrder);
        allTileMapsInScene.Reverse();

        foreach(SpriteRenderer eachSprite in pieceSpriteRenderers)
        {
            eachSprite.sortingOrder = sortPeak-1;
            eachSprite.GetComponentInChildren<LookAtObject>().GetComponent<SpriteRenderer>().sortingOrder = sortPeak;
            sortPeak-=2;
        }
    }
    int SortBySortingOrder(Tilemap t1, Tilemap t2)
    {
        TilemapRenderer tr1 = t1.GetComponent<TilemapRenderer>(), tr2 = t2.GetComponent<TilemapRenderer>();
        if (tr1.sortingLayerID == tr2.sortingLayerID)
        {
            return tr1.sortingOrder.CompareTo(tr2.sortingOrder);
        }
        else
        {

            return SortingLayer.GetLayerValueFromID(tr1.sortingLayerID).CompareTo(SortingLayer.GetLayerValueFromID(tr2.sortingLayerID));
        }
    }
    private void OnDisable()
    {
        InputManager.OnDeviceDetached -= OnControllerDettached;
        InputManager.OnDeviceAttached -= OnControllerAttached;
    }

    void Update()
    {
        WaitForDevice();

        GetInput();
        GroundedCheck();
        JumpInput();
        UpdateVelocityX();
        UpdateVelocityY();
        if (confinedSpaceAssist)
        {
            CheckForVerticalConfinedSpace();
            CheckForHorizontalConfinedSpace();
        }
        if (squarsh)
        {
            UpdateSquish();
        }
        bonkedPrevFrame = Bonking();

        
    }

    private void FixedUpdate()
    {
        if (rigidBodyComp.IsTouchingLayers(LayerMask.NameToLayer("Wall")))
        {
            //print("in the floor");

        }
        if (!pushing)
        {
            if (movementVelocity.x == 0 && grounded)
            {
                rigidBodyComp.bodyType = RigidbodyType2D.Kinematic;
                foreach (PlayerPiece eachCol in pieces)
                {
                    if (eachCol.isActiveAndEnabled)
                        eachCol.GetComponentInChildren<BoxCollider2D>().size = new Vector2(1, 1f);
                }
            }
            else if (!RunningIntoWall() || !grounded)
            {

                rigidBodyComp.bodyType = RigidbodyType2D.Dynamic;
                foreach (PlayerPiece eachCol in pieces)
                {
                    if (eachCol.isActiveAndEnabled)
                        eachCol.GetComponentInChildren<BoxCollider2D>().size = new Vector2(.95f, .95f);
                }
            }
        }
        UpdateRigidBody();
    }

    #region Input =================================================================================================
    public void ResetBothIncontrolThings()
    {
        //print("reset stf");
        playerInput.Device = null;
        playerInput.Enabled = false;
        playerInput.Destroy();
        playerInput = new MyCharacterActions();
    }

    public void OnControllerDettached(InputDevice device)
    {
        print("ON CONTROLLER DETATCHED");

        ResetBothIncontrolThings();

        // THIS IS FOR IF A CONTORLLER GETS UNPLUGGED DURING PLAY - REVERT THE GAME TO UNDECIDED SO THAT PLAYERS HAVE BETTER CONTROL
        CheckToSaveContrllerType();
        triedControls = false;

    }

    public void OnControllerAttached(InputDevice device)
    {
        print("ON CONTROLLER ATTATCHED");

        ResetBothIncontrolThings();

        // THIS IS FOR IF A CONTORLLER GETS UNPLUGGED DURING PLAY
        CheckToRevertControllerType();
        triedControls = false;
    }

    void CheckToSaveContrllerType()
    {
        switch (controllerManagerInstance.controllerTypeInputted)
        {
            case CurrentControllerSetup.Undecided:
                // Do Nothing
                break;
            case CurrentControllerSetup.OnePlayerKeyboard:
                if (InputManager.Devices.Count == 0)
                {
                    //controllerManagerInstance.isTwoPlayer = false;
                    //SaveControllerType();
                }
                break;
            case CurrentControllerSetup.OnePlayerController:
                if (InputManager.Devices.Count == 0)
                {
                    //controllerManagerInstance.isTwoPlayer = false;
                    //SaveControllerType();
                }
                break;
            case CurrentControllerSetup.TwoPlayerControllerAndKeyboard:
                if (InputManager.Devices.Count < 1)
                {
                    //controllerManagerInstance.isTwoPlayer = true;
                    //SaveControllerType();
                    if (controllerManagerInstance.controllerTypeInputted != CurrentControllerSetup.Undecided)
                    {
                        //controllerManagerInstance.lastControllerSetupType = controllerManagerInstance.controllerTypeInputted;
                        if (InputManager.Devices.Count == 0)
                        {
                            controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.OnePlayerKeyboard;
                            //controllerManagerInstance.isTwoPlayer = false;
                        }

                        if (controllerManagerInstance.swapPermitted == false)
                        {
                            controllerManagerInstance.swapPermitted = true;
                        }
                    }
                }
                break;
            case CurrentControllerSetup.TwoPlayerController:
                if (InputManager.Devices.Count < 2)
                {
                    //controllerManagerInstance.isTwoPlayer = true;
                    //SaveControllerType();
                    if (controllerManagerInstance.controllerTypeInputted != CurrentControllerSetup.Undecided)
                    {
                        //controllerManagerInstance.lastControllerSetupType = controllerManagerInstance.controllerTypeInputted;
                        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.TwoPlayerControllerAndKeyboard;

                        if (controllerManagerInstance.swapPermitted == true)
                        {
                            controllerManagerInstance.swapPermitted = false;
                        }
                    }
                }
                break;
        }
    }

    //void SaveControllerType()
    //{
    //    if (controllerManagerInstance.controllerTypeInputted != CurrentControllerSetup.Undecided)
    //    {
    //        controllerManagerInstance.lastControllerSetupType = controllerManagerInstance.controllerTypeInputted;
    //        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.Undecided;

    //        if (controllerManagerInstance.swapPermitted == false)
    //        {
    //            controllerManagerInstance.swapPermitted = true;
    //        }
    //    }
    //}

    void CheckToRevertControllerType()
    {
        switch (controllerManagerInstance.controllerTypeInputted)
        {
            case CurrentControllerSetup.Undecided:
                // Do Nothing
                break;
            case CurrentControllerSetup.OnePlayerKeyboard:
                //if there is no keyboard how are you playing?
                if (InputManager.Devices.Count >= 0)
                {
                    //controllerManagerInstance.isTwoPlayer = false;

                    if (controllerManagerInstance.swapPermitted == false)
                    {
                        controllerManagerInstance.swapPermitted = true;
                    }

                    if (InputManager.Devices.Count == 1)
                    {
                        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.OnePlayerController;
                    }
                    if (InputManager.Devices.Count > 1)
                    {
                        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.TwoPlayerController;
                        //controllerManagerInstance.isTwoPlayer = true;
                    }

                    //controllerManagerInstance.controllerTypeInputted = controllerManagerInstance.lastControllerSetupType;
                }
                break;
            case CurrentControllerSetup.TwoPlayerOneKeyboard:
                //if there is no keyboard how are you playing?
                if (InputManager.Devices.Count >= 0)
                {
                    //controllerManagerInstance.isTwoPlayer = false;

                    if (controllerManagerInstance.swapPermitted == false)
                    {
                        controllerManagerInstance.swapPermitted = true;
                    }

                    if (InputManager.Devices.Count == 1)
                    {
                        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.OnePlayerController;
                    }
                    if (InputManager.Devices.Count > 1)
                    {
                        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.TwoPlayerController;
                        //controllerManagerInstance.isTwoPlayer = true;
                    }

                    //controllerManagerInstance.controllerTypeInputted = controllerManagerInstance.lastControllerSetupType;
                }
                break;
            case CurrentControllerSetup.OnePlayerController:
                if (InputManager.Devices.Count > 0)
                {
                    //controllerManagerInstance.isTwoPlayer = false;

                    if (controllerManagerInstance.swapPermitted == false)
                    {
                        controllerManagerInstance.swapPermitted = true;
                    }

                    if (InputManager.Devices.Count > 1)
                    {
                        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.TwoPlayerController;
                        //controllerManagerInstance.isTwoPlayer = true;
                    }
                }
                break;
            case CurrentControllerSetup.TwoPlayerControllerAndKeyboard:
                if (InputManager.Devices.Count > 0)
                {
                    //controllerManagerInstance.isTwoPlayer = true;

                    if (controllerManagerInstance.swapPermitted == true)
                    {
                        controllerManagerInstance.swapPermitted = false;
                    }

                    if (InputManager.Devices.Count > 1)
                    {
                        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.TwoPlayerController;
                        //controllerManagerInstance.isTwoPlayer = true;
                    }
                    //controllerManagerInstance.controllerTypeInputted = controllerManagerInstance.lastControllerSetupType;
                }
                break;
            case CurrentControllerSetup.TwoPlayerController:
                if (InputManager.Devices.Count > 1)
                {
                    //controllerManagerInstance.isTwoPlayer = true;

                    if (controllerManagerInstance.swapPermitted == true)
                    {
                        controllerManagerInstance.swapPermitted = false;
                    }

                    //controllerManagerInstance.controllerTypeInputted = controllerManagerInstance.lastControllerSetupType;
                }
                break;
        }
    }

    /// <summary>
    /// waits for there to be a device corresponding with this player's id (ie there are enough plugged in)
    /// </summary>
    void WaitForDevice()
    {
        if (playerInput.Device == null  && !triedControls)
        {

            switch (controllerManagerInstance.controllerTypeInputted)
            {
                case CurrentControllerSetup.Undecided:
                    UndecidedInput();
                    break;
                case CurrentControllerSetup.OnePlayerKeyboard:
                    OnePlayerKeybaordInput();
                    break;
                case CurrentControllerSetup.TwoPlayerOneKeyboard:
                    TwoPlayerOneKeyboard();
                    break;
                case CurrentControllerSetup.OnePlayerController:
                    OnePlayerContoller();
                    break;
                case CurrentControllerSetup.TwoPlayerControllerAndKeyboard:
                    TwoPlayerKeyboardAndControllerInput();
                    break;
                case CurrentControllerSetup.TwoPlayerController:
                    TwoPlayerControllerInput();
                    break;
            }
            triedControls = true;
        }
    }

    void UndecidedInput()
    {
        if (controllerManagerInstance.swapPermitted == false)
        {
            controllerManagerInstance.swapPermitted = true;
        }

        // If devices == 0 - then make player 1 keyboard
        if (InputManager.Devices.Count == 0 && playerNumber == 0 )
        {
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.keyboard);
            playerInput.Enabled = true;
        }
        // if there is 1 con - then p1 on controller & p2 on key
        else if (InputManager.Devices.Count == 1)
        {
            if (playerNumber == 0)
            {
                playerInput.Device = InputManager.Devices[playerNumber];
                playerInput.Enabled = true;
                playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.controller);
            }
            if (playerNumber == 1)
            {
                playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.keyboard);
                playerInput.Enabled = true;
            }
        }
        // if there are 2 con - then p1 con controller & p2 on con and key
        else if (InputManager.Devices.Count > 1)
        {
            playerInput.Device = InputManager.Devices[playerNumber];
            playerInput.Enabled = true;
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.controller);

            if (playerNumber == 1)
            {
                playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.both);
                playerInput.Enabled = true;
            }
        }
    }

    void OnePlayerKeybaordInput()
    {
        if (controllerManagerInstance.swapPermitted == false)
        {
            controllerManagerInstance.swapPermitted = true;
        }

        if (playerNumber == 0)
        {
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.keyboard);
            playerInput.Enabled = true;
        }
    }

    void TwoPlayerOneKeyboard()
    {
        if (controllerManagerInstance.swapPermitted == false)
        {
            controllerManagerInstance.swapPermitted = true;
        }

        if (playerNumber == 0)
        {
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.P2KeyboardP1);
            playerInput.Enabled = true;
        }
        if (playerNumber == 1)
        {
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.P2KeyboardP2);
            playerInput.Enabled = true;
        }
    }

    void OnePlayerContoller()
    {
        if (controllerManagerInstance.swapPermitted == false)
        {
            controllerManagerInstance.swapPermitted = true;
        }

        if (playerNumber == 0)
        {
            playerInput.Device = InputManager.Devices[playerNumber];
            playerInput.Enabled = true;
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.controller);
        }
    }

    void TwoPlayerKeyboardAndControllerInput()
    {
        if (controllerManagerInstance.swapPermitted == true)
        {
            controllerManagerInstance.swapPermitted = false;
        }

        if (playerNumber == 0)
        {
            playerInput.Device = InputManager.Devices[playerNumber];
            playerInput.Enabled = true;
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.controller);
        }
        if (playerNumber == 1)
        {
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.keyboard);
            playerInput.Enabled = true;
        }
    }

    void TwoPlayerControllerInput()
    {
        if (controllerManagerInstance.swapPermitted == true)
        {
            controllerManagerInstance.swapPermitted = false;
        }

        if (playerNumber == 0)
        {
            playerInput.Device = InputManager.Devices[playerNumber];
            playerInput.Enabled = true;
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.controller);
        }
        if (playerNumber == 1)
        {
            playerInput.Device = InputManager.Devices[playerNumber];
            playerInput.Enabled = true;
            playerInput.SetupDefaults(inputSettings, MyCharacterActions.actionSetType.controller);
        }
    }

    #endregion

    void GetInput()
    {
        if (!playerInput.lockMovementAction.IsPressed && movementIsStopped == false)
        {
            movementInput = new Vector2(playerInput.horizontalAxis, playerInput.verticalAxis);

            // vvv marty added this stuff to fix the thing of players running at different speeds
            float angleOfRotational = Mathf.Rad2Deg * Mathf.Atan2(playerInput.horizontalAxis, playerInput.verticalAxis);
            if (movementInput.magnitude > .98f)
            {
                if (angleOfRotational > 35 && 145 > angleOfRotational)
                {
                    movementInput = new Vector2(1, 0);
                }
                if (angleOfRotational > -145 && -35 > angleOfRotational)
                {
                    movementInput = new Vector2(-1, 0);
                }
            }
            // ^^^

        }
        else if (grounded)
        {
            movementInput = Vector2.zero;
        }
        //throwInput = new Vector2(Input.GetAxis(hThrow), Input.GetAxis(vThrow));
    }

    /// <summary>
    /// determines which material to use for sound when walking
    /// </summary>

    public void StopMovement()
    {
        movementIsStopped = true;
        for (int i = 0; i < pieceColliders.Count; i++)
        {
            pieceColliders[i].gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        }
    }

    public void StopMovementNotCollision()
    {
        movementIsStopped = true;
    }

    public void StartMovement()
    {
        movementIsStopped = false;
        for (int i = 0; i < pieceColliders.Count; i++)
        {
            pieceColliders[i].gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    public void SetMovementDestination(Transform destination)
    {
        // give the player a target destination
        targetAutoMoveDestination = destination;
        reachedAutomatedDestination = false;
        reachedAutoXPos = false;
        reachedAutoYPos = false;
    }

    public void SetAutomatedMovement()
    {
        // set the movement to automated
        currentMovementType = MovementTypes.AutomatedMovement;
        //foreach (PlayerPiece piece in pieces) {
        //    piece.enabled = false;
        //}
    }

    public void AutomatedMovement()
    {
        // Check which way the target is from the player
        Vector3 playerVector = transform.TransformDirection(Vector3.right).normalized;
        Vector3 toDestinationVector = (targetAutoMoveDestination.position - transform.position).normalized;

        float dotDirection = Vector3.Dot(playerVector, toDestinationVector);

        //print(gameObject.name + " " + Vector3.Distance(targetAutoMoveDestination.position, transform.position));

        //if (Vector3.Distance(targetAutoMoveDestination.position, transform.position) <= 0.05f && reachedAutomatedDestination == false)
        if (Mathf.Abs(targetAutoMoveDestination.position.x - transform.position.x) <= 0.05f && reachedAutomatedDestination == false && transform.position.x != targetAutoMoveDestination.position.x)
        {
            // Reached the xPosition
            movementVelocity.x = 0;
            transform.position = new Vector3(targetAutoMoveDestination.position.x, transform.position.y, transform.position.z);
            reachedAutoXPos = true;

            if (transform.position.y < targetAutoMoveDestination.position.y && grounded == true)
            {
                //print("JUMP TO SPOT");
                // jump to position
                grounded = false;
                timeJumping = 0;
                yVelocity = startingV;
                rising = true;
                xPointJumpedFrom = transform.position.x;

                // ADD Jump Sound here
                fModJumpEvent.start();
            }
        }
        else if (reachedAutomatedDestination == false)
        {
            movementVelocity.x = xSpeedGrounded * dotDirection;
            rigidBodyComp.velocity = movementVelocity;

            // Add the movmentvelocity x (reduced) to the horizontal input so that the animation trigger plays
            movementInput = new Vector2(Mathf.Sign(movementVelocity.x), movementInput.y);
        }

        if (grounded == false)
        {
            if (rising && timeJumping > riseTime && shortHops)
            {
                //print("Fall");
                rising = false;
                timeJumping = 0;
            }
            if (cantJumpWithRoof && rising && Bonking())
            {
                rising = false;
                timeJumping = hangTime;
            }
        }

        if (Mathf.Abs(targetAutoMoveDestination.position.y - transform.position.y) <= 0.25f && rising == false && grounded == false && reachedAutoXPos == true && reachedAutoYPos == false)
        {
            //print("Stop jump at spot!");
            transform.position = new Vector3(transform.position.x, targetAutoMoveDestination.position.y, transform.position.z);
            reachedAutoYPos = true;
        }

        if (transform.position == targetAutoMoveDestination.position)
        {
            rigidBodyComp.velocity = Vector3.zero;
            grounded = true;
            timeJumping = 0;
            movementVelocity.y = 0;
            //print("have reached destination");
            reachedAutomatedDestination = true;
        }
    }
    void DetermineMat(PlayerPiece eachPiece)
    {
        switch (eachPiece.currentGround)
        {
            case groundMaterial.grass:
                fModStepEvent.setParameterValue("Surface", 0);
                fmodImpactEvent.setParameterValue("Surface", 0);
                particlesToSpawn = groundMatSettings.grassParticle;
                break;
            case groundMaterial.leaves:
                fModStepEvent.setParameterValue("Surface", 1);
                fmodImpactEvent.setParameterValue("Surface", 1);

                particlesToSpawn = groundMatSettings.leafParticle;
                break;
            case groundMaterial.wood:
                fModStepEvent.setParameterValue("Surface", 2);
                fmodImpactEvent.setParameterValue("Surface", 2);

                particlesToSpawn = groundMatSettings.woodParticle;
                break;
            case groundMaterial.dirt:
                fModStepEvent.setParameterValue("Surface", 3);
                fmodImpactEvent.setParameterValue("Surface", 3);

                particlesToSpawn = groundMatSettings.dirtParticle;
                break;
            case groundMaterial.snow:
                fModStepEvent.setParameterValue("Surface", 4);
                particlesToSpawn = groundMatSettings.snowParticle;
                fmodImpactEvent.setParameterValue("Surface", 4);
                break;
            case groundMaterial.rock:
                fModStepEvent.setParameterValue("Surface", 5);
                fmodImpactEvent.setParameterValue("Surface", 5);


                particlesToSpawn = groundMatSettings.rockParticle;
                break;
        }
    }

    /// <summary>
    /// do the particles seperately I guess
    /// </summary>
    void DetermineParticleMat(PlayerPiece eachPiece)
    {
        switch (eachPiece.currentGround)
        {
            case groundMaterial.grass:
                particlesToSpawn = groundMatSettings.grassParticle;
                break;
            case groundMaterial.leaves:
                particlesToSpawn = groundMatSettings.leafParticle;
                break;
            case groundMaterial.wood:
                particlesToSpawn = groundMatSettings.woodParticle;
                break;
            case groundMaterial.rock:
                particlesToSpawn = groundMatSettings.rockParticle;
                break;
            case groundMaterial.snow:
                break;
        }
    }
    /// <summary>
    /// sets whether or not the parent is grounded by checking all active children
    /// </summary>
    void GroundedCheck()
    {
        bool prevFrameGrounded = grounded;
        if (!rising)
        {
            grounded = false;
            bool materialDecided = false; //prioritize the first boi's mat for footstep sounds
            foreach (PlayerPiece eachPiece in pieces)
            {
                if (eachPiece.gameObject.activeInHierarchy)
                {
                    if (eachPiece.IsGrounded())
                    {
                        if (!materialDecided)
                        {
                            materialDecided = true;
                            DetermineMat(eachPiece);
                        }
                        if (!prevFrameGrounded)
                        {
                            if (footSounds)
                                fmodImpactEvent.start();
                            CreateGroundParticles(eachPiece);
                        }
                        grounded = true; //if any of your pieces is grounded then you are grounded young man

                        if (!rising)
                        {
                            yVelocity = 0;

                        }
                    }
                }
            }

            // if you walked off a ledge
            if (prevFrameGrounded && !grounded)
            {
                // set a marker for distance
                xPointJumpedFrom = transform.position.x;
                // start tracking time ungrounded
                if (!bonkedPrevFrame)
                    timeUngrounded = 0;

            }
        }
    }

    #region Confined Space Assist
    /// <summary>
    /// Determines if player is trying to push into a position
    /// </summary>
    [Header("Confined Vertical Space Variables")]
    [SerializeField] float confinedXThreshold; //for input
    private bool rightCheck = false, leftCheck = false;
    void CheckForVerticalConfinedSpace()
    {
        if (!grounded) //if player is not grounded and horizontal x input is greater than the threshold
        {
            foreach (PlayerPiece eachPiece in pieces)
            {
                if (eachPiece.gameObject.activeInHierarchy)
                {
                    if (movementInput.x >= confinedXThreshold) //right (positive)
                    {
                        rightCheck = eachPiece.CheckCastForConfinedSpaceV(Vector2.right, yVelocity);
                        if (rightCheck) //queue effect for right side
                        {
                            PushIntoVerticalConfinedSpace();
                            break; //prevent checking for more as the conditions are met
                        }
                    }

                    if (movementInput.x <= -confinedXThreshold) //left (negative)
                    {
                        leftCheck = eachPiece.CheckCastForConfinedSpaceV(Vector2.left, yVelocity);
                        if (leftCheck) //queue effect for left side
                        {
                            PushIntoVerticalConfinedSpace();
                            break; //prevent checking for more as the conditions are met
                        }
                    }
                }
            }
        }
    }

    void PushIntoVerticalConfinedSpace()
    {

        float distanceJumped = transform.position.x - xPointJumpedFrom;

        if (!(Mathf.Abs(distanceJumped) >= 1.8f && (movementInput.x * distanceJumped > 0))) //dont suck if at max h limit
        {
            rising = false;
            yVelocity = 0;
            movementVelocity = new Vector2(rigidBodyComp.velocity.x, 0);

            float roundedY = Mathf.Round(transform.position.y * 2) / 2;
            if (rightCheck)
            {
                transform.position = new Vector2(transform.position.x + .1f, roundedY);
                rightCheck = false;
            }

            if (leftCheck)
            {
                transform.position = new Vector2(transform.position.x - .1f, roundedY);
                leftCheck = false;
            }
        }
    }

    private bool upCheck = false, downCheck = false;
    void CheckForHorizontalConfinedSpace()
    {
        if (grounded)
        {
            if (Mathf.Abs(movementInput.x) <= .2f) //if no input
            {
                foreach (PlayerPiece eachPiece in pieces)
                {
                    if (eachPiece.gameObject.activeInHierarchy)
                    {
                        downCheck = eachPiece.CheckCastForConfinedSpaceH(Vector2.down, 0);
                        if (downCheck) //queue effect for bottom side
                        {
                            PushIntoHorizontalConfinedSpace();
                            break; //prevent checking for more as the conditions are met
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// check for a fitable space above a jumping player
    /// </summary>
    /// <returns>returns true if it has adjusted the player's position</returns>
    bool UpwardConfinedSpaceCheck()
    {
        foreach (PlayerPiece eachPiece in pieces)
        {
            if (eachPiece.gameObject.activeInHierarchy)
            {
                upCheck = eachPiece.CheckCastForConfinedSpaceH(Vector2.up, 1);
                if (upCheck) //queue effect for top side
                {
                    PushIntoHorizontalConfinedSpace();

                    return true; //prevent checking for more as the conditions are met
                }
            }
        }
        return false;
    }

    void PushIntoHorizontalConfinedSpace()
    {



        float roundedX = (Mathf.Round(transform.position.x * 2)) / 2;

        //these are separate in case we want to make additional changes to either of them
        if (upCheck)
        {
            transform.position = new Vector2(roundedX, transform.position.y);
            upCheck = false;
        }

        if (downCheck)
        {
            transform.position = new Vector2(roundedX, transform.position.y);
            downCheck = false;
        }

    }

    #endregion

    GameObject CreateGroundParticles(PlayerPiece eachPiece)
    {
        if (particlesEnabled)
        {
            DetermineParticleMat(eachPiece);
            if (particlesToSpawn)
            {
                GameObject newParticleBoi = Instantiate(particlesToSpawn, eachPiece.transform.position + (Vector3.down / 2), transform.rotation); //i can later rotate these based on velocity
                newParticleBoi.transform.up -= (new Vector3(movementVelocity.normalized.x, movementVelocity.normalized.y, 0)) * particleTiltMult;
                return newParticleBoi;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// determines whether or not the player is up against a wall
    /// </summary>
    /// <returns></returns>
    bool RunningIntoWall()
    {
        foreach (PlayerPiece eachPiece in pieces)
        {
            if (eachPiece.gameObject.activeInHierarchy)
            {
                if (eachPiece.IsRunningIntoWall(movementInput.x))
                {
                    return true;
                }
            }
        }
        return false;
    }
    bool Bonking()
    {
        foreach (PlayerPiece eachPiece in pieces)
        {
            if (eachPiece.gameObject.activeInHierarchy)
            {
                if (eachPiece.CheckCastForJumpables(eachPiece.upCast1) || eachPiece.CheckCastForJumpables(eachPiece.upCast2))
                {
                    return true;
                }
            }
        }
        return false;
    }
    void JumpInput()
    {
        if (movementIsStopped == false)
        {
            bool isFitableSpace = false;
            if (playerInput.jumpAction.WasPressed && grounded)
            {
                isFitableSpace = UpwardConfinedSpaceCheck();
            }
            if (isFitableSpace)
            {
                //if you got fit into a space upward  recheck your raycasts and grounded
                foreach (PlayerPiece eachPiece in pieces)
                {
                    if (eachPiece.gameObject.activeInHierarchy)
                    {
                        eachPiece.UpdateNeighbours();
                    }
                }
                GroundedCheck();
            }
            //jumping
            if (playerInput.jumpAction.WasPressed && (grounded || (jumpAfterWalkOffLedge && timeUngrounded < walkOffLedgeJumpWindow)) && (!cantJumpWithRoof || isFitableSpace || !Bonking()))
            {
                if (grounded)
                {
                    // only update this when jumping from grounded, otherwise use the point of the ledge walk off
                    xPointJumpedFrom = transform.position.x;
                }
                grounded = false;
                timeJumping = 0;
                yVelocity = startingV;
                rising = true;
                timeUngrounded = walkOffLedgeJumpWindow;

                // Add Jump Sound Here
                fModJumpEvent.start();
            }

            else if (rising && timeJumping < riseTime && shortHops && playerInput.jumpAction.WasReleased)
            {

                rising = false;
                timeJumping = 0;
            }

            //else if (!Input.GetButton(jumpButton) && yInCurve<shortHopWindow)
            //{
            //    currentCurve = shortHopArc;
            //}
            if (!grounded)
            {
                timeUngrounded+=Time.deltaTime;
            }
        }
    }
    /// <summary>
    /// A function that will be used on the controller select to make the other player character jump when a one player option is selected
    /// </summary>
    public void CinematicJump()
    {
        if (grounded)
        {
            // only update this when jumping from grounded, otherwise use the point of the ledge walk off
            xPointJumpedFrom = transform.position.x;
        }
        grounded = false;
        timeJumping = 0;
        yVelocity = startingV;
        rising = true;
        timeUngrounded = walkOffLedgeJumpWindow;

        // Add Jump Sound Here
        fModJumpEvent.start();
    }

    void UpdateVelocityX()
    {
        /*
        //if (!RunningIntoWall())
        //{
        if(grounded || !castevaniaJumps)
        if (movementInput.x > 0.2f)
        {
            movementVelocity.x = xSpeed * Mathf.Abs(movementInput.x);
        }
        else if (movementInput.x < -0.2f)
        {
            movementVelocity.x = -xSpeed * Mathf.Abs(movementInput.x);
        }
        else
        {
            movementVelocity.x = 0;
        }

        //}
        //else
        //{
        //    print("stop running into walls ");
        //    movementVelocity.x = 0;
        //}
        */
        if (currentMovementType == MovementTypes.PlayerMovement)
        {
            if (grounded || (!castevaniaJumps && !jumpHorizontalLimit))
            {
                if (movementInput.x > 0.2f)
                {
                    //if (movementVelocity.x < 0 && grounded)
                    //{
                    //    pivotParticles = CreateGroundParticles(pieces[0]);
                    //}
                    movementVelocity.x = xSpeedGrounded * Mathf.Abs(movementInput.x);
                }
                else if (movementInput.x < -0.2f)
                {
                    //if (movementVelocity.x > 0 && grounded)
                    //{
                    //    pivotParticles = CreateGroundParticles(pieces[0]);
                    //}
                    movementVelocity.x = -xSpeedGrounded * Mathf.Abs(movementInput.x);
                }
                else
                {
                    movementVelocity.x = 0;
                }
                if (RunningIntoWall())
                {
                    movementVelocity.x = 0;
                }

                // If the player is moving then play a stepping sound
                if (movementVelocity.x != 0)
                {
                    stepTime -= Time.deltaTime * Mathf.Abs(movementVelocity.x);
                    if (stepTime <= 0)
                    {
                        stepTime = originalStepTime;
                        if (footSounds)
                            fModStepEvent.start();
                    }
                }
                else
                {
                    stepTime = 0.01f;
                }
                if (horizontalAdjust)
                {
                    // put in check for weird snag
                    if (Mathf.Abs(rigidBodyComp.velocity.x) <= .001f && Mathf.Abs(movementVelocity.x) > 0)
                    {
                        horizontalSnagCheck++;
                        //print(" player "+ playerNumber+ " check number: " + cornerSnagCheck);
                        if (horizontalSnagCheck > 2)
                        {
                            //bool touchingSomethin = false;
                            ////dont do it if you're touching anything
                            //foreach (Collider2D eachPiece in pieceColliders)
                            //{
                            //    if (eachPiece.IsTouchingLayers(LayerMask.GetMask("Player")))
                            //    {
                            //        if (SaveManager.instance)
                            //        {
                            //            if (!otherPlayer.grounded)
                            //            {

                            //                touchingSomethin = true;
                            //                print("its cause you're hitting the player");
                            //            }

                            //        }
                            //    }
                            //}
                            //if (!touchingSomethin)
                            //{
                            // okay it's doing the thing
                            rigidBodyComp.transform.position += new Vector3(xSpeedGrounded * movementInput.x, 0, 0) * Time.deltaTime;
                            //print("H adjusted");
                            //}
                        }
                    }
                    else
                    {
                        horizontalSnagCheck = 0;
                    }
                }

            }

            else if (jumpHorizontalLimit)
            {
                // calculate this frame's slowed x speed
                float distanceJumped = transform.position.x - xPointJumpedFrom;
                float modifiedXSpeed = xSpeedAerial;

                // if the player is past the slowing point AND is still moving in that direction
                if (Mathf.Abs(distanceJumped) > slowingPointPosition && (movementInput.x * distanceJumped > 0))
                {
                    // calculate the adjusted movement speed
                    modifiedXSpeed = xSpeedAerial * (1 - (Mathf.Abs(distanceJumped) - slowingPointPosition) / (maxHorizontalJump - slowingPointPosition));
                    if (modifiedXSpeed < 0)
                        modifiedXSpeed = 0;
                }

                // update velocity
                if (movementInput.x > 0.2f)
                {
                    movementVelocity.x = modifiedXSpeed * Mathf.Abs(movementInput.x);
                }
                else if (movementInput.x < -0.2f)
                {
                    movementVelocity.x = -modifiedXSpeed * Mathf.Abs(movementInput.x);
                }
                else
                {
                    movementVelocity.x = 0;
                }

            }
        }
        if (currentMovementType == MovementTypes.AutomatedMovement)
        {
            AutomatedMovement();
        }
    }
    void UpdateVelocityY()
    {
        if (!grounded)
        {
            if (rising)
            {
                if (timeJumping <= riseTime)
                {
                    if (!Bonking())
                    {
                        yVelocity += risingGravity * Time.deltaTime;
                        bonkTimer = 0;
                    }

                }
                else
                {
                    rising = false;
                    timeJumping = 0;
                }
            }
            else
            {
                if (timeJumping <= hangTime)
                {
                    yVelocity *= shortHopDeAccel;
                }
                else
                {
                    yVelocity += fallAccel * Time.deltaTime;
                }
                if (verticalAdjust)
                {
                    // put in check for weird snag
                    if (rigidBodyComp.velocity.y >= -.001f && yVelocity < 0)
                    {
                        cornerSnagCheck++;
                        //print(" player "+ playerNumber+ " check number: " + cornerSnagCheck);
                        if (cornerSnagCheck > 2)
                        {
                            bool touchingSomethin = false;
                            //dont do it if you're touching anything
                            foreach (Collider2D eachPiece in pieceColliders)
                            {
                                if (eachPiece.IsTouchingLayers(LayerMask.GetMask("Player")))
                                {

                                    if (!otherPlayer.grounded)
                                    {

                                        touchingSomethin = true;
                                        //print("its cause you're hitting the player");
                                    }


                                }
                            }
                            if (!touchingSomethin)
                            {
                                // okay it's doing the thing
                                rigidBodyComp.transform.position += new Vector3(0, Mathf.Clamp(yVelocity, -maxFallSpeed, maxFallSpeed) * Time.deltaTime, 0);
                                //print("adjusted");
                            }
                        }
                    }
                    else
                    {
                        cornerSnagCheck = 0;
                    }
                    // save rigidbodycomp velocity
                    // move down manually
                    // return rigidbodycomp velocity
                }
            }

            if (Bonking())
            {
                if (!rising)
                {
                    bonkTimer = stickToRoofTime;
                }
                bonkTimer += Time.deltaTime;
                if (bonkTimer >= stickToRoofTime)
                {
                    if (rising)
                    {
                        yVelocity = 0;

                        rising = false;
                        timeJumping = hangTime;
                    }

                    yVelocity += risingGravity * Time.deltaTime;
                }
                //else if (movementVelocity.x == 0)
                //{

                //}
            }

            timeJumping += Time.deltaTime;

            movementVelocity.y = Mathf.Clamp(yVelocity, -maxFallSpeed, maxFallSpeed);
        }
        else
        {
            movementVelocity.y = 0;
        }
    }
    void UpdateSquish()
    {
        //squash
        float squashScaleX = Mathf.Max(minimumScale, 1 - defaultScale.y * (Mathf.Abs(rigidBodyComp.velocity.y) / maxFallSpeed));
        float squashScaleY = Mathf.Max(minimumScale, 1 - defaultScale.x * (Mathf.Abs(rigidBodyComp.velocity.x) / xSpeedGrounded));

        //stretch
        squashScaleX += Mathf.Max((defaultScale.y - squashScaleY), 1 - defaultScale.y);
        squashScaleY += Mathf.Max((defaultScale.x - squashScaleX), 1 - defaultScale.x);

        //to squish each block
        foreach (SpriteRenderer eachPiece in pieceSpriteRenderers)
        {
            eachPiece.transform.localScale = new Vector3(
                    squashScaleX,
                     squashScaleY,
                    eachPiece.transform.localScale.z);
        }

        //to squish the whole boi
        //transform.localScale = new Vector3(
        //                        squashScaleX,
        //                         squashScaleY,
        //                        transform.localScale.z);
    }
    void UpdateRigidBody()
    {
        rigidBodyComp.velocity = movementVelocity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vectorToRound">the vector to start from</param>
    /// <returns>returns the passed vector with all of it's components rounded to the nearest int</returns>
    public static Vector3 RoundVectorToPoint5s(Vector3 vectorToRound)
    {

        return new Vector3(Mathf.Round(vectorToRound.x * 2) / 2, Mathf.Round(vectorToRound.y * 2) / 2, Mathf.Round(vectorToRound.z));

    }
}
