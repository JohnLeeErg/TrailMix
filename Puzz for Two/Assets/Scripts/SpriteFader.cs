using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFader : MonoBehaviour
{
    public static SpriteFader instance;
    SpriteRenderer thingToFade;
    [SerializeField] Color colorOnStart;
    [SerializeField] bool fadeOnStart;
    [SerializeField] float fadeInTime = 0.5f;
    
    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        thingToFade = GetComponent<SpriteRenderer>();
        //DontDestroyOnLoad(transform.root.gameObject);
        //set default color
        thingToFade.color = colorOnStart;
        
    }

    public void Start()
    {
        //FadeToColor(Color.clear, 0.5f);
    }

    public void Update()
    {
        thingToFade.color = new Color(thingToFade.color.r, thingToFade.color.g, thingToFade.color.b,1 - (Camera.main.transform.position.x / 300));
    }
}
