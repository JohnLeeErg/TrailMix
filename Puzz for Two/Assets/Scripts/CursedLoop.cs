using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedLoop : MonoBehaviour {
    AudioSource audioRef;
    [SerializeField] float pitchShiftRate;
	// Use this for initialization
	void Start () {
        audioRef = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!audioRef.isPlaying)
        {
            //audioRef.time=87;
            audioRef.pitch *= pitchShiftRate;
            audioRef.Play();
        }
	}
}
