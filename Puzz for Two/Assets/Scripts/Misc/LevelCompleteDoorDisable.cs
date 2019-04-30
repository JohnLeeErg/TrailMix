using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteDoorDisable : MonoBehaviour
{
    SaveManager saveManagerInstance;

    // Use this for initialization
    void Start()
    {
        saveManagerInstance = SaveManager.instance;

        if (saveManagerInstance != null)
        {
            if (saveManagerInstance.save.levelsComplete >= 3)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
