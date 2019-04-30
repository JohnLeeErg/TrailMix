using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Push3DTextToFront : MonoBehaviour {

    public string layerToPushTo;

    public bool execute = true;

    void Start()
    {
        if (execute)
        {
            GetComponent<Renderer>().sortingLayerName = layerToPushTo;
            GetComponent<Renderer>().sortingOrder = 10;
            //Debug.Log(GetComponent<Renderer>().sortingLayerName);
        }
    }

    public void PushToLayer()
    {
        GetComponent<Canvas>().sortingLayerName = layerToPushTo;
        GetComponent<Canvas>().sortingOrder = 10;
    }
}
