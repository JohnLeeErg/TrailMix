using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader instance;
    Image thingToFade;
    [SerializeField] Color colorOnStart;
    [SerializeField] bool fadeOnStart;
    [SerializeField] float fadeInTime = 0.5f;
    Image[] childImages;
    Text[] childTexts;
    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        thingToFade = GetComponent<Image>();
        //DontDestroyOnLoad(transform.root.gameObject);
        //set default color
        thingToFade.color = colorOnStart;
        childImages = GetComponentsInChildren<Image>();
        childTexts = GetComponentsInChildren<Text>();
        if (fadeOnStart)
        {
            FadeToColor(Color.clear, fadeInTime);
        }
    }

    public void Start()
    {
        //FadeToColor(Color.clear, 0.5f);
    }

    public void FadeToColor(Color color, float time)
    {
        if (thingToFade)
        {
            StopAllCoroutines();
            StartCoroutine(FadeEnumerator(color, time));
        }
    }
    void FadeToBlack(float time)
    {
        if (thingToFade)
        {
            StopAllCoroutines();
            StartCoroutine(FadeEnumerator(Color.black, time, true));
        }
    }
    /// <summary>
    /// used by UI buttons cause they cant pass in 2 things i guess
    /// </summary>
    /// <param name="time"></param>
    public void FadeToBlack(float time, bool useLoadChildren=true)
    {
        if (thingToFade)
        {
            StopAllCoroutines();
            StartCoroutine(FadeEnumerator(Color.black, time,useLoadChildren));
        }
    }

    IEnumerator FadeEnumerator(Color color, float time, bool useLoadChildren=true)
    {
        Color startColor = thingToFade.color;
        float timer = 0;
        foreach (Image eachChild in childImages)
        {

            if (eachChild.gameObject != gameObject)
            {//dont turn yourself on please
                if (useLoadChildren)
                {
                    eachChild.gameObject.SetActive(true);
                }
            }
        }
        
        while (thingToFade.color != color)
        {
            thingToFade.color = Color.Lerp(startColor, color, timer / time);
            foreach (Image eachChild in childImages)
            {
                eachChild.color = new Color(eachChild.color.r, eachChild.color.g, eachChild.color.b, thingToFade.color.a);
            }
            if (useLoadChildren)
            {

                foreach (Text eachText in childTexts)
                {
                    eachText.color = new Color(eachText.color.r, eachText.color.g, eachText.color.b, thingToFade.color.a);
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
        if (color == Color.clear)
        {
            foreach (Image eachChild in childImages)
            {

                if (eachChild.gameObject != gameObject)//dont turn yourself off please
                    eachChild.gameObject.SetActive(false);
            }
        }
        yield return null;
    }
}
