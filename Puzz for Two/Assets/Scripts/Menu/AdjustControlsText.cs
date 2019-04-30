using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustControlsText : MonoBehaviour
{
    [SerializeField] GameObject keyObj, conchObj, bothObj, p2Keyboard;
    // Use this for initialization
    void Start()
    {
        
    }
    public void swapText()
    {
        if (NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.OnePlayerController ||
            NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController)
        {
            conchObj.SetActive(true);
            keyObj.SetActive(false);
            bothObj.SetActive(false);
            p2Keyboard.SetActive(false);
        }
        else if (NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard ||
            NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.Undecided)
        {
            conchObj.SetActive(false);
            keyObj.SetActive(false);
            bothObj.SetActive(true);
            p2Keyboard.SetActive(false);
        }
        else if (NewControllerManager.instance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
        {
            conchObj.SetActive(false);
            keyObj.SetActive(false);
            bothObj.SetActive(false);
            p2Keyboard.SetActive(true);
        }
        else
        {
            conchObj.SetActive(false);
            keyObj.SetActive(true);
            bothObj.SetActive(false);
            p2Keyboard.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {

        swapText();
    }
}
