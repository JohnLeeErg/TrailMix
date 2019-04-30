using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputField : MonoBehaviour
{
    public new string name;
    [HideInInspector]
    public string[] inputs; //the inputs from the input manager
    [HideInInspector]
    public string inputUsed; //input that is used
}
