using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// the script on each of the player's pieces, tracks what's next to it, does logic for what sprite to show and such
/// </summary>
public enum groundMaterial
{
    grass,
    wood,
    leaves,
    rock,
    snow,
    dirt
}
public enum reverbType
{

    outside,
    house,
    cave,
}
public class PlayerPiece : MonoBehaviour
{
    static bool butterflyJump = false; //whether or not to enable jumping on midair players
    static bool stickyWalls = true; //whether or not to align to grid when rubbed up on a wall
    static float castLength, downCastWidth; //how much past a blocks width it should cast
    static bool sucksWithPlayers = false;
    [HideInInspector] public RaycastHit2D leftCast, rightCast, upCast1, upCast2, downCast1, downCast2;  //the pieces next to it
    [HideInInspector] public Movement parentMovement;
    [SerializeField] LayerMask solidLayers;
    BoxCollider2D myOwnCollider;
    public PlayerPiece pieceThatGrabsMe;
    public Vector2 directionOfPieceThatGrabsMe;
    public groundMaterial currentGround;
    public Sprite highlightSprite;
    public int activePiecesUp, activePiecesDown, activePiecesLeft, activePiecesRight;
    [SerializeField] bool manuallySort;
    Vector2[] fourDirections = new Vector2[4] { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
    // Use this for initialization
    void Start()
    {
        pieceThatGrabsMe = GetBlockThatGrabsMe();
        parentMovement = GetComponentInParent<Movement>();
        myOwnCollider = GetComponentInChildren<BoxCollider2D>();
        downCastWidth = (myOwnCollider.size.x / 2);
        castLength = (myOwnCollider.size.y / 2) + .05f;
        if (highlightSprite == null)
        {
            highlightSprite = GetComponentInChildren<SpriteRenderer>(true).sprite;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNeighbours();
        if (stickyWalls)
            AvoidSticking();

        //if (Input.GetKeyDown("l"))
        //{
        //    pieceThatGrabsMe = GetBlockThatGrabsMe();
        //}
    }

    void AvoidSticking()
    {
        if ((leftCast && !leftCast.collider.GetComponentInParent<PlayerPiece>()) || (rightCast && !rightCast.collider.GetComponentInParent<PlayerPiece>()))
        {
            parentMovement.transform.position = new Vector3(Mathf.Round(parentMovement.transform.position.x * 2) / 2, parentMovement.transform.position.y, parentMovement.transform.position.z);
        }
        if ((downCast1 && !downCast1.collider.GetComponentInParent<PlayerPiece>()) || (downCast2 && !downCast2.collider.GetComponentInParent<PlayerPiece>() || (upCast1 && !upCast1.collider.GetComponentInParent<PlayerPiece>()) || (upCast2 && !upCast2.collider.GetComponentInParent<PlayerPiece>())))
        {
            parentMovement.transform.position = new Vector3(parentMovement.transform.position.x, Mathf.Round(parentMovement.transform.position.y * 2) / 2, parentMovement.transform.position.z);
        }
    }
    /// <summary>
    /// recasts orthagonals
    /// </summary>
    public void UpdateNeighbours()
    {
        //leftCast = checkCast(Vector3.left, castLength);
        //rightCast = checkCast(Vector3.right, castLength);
        //upCast1 = checkCast(Vector3.up, castLength);
        //upCast2 = checkCast(Vector3.up, castLength);
        //downCast1 = checkCast(Vector3.down, castLength);
        //downCast2 = checkCast(Vector3.down, castLength);
        leftCast = Physics2D.Raycast(transform.position, Vector2.left, castLength, solidLayers);
        rightCast = Physics2D.Raycast(transform.position, Vector2.right, castLength, solidLayers);

        upCast1 = Physics2D.Raycast(transform.position - Vector3.right * (downCastWidth), Vector3.up, castLength, solidLayers);
        upCast2 = Physics2D.Raycast(transform.position + Vector3.right * (downCastWidth), Vector3.up, castLength, solidLayers);

        downCast1 = Physics2D.Raycast(transform.position - Vector3.right * downCastWidth, -Vector3.up, castLength, solidLayers);

        downCast2 = Physics2D.Raycast(transform.position + Vector3.right * downCastWidth, -Vector3.up, castLength, solidLayers);
    }
    //RaycastHit2D checkCast(Vector3 direction, float dist)
    //{
    //    RaycastHit2D[] tempList = new RaycastHit2D[1];
    //    myOwnCollider.Raycast(direction, tempList, dist, solidLayers);
    //    return tempList[0];

    //}

    /// <summary>
    /// checks both downcasts to determine whether the block is grounded or not
    /// </summary>
    /// <returns>true if grounded, false if not</returns>
    public bool IsGrounded()
    {

        return (CheckCastForJumpables(downCast1) || CheckCastForJumpables(downCast2));
    }
    public bool IsRunningIntoWall(float directionOfRunning)
    {
        if (directionOfRunning > 0)
        {
            return CheckCastForJumpables(rightCast);
        }
        else if (directionOfRunning < 0)
        {
            return CheckCastForJumpables(leftCast);
        }
        else
        {
            return false;
        }
    }

    public bool IsGroundedOnOwnBoy()
    {
        return (CheckCastForOwnBoy(Physics2D.Raycast(transform.position, -Vector3.up, castLength + .2f, solidLayers)));
    }
    /// <summary>
    /// checks if a cast is hitting something you can jump off of
    /// </summary>
    /// <param name="cast">cast to check</param>
    /// <returns>returns true if it is, false if it isnt</returns>
    public bool CheckCastForJumpables(RaycastHit2D cast)
    {
        if (cast)
        {

            if (cast.collider.GetComponentInParent<PlayerPiece>())
            {
                Movement collidedPlayerMovement = cast.collider.GetComponentInParent<PlayerPiece>().parentMovement;
                if (collidedPlayerMovement != parentMovement)
                {
                    if (collidedPlayerMovement.grounded || butterflyJump)
                    {
                        //Debug.DrawLine(transform.position, cast.point, Color.blue);
                        //you are ontop of a grounded player (made of wood I guess)
                        currentGround = groundMaterial.wood;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //Debug.DrawLine(transform.position, cast.point, Color.red);
                    return false;
                }
            }

            else //hitting ground
            {
                DetermineFloorMat(cast);
                //Debug.DrawLine(transform.position, cast.point, Color.green);
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// tries to figure out floor material based on ground collision tag
    /// </summary>
    /// <param name="tag"></param>
    void DetermineFloorMat(RaycastHit2D cast)
    {
        Tilemap tileMapHit = cast.collider.GetComponent<Tilemap>();
        if (tileMapHit)
        {
            //once we know that it hit a tilemap, decide which tilemap to *actually* pull the sound from, based on sort order
            Vector2 tilePoint = cast.point + Vector2.down;


            TileBase tileHit = tileMapHit.GetTile(tileMapHit.WorldToCell(tilePoint)); //default it does theese

            //grass exception:
            for (int i = 0; i < parentMovement.allTileMapsInScene.Count; i++)
            {


                tileHit = parentMovement.allTileMapsInScene[i].GetTile(parentMovement.allTileMapsInScene[i].WorldToCell(tilePoint));
                if (tileHit)
                {
                    break; //break the loop on the top tile that you hit
                }
            }

            //end of grass exception
            if (tileHit)
            {
                if (parentMovement.groundMatSettings.dirtTiles.Contains(tileHit))
                {
                    currentGround = groundMaterial.dirt;
                }
                else if (parentMovement.groundMatSettings.woodTiles.Contains(tileHit))
                {
                    currentGround = groundMaterial.wood;
                }
                else if (parentMovement.groundMatSettings.snowTiles.Contains(tileHit))
                {
                    currentGround = groundMaterial.snow;
                }
                else if (parentMovement.groundMatSettings.grassTiles.Contains(tileHit))
                {
                    currentGround = groundMaterial.grass;
                }
                else if (parentMovement.groundMatSettings.rockTiles.Contains(tileHit))
                {
                    currentGround = groundMaterial.rock;
                }
                else if (parentMovement.groundMatSettings.leafTiles.Contains(tileHit))
                {
                    currentGround = groundMaterial.leaves;
                }
                else
                {
                    //default to wood I guess
                    currentGround = groundMaterial.wood;
                }

            }
        }

    }
    public bool CheckCastForOwnBoy(RaycastHit2D cast)
    {
        if (cast)
        {

            if (cast.collider.GetComponentInParent<PlayerPiece>())
            {
                Transform collidedPlayerTransform = cast.collider.transform.root;
                if (collidedPlayerTransform == transform.root)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    
        
    }

    #region Horizontal Suck
    private float confinedSpaceYCastUp = .6f, confinedSpaceYCastDown = .6f;
    static float confinedSpaceYCastMaxIncrement = .8f;
    static float confinedSpaceYCastMinIncrement = .6f;
    float yVelocityThreshold = 14.5f;
    [SerializeField] LayerMask confinedSpaceLayerMask;
    [SerializeField] LayerMask playerPieceLayerMask;

    public bool CheckCastForConfinedSpaceV(Vector2 castDirection, float yVelocity)
    {
        if (!IsGrounded()) //if the player isn't grounded
        {
            Vector2 spaceChecked = (Vector2)transform.position + (castDirection * (castLength + .1f)); //our position checking if there's an empty space

            if (!Physics2D.OverlapPoint(spaceChecked, solidLayers)) //REPLACE WITH NON ALLOC IF POSSIBLE
            {
                
                //recursively determine the number of pieces above/below this piece
                //int activePiecesUp = GetActivePlayerPiecesInDirection(Vector2.up, transform.position, 1f, 0);
                //int activePiecesDown = GetActivePlayerPiecesInDirection(Vector2.down, transform.position, 1f, 0);

                //calculate size of casts depending on the active pieces
                confinedSpaceYCastUp = activePiecesUp + CalculateConfinedYCast(yVelocity);
                confinedSpaceYCastDown = activePiecesDown + CalculateConfinedYCast(yVelocity);

                RaycastHit2D spaceCastUp;
                RaycastHit2D spaceCastDown;

                if (sucksWithPlayers)
                {
                    spaceCastUp = Physics2D.Raycast(spaceChecked, Vector2.up, confinedSpaceYCastUp, solidLayers);
                    spaceCastDown = Physics2D.Raycast(spaceChecked, Vector2.down, confinedSpaceYCastDown, solidLayers);
                }
                else
                {
                    spaceCastUp = Physics2D.Raycast(spaceChecked, Vector2.up, confinedSpaceYCastUp, confinedSpaceLayerMask);
                    spaceCastDown = Physics2D.Raycast(spaceChecked, Vector2.down, confinedSpaceYCastDown, confinedSpaceLayerMask);
                }

                //Debug.DrawLine(spaceChecked + new Vector2(.1f, 0f), spaceChecked + Vector2.up * confinedSpaceYCastUp, Color.blue);
                //Debug.DrawLine(spaceChecked - new Vector2(.1f, 0f), spaceChecked + Vector2.down * confinedSpaceYCastDown, Color.magenta);

                float activePiecesUpAndDown = 1 + activePiecesUp + activePiecesDown;
                float openSpaceForPieces = 0;

                if (spaceCastUp && spaceCastDown) //if both raycasts hit a block
                {
                    if (spaceCastUp.collider.transform.root != transform.root && spaceCastDown.collider.transform.root != transform.root)
                    {
                        openSpaceForPieces = Mathf.RoundToInt(Vector2.Distance(spaceCastUp.point, spaceCastDown.point));
                        //change back to ceil if issues persist
                    }
                }

                if (activePiecesUpAndDown == openSpaceForPieces) //check the open space for the cast is equal to the amount of pieces trying to get into it
                {
                    if (CheckCastForJumpables(spaceCastUp) && CheckCastForJumpables(spaceCastDown))
                    {
                        //Debug.Log(gameObject.name + "open space for pieces: " + openSpaceForPieces);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    float CalculateConfinedYCast(float yVelocity)
    {
        float calculatedConfinedSpaceSize;

        if (Mathf.Abs(yVelocity) >= yVelocityThreshold)
        {
            calculatedConfinedSpaceSize = confinedSpaceYCastMaxIncrement;
        }
        else
        {
            calculatedConfinedSpaceSize = confinedSpaceYCastMinIncrement;
        }

        return calculatedConfinedSpaceSize;
    }
    #endregion

    #region Vertical Suck
    private float confinedSpaceYCastLeft = .625f, confinedSpaceYCastRight = .625f;
    public bool CheckCastForConfinedSpaceH(Vector2 castDirection, float xVelocity)
    {
        Vector2 spaceChecked = (Vector2)transform.position + (castDirection * (castLength + .1f)); //our position checking if there's an empty space

        if (!Physics2D.OverlapPoint(spaceChecked, solidLayers)) //if not hitting ground in the downcast
        {
            //recursively determine the number of pieces to the left/right of this piece
            //int activePiecesLeft = GetActivePlayerPiecesInDirection(Vector2.left, transform.position, 1f, 0);
            //int activePiecesRight = GetActivePlayerPiecesInDirection(Vector2.right, transform.position, 1f, 0);

            //calculate size of casts depending on the active pieces
            confinedSpaceYCastLeft = activePiecesLeft + CalculateConfinedXCast(0);
            confinedSpaceYCastRight = activePiecesRight + CalculateConfinedXCast(0);
            RaycastHit2D spaceCastLeft;
            RaycastHit2D spaceCastRight;

            if (sucksWithPlayers)
            {
                spaceCastLeft = Physics2D.Raycast(spaceChecked, Vector2.left, confinedSpaceYCastLeft, solidLayers);
                spaceCastRight = Physics2D.Raycast(spaceChecked, Vector2.right, confinedSpaceYCastRight, solidLayers);
            }
            else
            {
                spaceCastLeft = Physics2D.Raycast(spaceChecked, Vector2.left, confinedSpaceYCastLeft, confinedSpaceLayerMask);
                spaceCastRight = Physics2D.Raycast(spaceChecked, Vector2.right, confinedSpaceYCastRight, confinedSpaceLayerMask);
            }

            //Debug.DrawLine(spaceChecked, spaceChecked + Vector2.left * confinedSpaceYCastLeft, Color.blue);
            //Debug.DrawLine(spaceChecked, spaceChecked + Vector2.right * confinedSpaceYCastRight, Color.magenta);

            float activePiecesLeftAndRight = 1 + activePiecesLeft + activePiecesRight;
            float openSpaceForPieces = 0;

            if (spaceCastLeft && spaceCastRight) //if both raycasts hit a block, check the size of the space
            {
                if (spaceCastLeft.collider.transform.root != transform.root && spaceCastRight.collider.transform.root != transform.root)
                {
                    openSpaceForPieces = Mathf.CeilToInt(Vector2.Distance(spaceCastLeft.point, spaceCastRight.point));
                }
            }

            if (activePiecesLeftAndRight == openSpaceForPieces) //check the open space for the cast is equal to the amount of pieces trying to get into it - MUST BE THE SIZE OF THE HOLE
            {
                if (CheckCastForJumpables(spaceCastLeft) && CheckCastForJumpables(spaceCastRight))
                {
                    return true;
                }
            }
        }
        return false;
    }

    float CalculateConfinedXCast(float xVelocity) //change to accomodate xVelocity
    {
        float calculatedConfinedSpaceSize = .7f;
        if (xVelocity > 0) //up suc
        {
            calculatedConfinedSpaceSize = .5f;
        }
        else if (xVelocity <= 0) //down zuc
        {
            calculatedConfinedSpaceSize = .7f;
        }
        return calculatedConfinedSpaceSize;
    }
    #endregion


    public void CalculateActivePiecesInRange()
    {
        activePiecesUp = GetActivePlayerPiecesInDirection(Vector2.up, transform.position, 1f, 0);
        activePiecesDown = GetActivePlayerPiecesInDirection(Vector2.down, transform.position, 1f, 0);
        activePiecesLeft = GetActivePlayerPiecesInDirection(Vector2.left, transform.position, 1f, 0);
        activePiecesRight = GetActivePlayerPiecesInDirection(Vector2.right, transform.position, 1f, 0);
    }

    int GetActivePlayerPiecesInDirection(Vector2 direction, Vector2 castPosition, float castDistance, int currentPieceCount)
    {
        if (GetActivePiecesInRange(direction, castPosition, castDistance, playerPieceLayerMask) <= 0)
        {
            return currentPieceCount; //return the amount of pieces
        }

        currentPieceCount++;
        Vector2 newPosition = castPosition + (direction * castDistance); //get the newPosition of the next recursive cast
        return GetActivePlayerPiecesInDirection(direction, newPosition, castDistance, currentPieceCount); //recursion
    }


    public int GetActivePiecesInRange(Vector2 direction, Vector2 position, float castDistance, LayerMask maskForPieces)
    {
        int numberOfPieces = 0;
        RaycastHit2D castInDirection = Physics2D.Raycast(position, direction, castDistance, maskForPieces);
        if (castInDirection)
        {
            if (castInDirection.collider.transform.root == transform.root) //if from the same parent
            {
                if (castInDirection.collider.gameObject.activeInHierarchy)
                {
                    numberOfPieces++;
                }
            }
        }
        return numberOfPieces;
    }

    public PlayerPiece GetBlockThatGrabsMe()
    {
        if (transform.parent.GetChild(0) == transform)
        {
            return null;
        }


        for (int i = 0; i < fourDirections.Length; i++)
        {
            RaycastHit2D castInDirection;
            castInDirection = Physics2D.Raycast(transform.position, fourDirections[i], 1f, playerPieceLayerMask);
            if (castInDirection && castInDirection.transform.root == transform.root)
            {
                PlayerPiece tempPlayerPieceComp = castInDirection.collider.gameObject.GetComponentInParent<PlayerPiece>();
                if (tempPlayerPieceComp.transform.GetSiblingIndex() < transform.GetSiblingIndex())
                {
                    directionOfPieceThatGrabsMe = fourDirections[i];
                    return tempPlayerPieceComp;
                }
            }
        }
        return null;
    }

}