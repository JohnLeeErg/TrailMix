using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionAfterAnimation : MonoBehaviour
{
    Animator animatorComponent;
    SceneLoader sceneLoaderComponent;
    Fader faderInstence;

    bool isCalled = false;
    public string nextScene = "Level_Select";
    public float loadTime = 0.5f;

    // Use this for initialization
    void Start()
    {
        animatorComponent = GetComponent<Animator>();
        sceneLoaderComponent = GetComponent<SceneLoader>();
        faderInstence = Fader.instance;
    }

    public void TransitionToNextScene()
    {
        print("TRANISITION TO NEXT SCENE");

        sceneLoaderComponent.SetLoadSceneString(nextScene);
        sceneLoaderComponent.LoadScene(loadTime + 0.1f);
        faderInstence.FadeToBlack(loadTime);
    }
}
