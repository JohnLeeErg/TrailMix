using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSwapTutorial : MonoBehaviour
{
    [SerializeField] GameObject swapTut;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (NewControllerManager.instance.swapPermitted && NewControllerManager.instance.controllerTypeInputted!=CurrentControllerSetup.TwoPlayerOneKeyboard)
        {
            swapTut.SetActive(true);
        }
        else
        {
            swapTut.SetActive(false);
        }

    }
}
