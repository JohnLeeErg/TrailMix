using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InControl;
using UnityEngine.UI;

public class ControllerDeviceOption : MonoBehaviour {

    public bool isEnabled = false;
    Image[] ImageComponents;
    Text [] TextComponents;

    public UnityEvent OnChosenInputDevice;
    NewControllerManager controllerManagerInstance;

    private void Awake()
    {
        ImageComponents = GetComponentsInChildren<Image>();
        TextComponents = GetComponentsInChildren<Text>();
        DisbaleOption();
    }

    private void Start()
    {
        controllerManagerInstance = NewControllerManager.instance;
    }

    public void OnePlayerKeyboardControls()
    {
        //print("1 Player Keyboard!");
        controllerManagerInstance.swapPermitted = true;
        controllerManagerInstance.ResetBothIncontrolThings();

        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.OnePlayerKeyboard;

    }

    public void TwoPlayerOneKeyboard()
    {
        //print("2 Player one Keyboard!");
        controllerManagerInstance.swapPermitted = false;
        controllerManagerInstance.ResetBothIncontrolThings();

        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.TwoPlayerOneKeyboard;
    }

    public void OnePlayerControllerControls()
    {
        //print("1 Player Controller!");
        controllerManagerInstance.swapPermitted = true;
        controllerManagerInstance.ResetBothIncontrolThings();

        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.OnePlayerController;
    }

    public void TwoPlayerControllerAndKeyboardControls()
    {
        //print("2 Player Controller and Keyboard!");
        controllerManagerInstance.swapPermitted = false;
        controllerManagerInstance.ResetBothIncontrolThings();

        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.TwoPlayerControllerAndKeyboard;
    }

    public void TwoPlayerControllerControls()
    {
        //print("2 Player Controller!");
        controllerManagerInstance.swapPermitted = false;
        controllerManagerInstance.ResetBothIncontrolThings();

        controllerManagerInstance.controllerTypeInputted = CurrentControllerSetup.TwoPlayerController;
    }

    public void EnableOption()
    {
        isEnabled = true;

        for (int i = 0; i < ImageComponents.Length; i++)
        {
            ImageComponents[i].color = Color.white;
        }

        for (int j = 0; j < TextComponents.Length; j++)
        {
            TextComponents[j].color = new Color(TextComponents[j].color.r, TextComponents[j].color.g, TextComponents[j].color.b, 1.0f);
        }
    }

    public void DisbaleOption()
    {
        isEnabled = false;

        for (int i = 0; i < ImageComponents.Length; i++)
        {
            ImageComponents[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        for (int j = 0; j < TextComponents.Length; j++)
        {
            TextComponents[j].color = new Color(TextComponents[j].color.r, TextComponents[j].color.g, TextComponents[j].color.b, 0.5f);
        }
    }
}
