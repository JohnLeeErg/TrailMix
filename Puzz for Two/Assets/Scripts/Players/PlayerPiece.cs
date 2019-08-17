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
    [HideInInspector] public PlayerHealth parentHealth;
    [SerializeField] LayerMask solidLayers;
    BoxCollider2D myOwnCollider;
    public PlayerPiece pieceThatGrabsMe;
    public Vector2 directionOfPieceThatGrabsMe;
    public groundMaterial currentGround;
    public Sprite highlightSprite;
    public int activePiecesUp, activePiecesDown, activePiecesLeft, activePiecesRight;
    public Vector2 furthestPieceUp, furthestPieceDown, furthestPieceLeft, furthestPieceRight;
    [SerializeField] bool manuallySort;
    Vector2[] fourDirections = new Vector2[4] { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
    private void Awake()
    {
        parentHealth = GetComponentInParent<PlayerHealth>();
    }
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

    public bool CheckCastForConfinedSpaceV(Vector2 castDirection, float yVelocity, bool advancedCheck)
    {
        if (!IsGrounded()) //if the player isn't grounded
        {
            Vector2 spaceChecked = (Vector2)transform.position + (castDirection * (castLength + .1f)); //our position checking if there's an empty space
            Vector2 advancedCheckUpShift = new Vector2(0, 0);
            Vector2 advancedCheckDownShift = new Vector2(0, 0);


            if (!Physics2D.OverlapPoint(spaceChecked, solidLayers)) //REPLACE WITH NON ALLOC IF POSSIBLE
            {

                //recursively determine the number of pieces above/below this piece
                //int activePiecesUp = GetActivePlayerPiecesInDirection(Vector2.up, transform.position, 1f, 0);
                //int activePiecesDown = GetActivePlayerPiecesInDirection(Vector2.down, transform.position, 1f, 0);

                //calculate size of casts depending on the active pieces
                if (!advancedCheck)
                {
                    confinedSpaceYCastUp = activePiecesUp + CalculateConfinedYCast(yVelocity);
                    confinedSpaceYCastDown = activePiecesDown + CalculateConfinedYCast(yVelocity);
                } else
                {
                    confinedSpaceYCastUp = CalculateConfinedYCast(yVelocity);
                    confinedSpaceYCastDown = CalculateConfinedYCast(yVelocity);
                }
                RaycastHit2D spaceCastUp;
                RaycastHit2D spaceCastDown;

                if (sucksWithPlayers)
                {
                    // if we're enabling this, it needs to be updated for advanced suck, see below
                    spaceCastUp = Physics2D.Raycast(spaceChecked, Vector2.up, confinedSpaceYCastUp, solidLayers);
                    spaceCastDown = Physics2D.Raycast(spaceChecked, Vector2.down, confinedSpaceYCastDown, solidLayers);
                }
                else
                {
                    if (!advancedCheck)
                    {
                        spaceCastUp = Physics2D.Raycast(spaceChecked, Vector2.up, confinedSpaceYCastUp, confinedSpaceLayerMask);
                        spaceCastDown = Physics2D.Raycast(spaceChecked, Vector2.down, confinedSpaceYCastDown, confinedSpaceLayerMask);
                    } else
                    {
                        // if advanced, account for pieces of different L/R positions (the "shifting operation")
                        if (castDirection.x > 0)
                        {
                            // right casting
                            if (furthestPieceUp == new Vector2(0, 0))
                            {
                                advancedCheckDownShift = furthestPieceRight;
                            }
                            else if (furthestPieceDown == new Vector2(0, 0))
                            {
                                advancedCheckUpShift = furthestPieceRight;
                            }
                            else
                            {
                                // this is not the topmost or bottommost piece. some other piece should handle it
                                return false;
                            }
                        }
                        else
                        {
                            // left casting
                            if (furthestPieceUp == new Vector2(0, 0))
                            {
                                advancedCheckDownShift = furthestPieceLeft;
                            }
                            else if (furthestPieceDown == new Vector2(0, 0))
                            {
                                advancedCheckUpShift = furthestPieceLeft;
                            }
                            else
                            {
                                // this is not the topmost or bottommost piece. some other piece should handle it
                                return false;
                            }
                        }

                        spaceCastUp = Physics2D.Raycast(spaceChecked+advancedCheckUpShift, Vector2.up, confinedSpaceYCastUp, confinedSpaceLayerMask);
                        spaceCastDown = Physics2D.Raycast(spaceChecked+advancedCheckDownShift, Vector2.down, confinedSpaceYCastDown, confinedSpaceLayerMask);
                    }
                }

                //Debug.DrawLine(spaceChecked + new Vector2(.1f, 0f), spaceChecked + Vector2.up * confinedSpaceYCastUp, Color.blue);
                //Debug.DrawLine(spaceChecked - new Vector2(.1f, 0f), spaceChecked + Vector2.down * confinedSpaceYCastDown, Color.magenta);

                float activePiecesUpAndDown = 1 + activePiecesUp + activePiecesDown;
                float openSpaceForPieces = 0;

                if (spaceCastUp && spaceCastDown) //if both raycasts hit a block
                {
                    if (spaceCastUp.collider.transform.root != transform.root && spaceCastDown.collider.transform.root != transform.root)
                    {
                        if (!advancedCheck)
                        {
                            openSpaceForPieces = Mathf.RoundToInt(Vector2.Distance(spaceCastUp.point, spaceCastDown.point));
                            //change back to ceil if issues persist
                        } else
                        {
                            // undo the shifting operation from earlier
                            openSpaceForPieces = Mathf.RoundToInt(Vector2.Distance(spaceCastUp.point - new Vector2(advancedCheckUpShift.x,0), spaceCastDown.point - new Vector2(advancedCheckDownShift.x,0)));
                            //change back to ceil if issues persist
                        }
                    }
                }
                if (!advancedCheck)
                {
                    if (activePiecesUpAndDown == openSpaceForPieces) //check the open space for the cast is equal to the amount of pieces trying to get into it
                    {
                        if (CheckCastForJumpables(spaceCastUp) && CheckCastForJumpables(spaceCastDown))
                        {
                            //Debug.Log(gameObject.name + "open space for pieces: " + openSpaceForPieces);
                            return true;
                        }
                    }
                } else
                {
                    if (Mathf.Abs(advancedCheckUpShift.y) + Mathf.Abs(advancedCheckDownShift.y) == openSpaceForPieces - 1) //check the open space for the cast is equal to the amount of pieces trying to get into it - MUST BE THE SIZE OF THE HOLE
                    {
                        if (CheckCastForJumpables(spaceCastUp) && CheckCastForJumpables(spaceCastDown))
                        {
                            return true;
                        }
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
    public bool CheckCastForConfinedSpaceH(Vector2 castDirection, float xVelocity, bool advancedCheck)
    {
        Vector2 spaceChecked = (Vector2)transform.position + (castDirection * (castLength + .1f)); //our position checking if there's an empty space
        Vector2 advancedCheckLeftShift = new Vector2(0, 0);
        Vector2 advancedCheckRightShift = new Vector2(0, 0);

        if (!Physics2D.OverlapPoint(spaceChecked, solidLayers)) //if not hitting ground in the downcast
        {
            //recursively determine the number of pieces to the left/right of this piece
            //int activePiecesLeft = GetActivePlayerPiecesInDirection(Vector2.left, transform.position, 1f, 0);
            //int activePiecesRight = GetActivePlayerPiecesInDirection(Vector2.right, transform.position, 1f, 0);

            //calculate size of casts depending on the active pieces
            if (!advancedCheck)
            {
                confinedSpaceYCastLeft = activePiecesLeft + CalculateConfinedXCast(0);
                confinedSpaceYCastRight = activePiecesRight + CalculateConfinedXCast(0);
            } else
            {
                confinedSpaceYCastLeft = CalculateConfinedXCast(0);
                confinedSpaceYCastRight = CalculateConfinedXCast(0);
            }
            RaycastHit2D spaceCastLeft;
            RaycastHit2D spaceCastRight;

            if (sucksWithPlayers)
            {
                // if we're enabling this, it needs to be updated for advanced suck, see below
                spaceCastLeft = Physics2D.Raycast(spaceChecked, Vector2.left, confinedSpaceYCastLeft, solidLayers);
                spaceCastRight = Physics2D.Raycast(spaceChecked, Vector2.right, confinedSpaceYCastRight, solidLayers);
            }
            else
            {
                if (!advancedCheck)
                {
                    spaceCastLeft = Physics2D.Raycast(spaceChecked, Vector2.left, confinedSpaceYCastLeft, confinedSpaceLayerMask);
                    spaceCastRight = Physics2D.Raycast(spaceChecked, Vector2.right, confinedSpaceYCastRight, confinedSpaceLayerMask);
                } else
                {
                    // if advanced, account for pieces of different elevations (the "shifting operation")
                    if (castDirection.y > 0)
                    {
                        //print(gameObject.name);
                        // up casting
                        if (furthestPieceLeft == new Vector2(0, 0))
                        {
                            advancedCheckRightShift = furthestPieceUp;
                        }
                        else if (furthestPieceRight == new Vector2(0, 0))
                        {
                            advancedCheckLeftShift = furthestPieceUp;
                        } else
                        {
                            // this is not the leftmost or rightmost piece. some other piece should handle it
                            return false;
                        }
                    } else
                    {
                        // down casting
                        if (furthestPieceLeft == new Vector2(0, 0))
                        {
                            advancedCheckRightShift = furthestPieceDown;
                        }
                        else if (furthestPieceRight == new Vector2(0, 0))
                        {
                            advancedCheckLeftShift = furthestPieceDown;
                        }
                        else
                        {
                            // this is not the leftmost or rightmost piece. some other piece should handle it
                            return false;
                        }
                    }
                    spaceCastLeft = Physics2D.Raycast(spaceChecked+ advancedCheckLeftShift, Vector2.left, confinedSpaceYCastLeft, confinedSpaceLayerMask);
                    spaceCastRight = Physics2D.Raycast(spaceChecked+ advancedCheckRightShift, Vector2.right, confinedSpaceYCastRight, confinedSpaceLayerMask);
                }
            }

            //Debug.DrawLine(spaceChecked, spaceChecked + Vector2.left * confinedSpaceYCastLeft, Color.blue);
            //Debug.DrawLine(spaceChecked, spaceChecked + Vector2.right * confinedSpaceYCastRight, Color.magenta);

            float activePiecesLeftAndRight = 1 + activePiecesLeft + activePiecesRight;
            float openSpaceForPieces = 0;

            if (spaceCastLeft && spaceCastRight) //if both raycasts hit a block, check the size of the space
            {
                if (spaceCastLeft.collider.transform.root != transform.root && spaceCastRight.collider.transform.root != transform.root)
                {
                    if (!advancedCheck)
                    {
                        openSpaceForPieces = Mathf.CeilToInt(Vector2.Distance(spaceCastLeft.point, spaceCastRight.point));
                    } else
                    {

                        //openSpaceForPieces = Mathf.CeilToInt(Mathf.Abs(spaceCastRight.point.x - spaceCastLeft.point.x));
                        // undo the shifting operation from earlier
                        openSpaceForPieces = Mathf.RoundToInt(Vector2.Distance(spaceCastLeft.point- new Vector2(0,advancedCheckLeftShift.y), spaceCastRight.point- new Vector2(0, advancedCheckRightShift.y)));
                    }
                }
            }
            if (!advancedCheck)
            {
                if (activePiecesLeftAndRight == openSpaceForPieces) //check the open space for the cast is equal to the amount of pieces trying to get into it - MUST BE THE SIZE OF THE HOLE
                {
                    if (CheckCastForJumpables(spaceCastLeft) && CheckCastForJumpables(spaceCastRight))
                    {
                        return true;
                    }
                }
            } else
            {
              
                if (Mathf.Abs(advancedCheckLeftShift.x)+ Mathf.Abs(advancedCheckRightShift.x) == openSpaceForPieces - 1) //check the open space for the cast is equal to the amount of pieces trying to get into it - MUST BE THE SIZE OF THE HOLE
                {
                    if (CheckCastForJumpables(spaceCastLeft) && CheckCastForJumpables(spaceCastRight))
                    {
                        print(gameObject.name);
                        return true;
                    }
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
        // if advanced succ may be necessary, calculate further bounds
        if (parentHealth.health > 2)
        {
            furthestPieceUp = GetFurthestPlayerPieceInDirection(Vector2.up, (Vector2)transform.position , parentHealth.health);
            furthestPieceDown = GetFurthestPlayerPieceInDirection(Vector2.down, (Vector2)transform.position, parentHealth.health);
            furthestPieceLeft = GetFurthestPlayerPieceInDirection(Vector2.left, (Vector2)transform.position, parentHealth.health);
            furthestPieceRight = GetFurthestPlayerPieceInDirection(Vector2.right, (Vector2)transform.position ,parentHealth.health);

            /*             
            furthestPieceUp = GetFurthestPlayerPieceInDirection(Vector2.up, (Vector2)transform.position + new Vector2(activePiecesUp,0), parentHealth.health);
            furthestPieceDown = GetFurthestPlayerPieceInDirection(Vector2.down, (Vector2)transform.position - new Vector2(activePiecesDown, 0), parentHealth.health);
            furthestPieceLeft = GetFurthestPlayerPieceInDirection(Vector2.left, (Vector2)transform.position - new Vector2(activePiecesLeft, 0), parentHealth.health);
            furthestPieceRight = GetFurthestPlayerPieceInDirection(Vector2.right, (Vector2)transform.position + new Vector2(activePiecesRight, 0), parentHealth.health);
            */
        }
    }

    /// <summary>
    /// 
    /// NOTES ON ADVANCED SUCK:
    /// 
    /// This is not a miracle cure for all cases! Advanced suck accounts for the furthest pieces left and right, or up and down, given that they're on different
    /// rows/columns. If the player health maxes at five, this shouldn't break anything. Six pieces and up will introduce problems. If we want to dip into that design territory
    /// there should really be a better system for calculating sucks.
    /// 
    /// Sorry I made your code a lot less readable michael :(
    /// 
    /// - Marty
    /// 
    /// UPDATE: 8/17
    /// 
    /// Um so the code below doesn't really do what it's billed to - (it no longer locates the furthest piece of a given direction), but it inexplicably works.
    /// If we need to fix anything, I'M REALLY SORRY. I think this needs a total rewrite at some point if we want to expand on this at all. Or at least start from
    /// michaels code and redo advanced suck, because this shit is really weird.
    /// 
    /// Originally there was one "scan" that would grab the actual right piece it's supposed to, but there were some cases where the one it really "wants" for succ
    /// purposes is an adjacent piece, so now it's stupidly more complicated, half the scans get the wrong piece, BUT in every case that MATTERS every piece has
    /// reference to the ones it should. There's a lot of redundant casting and general stupidity, but as far as I can tell this is the first thing that totally works :p
    /// 
    /// 
    /// </summary>


    Vector2 GetFurthestPlayerPieceInDirection(Vector2 direction, Vector2 sweepStartingPosition, float sweepCastLength)
    {
        // add one to the starting position 
        Vector2 sweepPosition = sweepStartingPosition + direction;
        Vector2 furthestPieceDetected = new Vector2(0, 0);

        // SWEEP: cast up and down by the total health (guaranteed to catch everything), & continue till the furthest piece is found
        while (true)
        {
            RaycastHit2D castInDirection1, castInDirection2, castInDirection3;
            while (true)
            {
                castInDirection1 = Physics2D.Raycast(sweepPosition, Quaternion.Euler(0, 0, 90) * direction, sweepCastLength, playerPieceLayerMask);
                if (castInDirection1)
                {
                    if (castInDirection1.collider.transform.root == transform.root) //if from the same parent
                    {
                        if (castInDirection1.collider.gameObject.activeInHierarchy)
                        {
                            if (furthestPieceDetected.magnitude < (castInDirection1.collider.transform.position - transform.position).magnitude)
                            {
                                furthestPieceDetected = castInDirection1.collider.transform.position - transform.position;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
                Vector2 positionIncrement = (Quaternion.Euler(0, 0, 90) * direction);
                sweepPosition += positionIncrement;
            } while (true)
            {
                 castInDirection2 = Physics2D.Raycast(sweepPosition, Quaternion.Euler(0, 0, -90) * direction, sweepCastLength, playerPieceLayerMask);
                if (castInDirection2)
                {
                    if (castInDirection2.collider.transform.root == transform.root) //if from the same parent
                    {
                        if (castInDirection2.collider.gameObject.activeInHierarchy)
                        {
                            if (furthestPieceDetected.magnitude < (castInDirection2.collider.transform.position - transform.position).magnitude)
                            {
                                furthestPieceDetected = castInDirection2.collider.transform.position - transform.position;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
                Vector2 positionIncrement = (Quaternion.Euler(0, 0, -90) * direction);
                sweepPosition += positionIncrement;
            } while (true)
            {
                castInDirection3 = Physics2D.Raycast(sweepPosition - direction, direction, 1f, playerPieceLayerMask);
                if (castInDirection3)
                {
                    if (castInDirection3.collider.transform.root == transform.root) //if from the same parent
                    {
                        if (castInDirection3.collider.gameObject.activeInHierarchy)
                        {
                            if (furthestPieceDetected.magnitude < (castInDirection3.collider.transform.position - transform.position).magnitude)
                            {
                                furthestPieceDetected = castInDirection3.collider.transform.position - transform.position;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
                Vector2 positionIncrement = (direction);
                sweepPosition += positionIncrement;
            }
                //sweepPosition += direction;

                //if (direction == new Vector2(0, -1))
                //print(gameObject.name + " found: " + furthestPieceDetected);

                if (castInDirection1.collider == null && castInDirection2.collider == null && castInDirection3.collider == null)
                {
                    break;
                }

            }
        

        if (direction == new Vector2(-1,0))
        print(gameObject.name + " furthest left: " + furthestPieceDetected);

        // if it doesn't catch anything, return nothin
        return furthestPieceDetected;
    }

    int GetActivePlayerPiecesInDirection(Vector2 direction, Vector2 castPosition, float castDistance, int currentPieceCount)
    {
        if (GetActivePiecesInRange(direction, castPosition, castDistance, playerPieceLayerMask) <= 0)
        {
            // advanced zucc goes here
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
 