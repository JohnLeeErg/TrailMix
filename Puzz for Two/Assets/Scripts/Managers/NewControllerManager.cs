using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public enum CurrentControllerSetup
{
    Undecided,
    OnePlayerKeyboard,
    TwoPlayerOneKeyboard,
    OnePlayerController,
    TwoPlayerControllerAndKeyboard,
    TwoPlayerController
}

public class NewControllerManager : MonoBehaviour {

    [Header("Player Input")]
    public Movement player1Movement;
    public Movement player2Movement;
    public static NewControllerManager instance;
    public bool swapPermitted;
    public bool swapped = false;
    public bool demoMode; //doesnt actually have anything to do with controls, but determines many settings
    public CurrentControllerSetup controllerTypeInputted = CurrentControllerSetup.Undecided;

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetBothIncontrolThings()
    {
        print("reset stf");
        player1Movement.playerInput.Device = null;
        player1Movement.playerInput.Enabled = false;
        player1Movement.playerInput.Destroy();
        player1Movement.playerInput = new MyCharacterActions();

        player2Movement.playerInput.Device = null;
        player2Movement.playerInput.Enabled = false;
        player2Movement.playerInput.Destroy();
        player2Movement.playerInput = new MyCharacterActions();
    }
}
