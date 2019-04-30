using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnAnyButton : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] float delay;
    bool beenDid = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!beenDid)
        {
            if (Input.anyKeyDown)
            {
                beenDid = true;
                
                Invoke("LoadTheFader", delay + .1f);
            }
        }
    }
    void LoadTheFader()
    {
        //print(sceneName);
        Fader.instance.FadeToColor(Color.black, 1f);
        Invoke("LoadTheScene", 1 + .1f);
    }
    void LoadTheScene()
    {
        //print(sceneName);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
