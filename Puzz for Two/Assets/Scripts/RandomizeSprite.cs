using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSprite : MonoBehaviour {
    [SerializeField] Sprite[] possibleSprites;
	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
