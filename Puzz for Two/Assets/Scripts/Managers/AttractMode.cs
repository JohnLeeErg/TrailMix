using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AttractMode : MonoBehaviour
{

    //Attract Mode activation values
    float timeTillTransistion = 120;
    float countingTime = 0;
    KeyCode overrideTrasitionKey = KeyCode.Exclaim;

    [Header("Scene Name")]
    public string attractModeSceneName;

    NewControllerManager controllerManagerInstance;
    Movement player1Input, player2Input;

    Text debugTimerText;
    Animator debugTextAnimator;

    //public static AttractMode instance;
    bool isCalled = false;


    // Use this for initialization
    void Awake()
    {
        //debugTimerText = GetComponentInChildren<Text>();
        //debugTextAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (controllerManagerInstance == null)
        {
            controllerManagerInstance = NewControllerManager.instance;
            player1Input = controllerManagerInstance.player1Movement;
            player2Input = controllerManagerInstance.player1Movement;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NewControllerManager.instance && NewControllerManager.instance.demoMode)
        {
            //if (player1Input != controllerManagerInstance.player1Input)
            //{
            //    player1Input = controllerManagerInstance.player1Input;
            //}
            //if (player2Input != controllerManagerInstance.player2Input)
            //{
            //    player2Input = controllerManagerInstance.player2Input;
            //}

            //ChangeAttractModeLive();

            // if eaither of the players are moving then reset the timer
            if (player1Input.playerInput.horizontalAxis != 0 || player2Input.playerInput.horizontalAxis != 0
                || player1Input.playerInput.jumpAction == true || player2Input.playerInput.jumpAction == true
                || player1Input.playerInput.verticalAxis != 0 || player2Input.playerInput.verticalAxis != 0)
            {
                //print("MOVIN'");
                if (countingTime != 0)
                {
                    countingTime = 0;
                }
            }
            // if none of the players are moving then count down a timer that when reachs 0 transitions into the attract mode
            else if (player1Input.playerInput.horizontalAxis == 0 && player2Input.playerInput.horizontalAxis == 0
                && player1Input.playerInput.jumpAction == false && player2Input.playerInput.jumpAction == false
                && player1Input.playerInput.verticalAxis == 0 && player2Input.playerInput.verticalAxis == 0)
            {
                //print("NOT moving");
                CountTimeTillTransition();
            }
        }
    }

    void CountTimeTillTransition()
    {
        countingTime += Time.deltaTime;

        if (countingTime >= timeTillTransistion && isCalled == false)
        {
            isCalled = true;

            print("CHANGE LEVEL");
            StartCoroutine(TransitionScene());
        }
    }

    IEnumerator TransitionScene()
    {
        Fader.instance.FadeToColor(Color.black, 0.5f);

        yield return new WaitForSeconds(0.6f);

        SceneManager.LoadSceneAsync(attractModeSceneName, LoadSceneMode.Single);
    }

    void ChangeAttractModeLive()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            //Decrease Time
            timeTillTransistion -= 5;
            debugTimerText.text = timeTillTransistion.ToString();
            debugTextAnimator.SetTrigger("Activate");
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            //Increase Time
            timeTillTransistion += 5;
            debugTimerText.text = timeTillTransistion.ToString();
            debugTextAnimator.SetTrigger("Activate");
        }
    }
}
