using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

    string levelLoadString;
    [SerializeField] string demoModeOverrideLevel;
    public void ResetScene(float waitTime)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string currentSceneName = currentScene.name;

        IEnumerator waitEnum = WaitToReloadLevel(waitTime, currentSceneName);
        StartCoroutine(waitEnum);
    }

    public void SetLoadSceneString(string sceneName)
    {
        levelLoadString = sceneName;
    }

    public void LoadScene(float waitTime)
    {
        if (demoModeOverrideLevel != "" && NewControllerManager.instance && NewControllerManager.instance.demoMode)
        {
            IEnumerator waitEnum = WaitToChangeScene(waitTime, demoModeOverrideLevel);
            StartCoroutine(waitEnum);
        }
        else
        {
            IEnumerator waitEnum = WaitToChangeScene(waitTime, levelLoadString);
            StartCoroutine(waitEnum);
        }
    }

    public void QuitGame(float waitTime)
    {
        IEnumerator waitEnum = WaitToQuit(waitTime);
        StartCoroutine(waitEnum);
    }

    IEnumerator WaitToQuit(float waitAmount)
    {
        GameObject.Find("Loading Text").GetComponent<Text>().text = "Quitting...";
        yield return new WaitForSeconds(waitAmount);
        Application.Quit();
        
    }

    IEnumerator WaitToChangeScene(float waitAmount, string sceneName)
    {
        yield return new WaitForSeconds(waitAmount);
        DetachedPiece.sortPeak = 0; //reset these once in a while just in case
        Movement.sortPeak = 0;
        SceneManager.LoadSceneAsync(sceneName);
    }
    IEnumerator WaitToReloadLevel(float waitAmount, string sceneName)
    {
        yield return new WaitForSeconds(waitAmount);
        DetachedPiece.sortPeak = 0; //reset these once in a while just in case
        Movement.sortPeak = 0;
        if (SaveManager.instance)
            SaveManager.instance.startLevelTime = Time.time;
        SceneManager.LoadSceneAsync(sceneName);
    }
}
