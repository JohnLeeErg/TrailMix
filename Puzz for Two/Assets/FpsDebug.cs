using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsDebug : MonoBehaviour
{
    [SerializeField] Text renderFPSText;
    [SerializeField] Text physicsFPSText;


    float frameCount = 0;
    float dt = 0.0f;
    float fps = 0.0f;

    float frameCountP = 0;
    float dtP = 0.0f;
    float fpsP = 0.0f;

    float updateRate = 4.0f;  // 4 updates per sec.

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0f / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }

        renderFPSText.text = "RenderFPS: " + Mathf.RoundToInt(fps);
    }

    private void FixedUpdate()
    {
        frameCountP++;
        dtP += Time.fixedDeltaTime;
        if (dtP > 1.0f / updateRate)
        {
            fpsP = frameCountP / dtP;
            frameCountP = 0;
            dtP -= 1.0f / updateRate;
        }

        physicsFPSText.text = "PhysicsFPS: " + Mathf.RoundToInt(fpsP);
    }
}
