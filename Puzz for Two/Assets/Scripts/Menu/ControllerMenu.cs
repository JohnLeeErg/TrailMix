using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ControllerMenu : MonoBehaviour
{

    NewControllerManager controllerManagerInstance;
    Movement p1Movement, p2Movement;

    public GameObject[] controllerOptions;

    // Use this for initialization
    void Start()
    {
        controllerManagerInstance = NewControllerManager.instance;
        InputManager.OnDeviceDetached += ControllerDetached;
        InputManager.OnDeviceAttached += ControllerAttached;

        // Find the Players
        if (!p1Movement)
        {
            p1Movement = GameObject.Find("Player 1").GetComponent<Movement>();
        }
        if (!p2Movement)
        {
            p2Movement = GameObject.Find("Player 2").GetComponent<Movement>();
        }
    }

    private void OnDisable()
    {
        InputManager.OnDeviceDetached -= ControllerDetached;
        InputManager.OnDeviceAttached -= ControllerAttached;
    }

    public void ControllerDetached(InputDevice device)
    {
        OnDettach();
    }

    public void ControllerAttached(InputDevice device)
    {
        OnAttach();
    }

    void ActivateOptionGraphic(int activeOption)
    {
        for (int i = 0; i < controllerOptions.Length; i++)
        {
            if (i == activeOption)
            {
                controllerOptions[i].SetActive(true);
            }
            else
            {
                controllerOptions[i].SetActive(false);
            }
        }
    }

    void OnDettach()
    {
        switch (controllerManagerInstance.controllerTypeInputted)
        {
            case CurrentControllerSetup.Undecided:
                // Do Nothing
                break;
            case CurrentControllerSetup.OnePlayerKeyboard:
                if (InputManager.Devices.Count == 0)
                {
                    if (controllerManagerInstance.swapped == false)
                    {
                        ActivateOptionGraphic(0);
                    }
                    else
                    {
                        ActivateOptionGraphic(5);
                    }
                }
                break;
            case CurrentControllerSetup.TwoPlayerOneKeyboard:
                if (InputManager.Devices.Count == 0)
                {
                    if (controllerManagerInstance.swapped == false)
                    {
                        ActivateOptionGraphic(0);
                    }
                    else
                    {
                        ActivateOptionGraphic(5);
                    }
                }
                break;
            case CurrentControllerSetup.OnePlayerController:
                if (InputManager.Devices.Count == 1)
                {
                    if (controllerManagerInstance.swapped == false)
                    {
                        ActivateOptionGraphic(1);
                    }
                    else
                    {
                        ActivateOptionGraphic(6);
                    }
                }
                break;
            case CurrentControllerSetup.TwoPlayerControllerAndKeyboard:
                if (InputManager.Devices.Count == 1)
                {
                    if (controllerManagerInstance.swapped == false)
                    {
                        ActivateOptionGraphic(2);
                    }
                    else
                    {
                        ActivateOptionGraphic(3);
                    }
                }
                break;
            case CurrentControllerSetup.TwoPlayerController:
                if (InputManager.Devices.Count == 2)
                {
                    ActivateOptionGraphic(4);
                }
                break;
        }
    }

    void OnAttach()
    {
        

        switch (controllerManagerInstance.controllerTypeInputted)
        {
            case CurrentControllerSetup.Undecided:
                // Do Nothing
                break;
            case CurrentControllerSetup.OnePlayerKeyboard:
                if (InputManager.Devices.Count == 0)
                {
                    if (controllerManagerInstance.swapped == false)
                    {
                        ActivateOptionGraphic(0);
                    }
                    else
                    {
                        ActivateOptionGraphic(5);
                    }
                }
                break;
            case CurrentControllerSetup.TwoPlayerOneKeyboard:
                if (InputManager.Devices.Count == 0)
                {
                    if (controllerManagerInstance.swapped == false)
                    {
                        ActivateOptionGraphic(0);
                    }
                    else
                    {
                        ActivateOptionGraphic(5);
                    }
                }
                break;
            case CurrentControllerSetup.OnePlayerController:
                if (InputManager.Devices.Count == 1)
                {
                    if (controllerManagerInstance.swapped == false)
                    {
                        ActivateOptionGraphic(1);
                    }
                    else
                    {
                        ActivateOptionGraphic(6);
                    }
                }
                break;
            case CurrentControllerSetup.TwoPlayerControllerAndKeyboard:
                if (InputManager.Devices.Count == 1)
                {
                    if (controllerManagerInstance.swapped == false)
                    {
                        ActivateOptionGraphic(2);
                    }
                    else
                    {
                        ActivateOptionGraphic(3);
                    }
                }
                break;
            case CurrentControllerSetup.TwoPlayerController:
                if (InputManager.Devices.Count == 2)
                {
                    ActivateOptionGraphic(4);
                }
                break;
        }
    }
}
