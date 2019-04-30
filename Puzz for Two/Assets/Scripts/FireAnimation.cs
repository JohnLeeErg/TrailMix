using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAnimation : MonoBehaviour {

    public SpriteRenderer rockGlowSprite;
    Color ogColor;
    float rockGlowValue = .3f;
    public float rockGlowMin, rockGlowMax;
    public float rockGlowVariance;

    public Animator backFireAnimator;
    public SpriteRenderer sticks;


	// Use this for initialization
	void Start () {
        ogColor = rockGlowSprite.color;
        backFireAnimator.Play(0, -1, .98f);

	}
	
	// Update is called once per frame
	void Update () {
        rockGlowSprite.color = new Color(ogColor.r, ogColor.g, ogColor.b, rockGlowValue);

        rockGlowValue += Random.Range(-rockGlowVariance, rockGlowVariance);
        rockGlowValue = Mathf.Clamp(rockGlowValue, rockGlowMin, rockGlowMax);

        if (sticks.color != Color.black)
        {
            sticks.color -= new Color(.002f, .002f, .002f,0);
        }

	}
}
