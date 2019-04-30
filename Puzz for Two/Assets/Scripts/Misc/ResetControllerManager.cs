using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetControllerManager : MonoBehaviour
{

    NewControllerManager instance;

    // Use this for initialization
    void Start()
    {
        if (NewControllerManager.instance != null)
        {
            instance = NewControllerManager.instance;
        }
    }


    public void ResetControllerManagerInput()
    {
        if (instance != null)
        {
            instance.ResetBothIncontrolThings();
            instance.controllerTypeInputted = CurrentControllerSetup.Undecided;
        }
    }
}
