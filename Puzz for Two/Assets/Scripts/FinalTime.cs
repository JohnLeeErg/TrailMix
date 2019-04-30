using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalTime : MonoBehaviour {
    Text textComp;
	// Use this for initialization
	void Start () {
        textComp = GetComponent<Text>();
	}
	public void FinalTimeMode()
    {
        if (SaveManager.instance)
        {
            float time = SaveManager.instance.save.timeOnFile + (Time.time-SaveManager.instance.timeOfLastSave);
            textComp.text= "Your Total Playtime was:\n"+string.Format("{0}:{1:00}:{2:00}", (int)time / 3600, ((int)time / 60) % 60, (int)time % 60);

        }
    }
	
}
