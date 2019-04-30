using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowCode : MonoBehaviour {
    SpriteRenderer spriteComp;
    
	// Use this for initialization
	void Awake () {
        spriteComp = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerPiece>() && collision.transform.root==transform.root)
        {
            spriteComp.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerPiece>() && collision.transform.root == transform.root)
        {
            spriteComp.enabled = false;
        }
    }
    private void OnDisable()
    {
        if(spriteComp)
        spriteComp.enabled = false;
    }
}
