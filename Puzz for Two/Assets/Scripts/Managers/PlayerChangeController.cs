using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

/// <summary>
/// debug control for switching controllers, currently only for movemeent will add shotting when shotting be done
/// </summary>
public class PlayerChangeController : MonoBehaviour
{

    [SerializeField] Movement p1Movement, p2Movement;
    [Range(.01f, 10)]
    int switcher;
    Color subGray;
    float howGrayIsIt = .8f;
    NewControllerManager controllerManagerInstance;
    bool darkened1, darkened2;
    private void Start()
    {
        subGray = new Color(Color.gray.r * howGrayIsIt, Color.gray.g * howGrayIsIt, Color.gray.b * howGrayIsIt, 0);
        if (!p1Movement)
        {
            p1Movement = GameObject.Find("Player 1").GetComponent<Movement>();

        }
        if (!p2Movement)
        {
            p2Movement = GameObject.Find("Player 2").GetComponent<Movement>();
        }
        Cursor.visible = false;
        //fade back in

        controllerManagerInstance = NewControllerManager.instance;
        if (controllerManagerInstance.swapPermitted && controllerManagerInstance.controllerTypeInputted != CurrentControllerSetup.Undecided)
        {
            foreach (PlayerPiece eachPiece in p1Movement.GetComponentsInChildren<PlayerPiece>())
            {

                if (darkened1)
                {
                    eachPiece.GetComponentInChildren<SpriteRenderer>().color += subGray * 20;
                    darkened1 = false;
                }
            }
            foreach (PlayerPiece eachPiece in p2Movement.GetComponentsInChildren<PlayerPiece>())
            {
                eachPiece.GetComponentInChildren<SpriteRenderer>().color -= subGray;
                darkened2 = true;
            }
        }
    }


    void ResetBothIncontrolThings()
    {
        p1Movement.playerInput.Device = null;
        p2Movement.playerInput.Device = null;
        p2Movement.playerInput.Enabled = false;
        p1Movement.playerInput.Enabled = false;
        //p1Movement.playerInput.Destroy();
        //p2Movement.playerInput.Destroy();
        p1Movement.playerInput = new MyCharacterActions();
        p2Movement.playerInput = new MyCharacterActions();

    }

    private void Update()
    {

        if (controllerManagerInstance.swapPermitted && (p1Movement.playerInput.switchAction.WasReleased || p2Movement.playerInput.switchAction.WasReleased))
        {
            Color swapperColor;
            SpriteRenderer p1Sp, p2Sp;
            for(int i = 0; i < p1Movement.transform.GetChild(0).childCount; i++)
            {
                p1Sp = p1Movement.transform.GetChild(0).GetChild(i).GetComponentInChildren<SpriteRenderer>();
                p2Sp = p2Movement.transform.GetChild(0).GetChild(i).GetComponentInChildren<SpriteRenderer>();

                swapperColor = p1Sp.color;
                p1Sp.color = p2Sp.color;
                p2Sp.color = swapperColor;
            }

            switcher = p1Movement.playerNumber;
            p1Movement.playerNumber = p2Movement.playerNumber;
            p2Movement.playerNumber = switcher;

            ResetBothIncontrolThings();
            controllerManagerInstance.swapped = !controllerManagerInstance.swapped;
            p1Movement.triedControls = false;
            p2Movement.triedControls = false;
        }
    }

}
