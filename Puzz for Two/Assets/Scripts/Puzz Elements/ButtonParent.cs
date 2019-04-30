using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ButtonParent : MonoBehaviour
{
    public List<Button> buttonsInPuzzle = new List<Button>();
    public UnityEvent eventOnFulfilled,eventOnUnFulfilled;
    public KeyCode keyPress;
    public bool solved = false;
    public bool remainAfterSolving; //for the rare buttons that aren't one time use
    // Create FMOD Sound Effect Variables
    [Header("FMOD Audio Events")]
    [FMODUnity.EventRef]
    public string buttonActivateSound;
    FMOD.Studio.EventInstance fModButtonActivateEvent;
    public int currentlyActiveButtons;
    // Use this for initialization
    void Awake()
    {
        solved = false;
        buttonsInPuzzle.Clear();
        buttonsInPuzzle.AddRange(GetComponentsInChildren<Button>());
    }

    private void Start()
    {
        fModButtonActivateEvent = FMODUnity.RuntimeManager.CreateInstance(buttonActivateSound);
    }

    // Update is called once per frame
    void Update()
    {
        if (!solved)
        {
            if (CheckActivated())
            {
                solved = true;
                if (eventOnFulfilled != null)
                {
                    eventOnFulfilled.Invoke();
                    fModButtonActivateEvent.start();
                    if (!remainAfterSolving)
                    {
                        foreach(Button eachButt in buttonsInPuzzle)
                        {
                            eachButt.FadeAway();
                        }
                    }
                }
            }
            if (Input.GetKeyDown(keyPress)) //debug win
            {
                solved = true;
                if (eventOnFulfilled != null)
                {
                    eventOnFulfilled.Invoke();
                    fModButtonActivateEvent.start();
                }
            }
        }
        else if (!CheckActivated())
        {
            if (eventOnUnFulfilled != null)
            {
                eventOnUnFulfilled.Invoke();
                solved = false;
            }
        }
        
    }

    public bool CheckActivated()
    {
        bool solve = true;
        currentlyActiveButtons = 0;
        for (int i = 0; i < buttonsInPuzzle.Count; i++)
        {
            if (!buttonsInPuzzle[i].isActivated)
            {
                solve = false;
                break;
            }
            else
            {

                currentlyActiveButtons++;
            }
        }
        return solve;
    }
    /// <summary>
    /// determines if the owner passed in already is pressing any other buttons
    /// </summary>
    /// <param name="owner">thing pressing the button</param>
    /// <returns>true if its not pressing anything, otherwise false</returns>
    public bool IsNewOwner(GameObject owner)
    { 
        foreach(Button eachButton in buttonsInPuzzle)
        {
            if (eachButton.owner == owner)
            {
                return false;
            }
        }
        return true;
    }
}
