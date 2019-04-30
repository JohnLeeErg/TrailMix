using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour {
    Renderer rendComp;
    Material instanceMatRef;
    [SerializeField] float scrollSpeed;
    [SerializeField] Vector2 scrollVector;
	// Use this for initialization
	void Start () {
        rendComp = GetComponent<Renderer>();
        instanceMatRef = rendComp.material;
	}
	
	// Update is called once per frame
	void Update () {
        instanceMatRef.mainTextureOffset += scrollVector*Time.deltaTime;
	}
}
