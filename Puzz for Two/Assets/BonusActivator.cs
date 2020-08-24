using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusActivator : MonoBehaviour {

    Transform child;
    [SerializeField] Animator[] playerThingsToTurnOn;
	// Use this for initialization
	void Awake () {
        child = transform.GetChild(0);
        if (SaveManager.instance)
        {
            if (SaveManager.instance.save.levelsComplete >= 15)
            {
                child.gameObject.SetActive(true);
                foreach (Animator playerThing in playerThingsToTurnOn)
                {
                    playerThing.enabled = true;
                    playerThing.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
