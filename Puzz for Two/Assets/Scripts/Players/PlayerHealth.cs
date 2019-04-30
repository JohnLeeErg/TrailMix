using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class PlayerHealth : IHealth
{
    public Transform playerBlockParent;
    public GameObject highlightIndicator;
    public GameObject rotationIndicatorIcon;
    [Header("Catching")]
    [SerializeField] bool autoCatch;
    public string catchInput;
    SpriteRenderer highlightSprite;
    Movement MovementComp;
    Color highlightColor;
    [SerializeField] Color blockedColor;
    public PlayerPiece playerPieceLookingForBoy;
    public PlayerPiece pieceForThrow;
    public FaceProfile latestFaceProfile;
    // Stopping Variables
    bool catchingIsStopped = false;

    // Create FMOD Sound Effect Variables
    [Header("FMOD Audio Events")]
    [FMODUnity.EventRef]
    public string getPiecesound;
    FMOD.Studio.EventInstance fModGetPieceEvent;

    void Start()
    {
        //maxHealth = playerBlockParent.childCount;
        MovementComp = GetComponent<Movement>();
        BlockHealthCheck();
        highlightSprite = highlightIndicator.GetComponent<SpriteRenderer>();
        highlightColor = highlightSprite.color;
        if (health < maxHealth)
        {
            playerPieceLookingForBoy = MovementComp.pieces[(int)health].GetBlockThatGrabsMe();
        }

        SetHighlight();

        // Make the throwing indicator invisable before the game starts
        if (health == 1 && rotationIndicatorIcon.activeSelf == true)
        {
            rotationIndicatorIcon.SetActive(false);
        }
        else if (health > 1 && rotationIndicatorIcon.activeSelf == false)
        {
            rotationIndicatorIcon.SetActive(true);
        }

        //instanciate FMOD sound variables/events
        fModGetPieceEvent = FMODUnity.RuntimeManager.CreateInstance(getPiecesound);
    }
    /// <summary>
    /// checks whether there are any blocks left to throw, and lowers the health if possible
    /// </summary>
    public void OnThrow()
    {
        if (health > 1)
        {
            health--;
            BlockHealthCheck();
            SetHighlight();
        }

        if (health <= 1)
        {
            rotationIndicatorIcon.SetActive(false);
        }
    }

    public override void TakeDamage(float damageVal)
    {
        base.TakeDamage(damageVal);

        BlockHealthCheck();
        SetHighlight();

        if (health <= 1)
        {
            rotationIndicatorIcon.SetActive(false);
        }
    }

    public override void OnHeartCollide(float heartVal, DetachedPiece thisHeart)
    {
        if (catchingIsStopped == false)
        {
            if (NextPosFree())
            {
                health += heartVal;
                if (health > maxHealth)
                {
                    health = maxHealth;
                }
                BlockHealthCheck();
                SetHighlight();
                thisHeart.collidedOnce = true;

                LookAtObject newBoiFace = playerBlockParent.GetChild((int)health - 1).GetComponentInChildren<LookAtObject>();
                LookAtObject looseBoiFace = thisHeart.GetComponentInChildren<LookAtObject>();
                newBoiFace.randomizeFace = false;
                if (!newBoiFace.alreadyGotSpecialFace)
                {
                    newBoiFace.GetComponent<SpriteRenderer>().sprite = looseBoiFace.GetComponent<SpriteRenderer>().sprite;

                }
                else if (latestFaceProfile != null)
                {
                    if (!looseBoiFace.alreadyGotSpecialFace)
                    {
                        newBoiFace.SetRandomFace(latestFaceProfile); //asign the lipstick on attachment now i guess

                    }
                    else
                    {
                        newBoiFace.GetComponent<SpriteRenderer>().sprite = looseBoiFace.GetComponent<SpriteRenderer>().sprite;
                    }
                }

                Destroy(thisHeart.gameObject);

                // play Get Piece sound
                fModGetPieceEvent.start();

                if (health > 1 && rotationIndicatorIcon.activeSelf == false)
                {
                    rotationIndicatorIcon.SetActive(true);
                }

                PlayerPieceAnimatorManager animationManager = GetComponent<PlayerPieceAnimatorManager>();
                if (animationManager)
                {
                    //animationManager.ActivateAnimatorTrigger("Catch");
                }
            }
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    new void Update()
    {
        base.Update();

        //if (!autoCatch && health < maxHealth)
        //{
        //    if (MovementComp.playerInput.catchAction.WasPressed)
        //    {
        //        highlightSprite.enabled = true;//this can be replaced by fancier visuals
        //    }
        //    if (MovementComp.playerInput.catchAction.WasReleased)
        //    {
        //        highlightSprite.enabled = false;
        //    }
        //}
        if (NextPosFree())
        {
            highlightSprite.color = highlightColor;
        }
        else
        {
            highlightSprite.color = blockedColor;
        }

        // DEBUG - for setting health in inspector
        //if (health > 0)
        //{
        //    BlockHealthCheck();
        //    SetHighlight();
        //}

    }

    public void StopCatching()
    {
        catchingIsStopped = true;
    }

    public void StartCatching()
    {
        catchingIsStopped = false;
    }

    /// <summary>
    /// sets the highlight visual to the next position available or hides it if it's not
    /// </summary>
    void SetHighlight()
    {
        if (!catchingIsStopped)
        {
            if (health >= maxHealth || health <= 0)
            {
                highlightIndicator.SetActive(false); // Set the indicator to invisable if the player is dead or if its at max health
                playerPieceLookingForBoy = null;
                pieceForThrow = MovementComp.pieces[(int)health - 1].GetBlockThatGrabsMe();
            }
            else
            {
                highlightIndicator.SetActive(true); // set the indicator to be visable of there the player's health is not at max
                Transform nextBlock = playerBlockParent.GetChild((int)health);
                highlightIndicator.transform.position = nextBlock.position; // set the highlight to be placed at the position of the next block
                playerPieceLookingForBoy = MovementComp.pieces[(int)health].GetBlockThatGrabsMe();
                pieceForThrow = MovementComp.pieces[(int)health - 1].GetBlockThatGrabsMe();
                ChangeHighlightSprite();
            }
        }
        else
        {
            highlightIndicator.SetActive(false);
        }
    }
    /// <summary>
    /// checks whether the space the next block would occupy is free or not
    /// </summary>
    /// <returns>returns true if free, false if not</returns>
    public bool NextPosFree()
    {
        if ((!autoCatch && !MovementComp.playerInput.catchAction.IsPressed)) //you need to either be holding the catch button or have autocatch turned on 
        {
            return false;
        }

        Vector2 nextPoint = highlightIndicator.transform.position;
        //Debug.DrawLine(transform.position, nextPoint, Color.green, 100f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(nextPoint, new Vector2(.4f, .4f), 0);

        bool free = true;
        foreach (Collider2D eachHit in hits)
        {
            if (eachHit.gameObject.layer == LayerMask.NameToLayer("Player") || eachHit.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                //only care about walls and players
                free = false;
                //flash some sort of visual warning
                //Instantiate(errorEffectPrefab, playerBlockParent.GetChild((int)health - 1).transform.position,Quaternion.identity);
            }
        }
        return free;
    }
    private void BlockHealthCheck()
    {
        // Go through all of the blocks that are equal to the health and set them to active
        for (int i = 1; i < health; i++)
        {
            playerBlockParent.GetChild(i).gameObject.SetActive(true);
        }

        // Go through all of the blocks past the current health value and se them to false 
        for (int i = (int)health; i < (maxHealth); i++)
        {
            playerBlockParent.GetChild(i).gameObject.SetActive(false);
        }

        //do block check recursion
        if (MovementComp != null && MovementComp.pieces.Count > 0)
        {
            for (int i = 0; i < MovementComp.pieces.Count; i++)
            {
                if (MovementComp.pieces[i].gameObject.activeInHierarchy)
                {
                    MovementComp.pieces[i].CalculateActivePiecesInRange();
                }
            }
        }
    }

    void ChangeHighlightSprite()
    {
        if (((int)health + 1) <= maxHealth)
        {
            if (MovementComp.pieces[(int)health].highlightSprite == null)
            {
                highlightSprite.sprite = MovementComp.pieceSpriteRenderers[(int)health].sprite;
            }
            else
            {
                highlightSprite.sprite = MovementComp.pieces[(int)health].highlightSprite;
            }
        }
    }
}
