using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class InteriorTrigger : MonoBehaviour
{
    Renderer spriteComp;
    // Use this for initialization

    void Awake()
    {
        spriteComp = GetComponent<Renderer>();
        if (spriteComp)
            spriteComp.enabled = false;


    }

}