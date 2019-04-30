using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavyInteractive : MonoBehaviour {

    [System.Serializable]
    public struct WaveData
    {
        public float waveForce;
        //this is a struct in case I wanna add more to be changed I guess
    }

    public enum ActiveWaveState
    {
        Resting,
        Calming,
        Interacted
    }

    public ActiveWaveState waveState;
    public AnimationCurve curveToMax;
    public AnimationCurve curveToRest;
    public float timeToRestFromMax;
    public float timeTillRest;
    public WaveData restingWaveData;
    public WaveData maxmimumWaveData;
    [SerializeField] WaveData currentWaveData;
    private WaveData targetWaveData; //what its trying to get to
    public float timeToMax;
    private WavySprite wavySpriteComp;

	// Use this for initialization
	void Start () {
        wavySpriteComp = GetComponent<WavySprite>();
    }
	
	// Update is called once per frame
	void Update () {
        switch (waveState)
        {
            case ActiveWaveState.Interacted:
                CalculateCurrentWave(maxmimumWaveData, restingWaveData, timeTillRest / timeToRestFromMax);
                SetWave(currentWaveData);
                TimerToRest();
                break;
        }
	}

    private void OnTriggerEnter2D(Collider2D collided)
    {
        if (collided.gameObject.tag == "Player")
        {
            waveState = ActiveWaveState.Interacted;
            SetWave(maxmimumWaveData);
            timeTillRest = 0f;
        }
    }

    void CalculateCurrentWave(WaveData startWave, WaveData endWave, float timeInLerp)
    {
        float animCurveTime = curveToRest.Evaluate(timeInLerp);
        currentWaveData.waveForce = Mathf.Lerp(startWave.waveForce, endWave.waveForce, animCurveTime);
    }

    void SetWave(WaveData waveDataUsed)
    {
        wavySpriteComp.waveForce = waveDataUsed.waveForce;
    }

    void TimerToRest()
    {
        if(timeTillRest >= 0f && timeTillRest < timeToRestFromMax)
        {
            timeTillRest += Time.deltaTime;
        } else if (timeTillRest >= timeToRestFromMax)
        {
            SetWave(restingWaveData);
            waveState = ActiveWaveState.Resting;
        }
    }
}
