using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPieceAnimatorManager : MonoBehaviour
{
    [SerializeField] List<Animator> playerPieceAnimators;
    [SerializeField] Animator highlightAnimator;
    public Dictionary<Animator, PlayerPiece> playerPieceDictionary = new Dictionary<Animator, PlayerPiece>();
    public Dictionary<Animator, Animator> playerFaceDictionary = new Dictionary<Animator, Animator>();

    Movement playerMoveComp;
    PlayerHealth playerHealthComp;
    PlayerThrowing playerThrowComp;
    public GameObject otherPlayer;
    public PlayerPieceAnimatorManager otherPlayerAnimatorManager;
    bool grabCheck;
    [HideInInspector] public bool automatedThrow = false;
    // Use this for initialization
    public void Start()
    {
        playerMoveComp = GetComponent<Movement>();
        playerHealthComp = GetComponent<PlayerHealth>();
        playerThrowComp = GetComponent<PlayerThrowing>();
        otherPlayer = playerMoveComp.otherPlayer.gameObject;
        otherPlayerAnimatorManager = otherPlayer.GetComponent<PlayerPieceAnimatorManager>();
        for (int i = 0; i < playerMoveComp.pieces.Count; i++)
        {
            Animator pieceAnimator = playerMoveComp.pieces[i].gameObject.GetComponent<Animator>();
            if (pieceAnimator)
            {
                playerPieceAnimators.Add(pieceAnimator);
                playerPieceDictionary.Add(pieceAnimator, playerMoveComp.pieces[i]); //add the pieces and their animators to a dictionary for ease of use
                playerFaceDictionary.Add(pieceAnimator, pieceAnimator.transform.GetComponentInChildren<Animator>()); //add face animators

                LookAtObject tempLookComp = pieceAnimator.gameObject.GetComponentInChildren<LookAtObject>(); //get the look at object component on the face and set the target look to the other player
                if (tempLookComp)
                {
                    tempLookComp.targetLook = otherPlayer.transform;
                }
            }
        }

        playerPieceAnimators[0].SetBool("baseCamper", true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimatorValues();
    }

    public void UpdateAnimatorValues()
    {
        for (int i = 0; i < playerPieceAnimators.Count; i++)
        {
            Animator pieceAnimator = playerPieceAnimators[i];
            if (pieceAnimator.gameObject.activeInHierarchy && playerPieceDictionary.ContainsKey(pieceAnimator))
            {
                pieceAnimator.SetFloat("horizontalInput", playerMoveComp.movementInput.x);
                pieceAnimator.SetFloat("horizontalAbsInput", Mathf.Abs(playerMoveComp.movementInput.x));
                pieceAnimator.SetBool("rising", playerMoveComp.rising);
                pieceAnimator.SetBool("isGrounded", playerPieceDictionary[pieceAnimator].IsGrounded());
                pieceAnimator.SetBool("isGroundedOnBoy", playerPieceDictionary[pieceAnimator].IsGroundedOnOwnBoy());
                pieceAnimator.SetBool("parentGrounded", playerMoveComp.grounded);
                pieceAnimator.SetBool("catching", playerMoveComp.playerInput.catchAction.IsPressed);
                pieceAnimator.SetBool("catchBlocked", !playerHealthComp.NextPosFree());
                if (playerHealthComp.playerPieceLookingForBoy == playerPieceDictionary[pieceAnimator])
                {
                    pieceAnimator.SetBool("lookingForBoy", true);
                }
                else
                {
                    pieceAnimator.SetBool("lookingForBoy", false);
                }


                if (playerHealthComp.health > 1 && playerMoveComp.playerInput.throwAction.WasPressed)
                {
                    pieceAnimator.SetFloat("throwAxisX", playerThrowComp.rotationIndicatorInput.x);
                    pieceAnimator.SetFloat("throwAxisY", playerThrowComp.rotationIndicatorInput.y);
                    playerHealthComp.pieceForThrow.gameObject.GetComponent<Animator>().SetTrigger("Throw");
                }

                if (automatedThrow == true)
                {
                    pieceAnimator.SetFloat("throwAxisX", 0);
                    pieceAnimator.SetFloat("throwAxisY", 1);
                    playerPieceAnimators[0].SetTrigger("Throw");
                    automatedThrow = false;
                }
            }

            if (pieceAnimator.gameObject.activeSelf)
            {
                pieceAnimator.Update(0);
            }
        }

        if (highlightAnimator && highlightAnimator.gameObject.activeInHierarchy)
        {
            highlightAnimator.SetBool("catchBlocked", !playerHealthComp.NextPosFree());
            highlightAnimator.SetBool("catching", playerMoveComp.playerInput.catchAction.IsPressed);
        }
    }

    //used to activate the animator triggers
    public void ActivateAnimatorTrigger(string triggerName)
    {
        for (int i = 0; i < playerPieceAnimators.Count; i++)
        {
            Animator pieceAnimator = playerPieceAnimators[i];
            if (pieceAnimator.gameObject.activeInHierarchy && playerPieceDictionary.ContainsKey(pieceAnimator))
            {
                pieceAnimator.SetTrigger(triggerName);
            }
        }
    }

    #region Face Stuff

    #endregion

    public void FixPlayerSprites()
    {
        if (!Application.isPlaying)
        {
            playerMoveComp = GetComponent<Movement>();
            playerHealthComp = GetComponent<PlayerHealth>();
            playerThrowComp = GetComponent<PlayerThrowing>();
            List<PlayerPiece> playerPieces = new List<PlayerPiece>();
            playerPieces.AddRange(GetComponentsInChildren<PlayerPiece>());
            for (int i = 0; i < playerPieces.Count; i++)
            {
                playerPieces[i].GetComponent<Animator>().SetBool("isGrounded", true);
                playerPieces[i].GetComponent<Animator>().Update(Time.deltaTime);
                playerPieces[i].GetComponentInChildren<LookAtObject>().SetToPivotPosition();
            }
        }
    }
}
