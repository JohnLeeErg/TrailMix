using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OnlyPlayMusicIfSave : MonoBehaviour {
    [SerializeField] FMODUnity.StudioEventEmitter songo;
    // Use this for initialization
    void Start()
    {
        //if theres a save play the song
        if (Directory.Exists("saves/"))
        {
            songo.Play();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
