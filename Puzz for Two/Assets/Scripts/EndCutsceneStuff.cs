using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EndCutsceneStuff : MonoBehaviour {

    bool ended = false;
    public GameObject p1, p2;

    float xValueForDoors = 118;
    public Transform door1, door2;
    float door1StartingXpos;
    float door2StartingXpos;
    public float door1EndingXpos;
    public float door2EndingXpos;
    public AnimationCurve doorCurve;
    float doorAnimationTimer = 0;
    public float doorAnimationTime = 30;
    [SerializeField] FinalTime timeRef;
    bool animatingDoors = false;

    float xValue = 128;

    int numberInCreditsSequence = 0;
    public Text[] endText;
    public Image endImage;
    public AnimationCurve fadeAnimation;
    float AnimationTimer = 160;
    public float AnimationFrames = 300;

    Fader faderInstance;
    [SerializeField] SceneLoader sceneLoaderComponent;

    // Use this for initialization
    void Start () {
        faderInstance = Fader.instance;
        door1StartingXpos = door1.transform.position.x;
        door2StartingXpos = door2.transform.position.x;
    }
	
	// Update is called once per frame
	void Update () {
        if (!ended)
        {
            if (p1.transform.position.x > xValue && p2.transform.position.x > xValue)
            {
                ended = true;
                p1.GetComponent<Movement>().enabled = false;
                p2.GetComponent<Movement>().enabled = false;
                timeRef.FinalTimeMode();
            }
            if (!animatingDoors && (p1.transform.position.x > xValueForDoors || p2.transform.position.x > xValueForDoors))
            {
                animatingDoors = true;
            }
            if (p1.transform.position.x < (xValueForDoors-30) && p2.transform.position.x < (xValueForDoors-30))
            {
                animatingDoors = false;
                door1.position = new Vector3(door1StartingXpos, door1.position.y, door1.position.z);
                door2.position = new Vector3(door2StartingXpos, door2.position.y, door2.position.z);
            }
        } else
        {
            if (AnimationTimer < AnimationFrames && numberInCreditsSequence < endText.Length) {
                AnimationTimer++;
                endText[numberInCreditsSequence].color = new Color(1,1,1,fadeAnimation.Evaluate(AnimationTimer/AnimationFrames));
                if (numberInCreditsSequence == 1)
                {
                    endImage.color = endText[numberInCreditsSequence].color;
                }
            } else
            {
                numberInCreditsSequence++;
                AnimationTimer = 0;
                if (numberInCreditsSequence >= endText.Length)
                {
                    sceneLoaderComponent.SetLoadSceneString("Level_Select");
                    sceneLoaderComponent.LoadScene(10 + 2f);
                    faderInstance.FadeToBlack(10,false);
                    if (SaveManager.instance)
                    {

                        SaveManager.instance.save.player1Position = new Vector3(7.5f, -2.5f, 0);
                        SaveManager.instance.save.player2Position = new Vector3(1.5f, -2.5f, 0);
                    }
                    this.enabled = false; //stop calling this
                }
            }
            
        }
        if (animatingDoors)
        {
            doorAnimationTimer++;
            door1.position = new Vector3(Mathf.Lerp(door1StartingXpos,door1EndingXpos, doorCurve.Evaluate(doorAnimationTimer / doorAnimationTime)), door1.position.y, door1.position.z);
            door2.position = new Vector3(Mathf.Lerp(door2StartingXpos,door2EndingXpos, doorCurve.Evaluate(doorAnimationTimer / doorAnimationTime)), door2.position.y, door2.position.z);

            if (doorAnimationTimer >= doorAnimationTime)
            {
                animatingDoors = false;
            }
        }

	}
}
