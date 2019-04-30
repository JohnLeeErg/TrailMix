using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SceneLoader))]
public class DebugSceneLoader : MonoBehaviour {

    Fader faderInstance;
    SceneLoader sceneLoaderComponent;

    public float loadSceneTime = 1f;
    public GameObject lockedDoor;

    private void Start()
    {
        faderInstance = Fader.instance;
        sceneLoaderComponent = GetComponent<SceneLoader>();
        if (NewControllerManager.instance)
        {
            if (!NewControllerManager.instance.demoMode)
            {
                enabled = false;
            }
        }
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))   // Level 1
        {
            CallScene("Level_1_Final");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))   // Level 2
        {
            CallScene("Level_2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))   // Level 3
        {
            CallScene("Level_3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))   // Level 4
        {
            CallScene("The Bite");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))   // Level 5
        {
            CallScene("Richard");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))   // Level 6
        {
            CallScene("Starcrossed Glovers");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))   // Title Screen 
        {
            CallScene("Controller_Setup");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))   // Level Select
        {
            CallScene("Level_Select");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))   // Thank You for Playing
        {
            CallScene("Demo_Thanks");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))   // Attract Mode
        {
            CallScene("Arract_Mode");
        }

        if (lockedDoor != null && Input.GetKeyDown(KeyCode.Equals))   // Open Door
        {
            lockedDoor.SetActive(false);
        }
    }

    void CallScene(string sceneName)
    {
        sceneLoaderComponent.SetLoadSceneString(sceneName);
        sceneLoaderComponent.LoadScene(loadSceneTime + 0.1f);

        faderInstance.FadeToBlack(loadSceneTime);
    }

    /*  Things we want to be able to debug
     *      Load Title Screen
     *      Load Level Select
     *      Reset Level
     *      Load Lvl 1
     *      Load Lvl 2
     *      Load Lvl 3 
     *      Load Lvl 4
     *      Load Lvl 5
     *      Load Lvl 6
     *      Open Level Select Door Open
     *      Load Thank You For Playing
     *      Load Attract Mode
     */
}
