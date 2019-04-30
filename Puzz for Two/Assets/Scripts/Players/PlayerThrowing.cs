using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerThrowing : MonoBehaviour
{
    static float rightStickThreshhold = .1f;
    public enum RotationType
    {
        EightWay,
        Free,
        Orthagonal
    }
    //private Player player; //the rewired manager

    //[Header("Health Throwing")]
    //public float timeToThrow = 1f;
    //private float timeInThrow = 0f;

    //public AnimationCurve throwStrengthCurve;
    //public LineRenderer throwLine;

    public GameObject rotationIndicatorIcon; //the object we intend to rotate
    public GameObject pieceToThrow;
    public GameObject objectToRotateAround;
    private PlayerHealth playerHealthComp;
    public float throwForce = 40f;
    //private bool shotCancelled = false;

    [Header("Rotation Values")]
    public RotationType typeRotation;
    [SerializeField] float magnitude = 0.4f, deadZoneXThreshold = 0.5f, deadZoneYThreshold = 0.5f;
    private float horizontalAxis, verticalAxis, horizontalAxisKeyboard;

    [SerializeField] public string horizontal, vertical, action;
    private bool canShoot = false;
    [SerializeField] bool axisInUse = false;

    private Vector3 offset;
    Movement playerMoveComp;

    [Header("Throwing Values")]
    public ThrowingProfile throwingProfile;
    [HideInInspector]
    public Vector2 rotationIndicatorInput;
    private Throw currentThrow;
    // Stopping Variables
    public bool throwingIsStopped = false;
    PlayerPieceAnimatorManager pieceAnimatorManager;

    // Create FMOD Sound Effect Variables
    [Header("FMOD Audio Events")]
    [FMODUnity.EventRef]
    public string throwPieceSound,throwScream;
    [HideInInspector] public FMOD.Studio.EventInstance fModThrowPieceEvent,throwScreamEvent;

    List<GameObject> thrownPieces = new List<GameObject>(); // For campfire
    PlayerPieceAnimatorManager playerPieceAnimatorComp;

    private void Awake()
    {
        playerMoveComp = GetComponent<Movement>();
        playerHealthComp = GetComponent<PlayerHealth>();

        throwingProfile.diagonalUp = new Throw(throwingProfile.diagonalUp.yDistance, throwingProfile.diagonalUp.xDistance, throwingProfile.diagonalUp.timeToApex);
        throwingProfile.diagonalDown = new Throw(throwingProfile.diagonalDown.yDistance, throwingProfile.diagonalDown.xDistance, throwingProfile.diagonalDown.timeToApex);
        throwingProfile.upwards = new Throw(throwingProfile.upwards.yDistance, throwingProfile.upwards.xDistance, throwingProfile.upwards.timeToApex);
        throwingProfile.downwards = new Throw(throwingProfile.downwards.yDistance, throwingProfile.downwards.xDistance, throwingProfile.downwards.timeToApex);
        throwingProfile.horizontals = new Throw(throwingProfile.horizontals.yDistance, throwingProfile.horizontals.xDistance, throwingProfile.horizontals.timeToApex);

        playerPieceAnimatorComp = GetComponent<PlayerPieceAnimatorManager>();
    }

    private void Start()
    {
        //set default throw to right to avoid infinite spawn bois
        currentThrow = throwingProfile.horizontals;
        horizontalAxis = 1;
        ObjectRotation();

        fModThrowPieceEvent = FMODUnity.RuntimeManager.CreateInstance(throwPieceSound);
        throwScreamEvent = FMODUnity.RuntimeManager.CreateInstance(throwScream);
    }

    void Update()
    {
        UpdateAxis();
        

        // Register when the player holds down the shoot button
        // When the player presses the shoot button, fire a projectile

        //if (Input.GetAxis(action) <= 0f)
        //{
        //    axisInUse = false;
        //}

        //if (!axisInUse)
        //{
        if (playerMoveComp.playerInput.throwAction.WasPressed)
        {

            if (throwingIsStopped == false)
            {
                if (playerHealthComp.health > 1)
                {
                    ShootProjectile();
                    axisInUse = true;
                }
                else
                {
                    if (rotationIndicatorIcon.activeSelf == true)
                    {
                        rotationIndicatorIcon.SetActive(false);
                    }
                }
            }
        }


        //if (playerHealthComp.health > 1 && rotationIndicatorIcon.activeSelf == false)
        //{
        //    rotationIndicatorIcon.SetActive(true);
        //}
        //else if (playerHealthComp.health <= 1 && rotationIndicatorIcon.activeSelf == true)
        //{
        //    rotationIndicatorIcon.SetActive(false);
        //}
    }

    void UpdateAxis() //updating the axis for the player
    {
        if (throwingIsStopped == false)
        {
            if (Mathf.Abs(playerMoveComp.playerInput.altHorizontalAxis) > rightStickThreshhold || Mathf.Abs(playerMoveComp.playerInput.altVerticalAxis) > rightStickThreshhold)
            {
                horizontalAxis = playerMoveComp.playerInput.altHorizontalAxis;
                verticalAxis = playerMoveComp.playerInput.altVerticalAxis;
            }
            else
            {
                horizontalAxis = playerMoveComp.playerInput.horizontalAxis;
                verticalAxis = playerMoveComp.playerInput.verticalAxis;
            }
        }
    }
    
    public void StopThrowing()
    {
        throwingIsStopped = true;
        rotationIndicatorIcon.SetActive(false);
    }

    public void StartThrowing()
    {
        throwingIsStopped = false;
        rotationIndicatorIcon.SetActive(false);
    }

    GameObject ShootProjectile()
    {
        // Generate the object to shot from the player
        //Vector2 positionThrown = playerHealthComp.playerBlockParent.GetChild((int)playerHealthComp.health - 1).transform.position;
        //positionThrown.y = Mathf.RoundToInt(positionThrown.y) + .5f;

        //if grounded
        Vector2 positionThrown = playerHealthComp.playerBlockParent.GetChild((int)playerHealthComp.health - 1).transform.position;
        //positionThrown = Movement.RoundVectorToPoint5s(positionThrown);
        //if (positionThrown.y % 1 == 0)
        //{
        //    positionThrown.y += .5f;
        //    print("on the line");
        //}
        positionThrown.y = Mathf.Round(positionThrown.y + .5f) - .5f;


        GameObject pieceThrown = Instantiate(pieceToThrow, positionThrown, Quaternion.identity);

        //set the face of the new loose boi
        LookAtObject looseBoiFace = pieceThrown.GetComponentInChildren<LookAtObject>();
        looseBoiFace.randomizeFace = false;
        LookAtObject newBoiFace = playerHealthComp.playerBlockParent.GetChild((int)playerHealthComp.health - 1).GetComponentInChildren<LookAtObject>();
        looseBoiFace.GetComponent<SpriteRenderer>().sprite = newBoiFace.GetComponent<SpriteRenderer>().sprite;
        if (newBoiFace.alreadyGotSpecialFace)
        {
            looseBoiFace.alreadyGotSpecialFace = true;
        }
        //make them look at the one who dropped them (can change)
        looseBoiFace.autofindLookTarget = false;
        looseBoiFace.targetLook = transform;

        DetachedPiece detachedPieceComp = pieceThrown.GetComponent<DetachedPiece>();
        detachedPieceComp.IgnoreColl(playerMoveComp.pieceColliders, true);
        detachedPieceComp.RevertCollision();

        //Get the Vectpr to fire the piece from
        Transform playerHead = playerHealthComp.playerBlockParent.GetChild((int)playerHealthComp.health - 1);
        Vector3 fireDirectionVector = (rotationIndicatorIcon.transform.position - playerHead.position).normalized;

        CalculateThrow(); // Calculate the shot velocity and gravity depending on the incial launch angle


        Rigidbody2D pieceRigidBody = pieceThrown.GetComponent<Rigidbody2D>();
        pieceRigidBody.velocity = new Vector2(currentThrow.startingVelocity.x * rotationIndicatorInput.x, currentThrow.startingVelocity.y * Mathf.Sign(rotationIndicatorInput.y));
        pieceRigidBody.gravityScale = 0; //temporarily turn off the normal gravity in order to do it manually for the throws
        detachedPieceComp.gravity = currentThrow.gravity;
        detachedPieceComp.hasLanded = false;
        if (currentThrow == throwingProfile.horizontals)
        {

            detachedPieceComp.SetTimeFromSpeedAndDistance(currentThrow.xDistance);


        }
        playerHealthComp.OnThrow(); //ENABLE TO DETATCH PIECES

        // Play Throw Sound
        fModThrowPieceEvent.start();
        //throwScreamEvent.start();
        return pieceThrown;
    }

    // THIS IS THE OLD PREFAB FIREING FOR THE CAMPFIRE DELETE LATER ------------------------------------------------------------------
    GameObject ShootProjectile(Throw directionalThrow, Transform stopPos, Vector2 positionThrown)
    {
        //if grounded
        //Vector2 positionThrown = playerHealthComp.playerBlockParent.GetChild((int)playerHealthComp.health - 1).transform.position;
        //if (positionThrown.y % 2 == 0)
        //{
        //    positionThrown.y = Mathf.RoundToInt(positionThrown.y) + .5f;
        //}

        GameObject pieceThrown = Instantiate(pieceToThrow, positionThrown, Quaternion.identity);
        foreach (Transform child in pieceThrown.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        }
        DetachedPiece detachedPieceComp = pieceThrown.GetComponent<DetachedPiece>();
        detachedPieceComp.IgnoreColl(playerMoveComp.pieceColliders, true);
        detachedPieceComp.RevertCollision();

        // Set the detatched pieces stopping point
        //detachedPieceComp.SetStoppingDestination(stopPos);

        //Get the Vectpr to fire the piece from
        //Transform playerHead = playerHealthComp.playerBlockParent.GetChild((int)playerHealthComp.health - 1);
        //Vector3 fireDirectionVector = (rotationIndicatorIcon.transform.position - playerHead.position).normalized;

        //CalculateThrow(); // Calculate the shot velocity and gravity depending on the incial launch angle


        Rigidbody2D pieceRigidBody = pieceThrown.GetComponent<Rigidbody2D>();
        pieceRigidBody.velocity = new Vector2(directionalThrow.startingVelocity.x, directionalThrow.startingVelocity.y);
        pieceRigidBody.gravityScale = 0; //temporarily turn off the normal gravity in order to do it manually for the throws
        detachedPieceComp.gravity = directionalThrow.gravity;
        detachedPieceComp.hasLanded = false;

        playerHealthComp.OnThrow(); //ENABLE TO DETATCH PIECES

        // Play Throw Sound
        fModThrowPieceEvent.start();

        return pieceThrown;
    }

    public List<GameObject> ThrowToSpecificSpot(List<Transform> throwPositions)
    {
        int currentPlayerHealth = (int)playerHealthComp.health;

        //// call the throw code for the number of pieces that the player has
        //for (int i = 1; i < currentPlayerHealth; i++)
        //{
        //    // throw each boy upwards at the same time
        //    Throw newThrow;
        //    float endXPos = throwPositions[throwPositions.Count - 1].position.x - playerMoveComp.pieces[i].transform.position.x;

        //    newThrow = new Throw(3f, endXPos + (Mathf.Sign(endXPos) * (.03f * Mathf.Abs(endXPos))), .5f);
        //    GameObject thrownPiece = ShootProjectile(newThrow, throwPositions[throwPositions.Count - 1], playerMoveComp.pieces[i].transform.position);

        //    throwPositions.RemoveAt(throwPositions.Count - 1);  // Remove the sitting position from the list so its occupied

        //    thrownPieces.Add(thrownPiece);
        //}
        // --------------------------------------------------------

        // get the randomized spot
        // delete it from the list
        // set the various physics of the camper (namely gravity) to 0
        // create a point in the middle of the disance
        // move the mid point up a given height
        // lerp from starting spot -> mid spot - end spot
        // add a modifcation to the height of the piece while lerping from one spot to another
        // add a modifcation to the speed of the piece while lerping from one spot to another
        // make a way for the player piece to potencially miss it's spot at the campfire

        // Play Throw Sound
        fModThrowPieceEvent.start();
        //print("play sound");


        for (int i = 1; i< currentPlayerHealth; i++)
        {
            // throw each boy upwards at the same time
            GameObject thrownPiece = SetUpAutomatedThrowing(throwPositions[throwPositions.Count - 1], playerMoveComp.pieces[i].transform.position);

            throwPositions.RemoveAt(throwPositions.Count - 1);  // Remove the sitting position from the list so its occupied
            thrownPieces.Add(thrownPiece);

            //playerMoveComp.playerInput.throwAction.SetValue(1, 1 / 60);
            if (currentPlayerHealth > 1 && (int)playerHealthComp.health == 1)
            {
                playerPieceAnimatorComp.automatedThrow = true;
            }
        }

        return thrownPieces;
    }

    GameObject SetUpAutomatedThrowing(Transform stopPos, Vector2 positionThrown)
    {
        GameObject pieceThrown = Instantiate(pieceToThrow, positionThrown, Quaternion.identity);
        foreach (Transform child in pieceThrown.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        }
        DetachedPiece detachedPieceComp = pieceThrown.GetComponent<DetachedPiece>();
        detachedPieceComp.IgnoreColl(playerMoveComp.pieceColliders, true);
        detachedPieceComp.RevertCollision();

        // SET UP POSITION TO LERP TO
        Vector3 midPos = new Vector3();
        midPos.x = stopPos.position.x + (positionThrown.x - stopPos.position.x) / 2;
        midPos.y = stopPos.position.y + 3f;

        Vector3[] positions = new Vector3[] { positionThrown, midPos, stopPos.position };

        // Set the detatched pieces stopping point
        detachedPieceComp.SetStoppingDestination(positions);

        //Rigidbody2D pieceRigidBody = pieceThrown.GetComponent<Rigidbody2D>();
        //pieceRigidBody.gravityScale = 0; //temporarily turn off the normal gravity in order to do it manually for the throws


        //set the face of the new loose boi
        LookAtObject looseBoiFace = pieceThrown.GetComponentInChildren<LookAtObject>();
        looseBoiFace.randomizeFace = false;
        LookAtObject newBoiFace = playerHealthComp.playerBlockParent.GetChild((int)playerHealthComp.health - 1).GetComponentInChildren<LookAtObject>();
        looseBoiFace.GetComponent<SpriteRenderer>().sprite = newBoiFace.GetComponent<SpriteRenderer>().sprite;
        if (newBoiFace.alreadyGotSpecialFace)
        {
            looseBoiFace.alreadyGotSpecialFace = true;
        }
        //make them look at the one who dropped them (can change)
        looseBoiFace.autofindLookTarget = false;
        looseBoiFace.targetLook = transform;

        playerHealthComp.OnThrow(); //ENABLE TO DETATCH PIECES

        return pieceThrown;
    }

    void ObjectRotation() //function to rotate our object
    {
        if (Mathf.Abs(horizontalAxis) >= deadZoneXThreshold || Mathf.Abs(verticalAxis) >= deadZoneYThreshold)
        {
            float angleOfRotational = Mathf.Atan2(horizontalAxis, verticalAxis);
            Vector2 normalizedV = new Vector2(horizontalAxis, verticalAxis).normalized;

            switch (typeRotation)
            {
                case RotationType.EightWay:
                    normalizedV = new Vector2(Mathf.Round(normalizedV.x), Mathf.Round(normalizedV.y));
                    offset = normalizedV * magnitude;

                    rotationIndicatorInput = normalizedV; // The Input values of the Rotational Indicator needed for the throw angle
                    break;
                case RotationType.Free:
                    offset = Quaternion.AngleAxis(angleOfRotational, objectToRotateAround.transform.forward) * normalizedV * magnitude;
                    break;
                case RotationType.Orthagonal:
                    normalizedV = new Vector2(Mathf.Round(normalizedV.x), Mathf.Round(normalizedV.y));
                    if (normalizedV.x != 0)
                    {
                        normalizedV.y = 0;
                    }
                    offset = normalizedV * magnitude;
                    rotationIndicatorInput = normalizedV; // The Input values of the Rotational Indicator needed for the throw angle
                    break;
            }
        }

        // Se the position of the projectile to be on the last gathered blocked
        int numChild = (int)playerHealthComp.health - 1;
        Transform positionOfHead = playerHealthComp.playerBlockParent.GetChild(numChild).gameObject.transform;
        rotationIndicatorIcon.transform.position = positionOfHead.position + offset; //add the offset to the object we're holding

        //MoveRotationalObject(); //check for keyboard input to modify the offset value

        rotationIndicatorIcon.transform.right = -(positionOfHead.position - rotationIndicatorIcon.transform.position).normalized;
    }

    void CalculateThrow()
    {
        if (typeRotation == RotationType.EightWay || typeRotation == RotationType.Orthagonal)
        {
            //print(rotationIndicatorInput);

            if (Mathf.Abs(rotationIndicatorInput.y) == 1 && rotationIndicatorInput.x == 0)  // UP
                currentThrow = throwingProfile.upwards;
            else if (Mathf.Abs(rotationIndicatorInput.y) == -1 && rotationIndicatorInput.x == 0)  // DOWN
                currentThrow = throwingProfile.downwards;
            else if (rotationIndicatorInput.y == 0 && Mathf.Abs(rotationIndicatorInput.x) == 1) // HORIZONTAL
                currentThrow = throwingProfile.horizontals;
            else if (rotationIndicatorInput.y == 1 && Mathf.Abs(rotationIndicatorInput.x) == 1)  // DIAGONALUP
                currentThrow = throwingProfile.diagonalUp;
            else if (rotationIndicatorInput.y == -1 && Mathf.Abs(rotationIndicatorInput.x) == 1)  // DIAGONALDOWN
                currentThrow = throwingProfile.diagonalDown;
        }
    }
    private void FixedUpdate()
    {
        if (!playerMoveComp.playerInput.lockThrowAction)
        {
            ObjectRotation();
        }
    }
    Throw CalculateThrow(Vector2 input)
    {
        Throw throwType = currentThrow;

        if (typeRotation == RotationType.EightWay || typeRotation == RotationType.Orthagonal)
        {
            //print(rotationIndicatorInput);

            if (Mathf.Abs(input.y) == 1 && input.x == 0)  // UP
                throwType = throwingProfile.upwards;
            else if (Mathf.Abs(input.y) == -1 && input.x == 0)  // DOWN
                throwType = throwingProfile.downwards;
            else if (input.y == 0 && Mathf.Abs(input.x) == 1) // HORIZONTAL
                throwType = throwingProfile.horizontals;
            else if (input.y == 1 && Mathf.Abs(input.x) == 1)  // DIAGONALUP
                throwType = throwingProfile.diagonalUp;
            else if (input.y == -1 && Mathf.Abs(input.x) == 1)  // DIAGONALDOWN
                throwType = throwingProfile.diagonalDown;
        }

        return throwType;
    }

    void MoveRotationalObject()
    {
        if (Mathf.Abs(horizontalAxisKeyboard) >= deadZoneXThreshold) //if moving via keyboard controls
        {
            rotationIndicatorIcon.transform.position = objectToRotateAround.transform.position + (rotationIndicatorIcon.transform.position - objectToRotateAround.transform.position).normalized * magnitude; //change position
            rotationIndicatorIcon.transform.RotateAround(objectToRotateAround.transform.position, Vector3.forward, horizontalAxisKeyboard * Time.deltaTime); //rotate around based on new position
            offset = rotationIndicatorIcon.transform.position - objectToRotateAround.transform.position; //reset offset
        }
    }

}
