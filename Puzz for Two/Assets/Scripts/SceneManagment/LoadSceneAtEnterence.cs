using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAtEnterence : MonoBehaviour
{
    [Header("Player Values")]
    List<GameObject> players = new List<GameObject>();
    List<Movement> playerMovementScripts = new List<Movement>();
    List<PlayerThrowing> playerThrowingScripts = new List<PlayerThrowing>();
    List<PlayerHealth> playerHealthScripts = new List<PlayerHealth>();
    List<PlayerIndicator> playerIndicatorScripts = new List<PlayerIndicator>();


    [Header("Scene Change Values")]
    [SerializeField] int levelCompletionRequirements;
    [SerializeField] string sceneName;
    [SerializeField] float delay = 0.5f;
    bool completedOnce = false;


    public FMODUnity.StudioEventEmitter InteractionMusic;


    // Create FMOD Sound Effect Variables
    [Header("FMOD Audio Events")]
    [FMODUnity.EventRef]
    public string confirmLevelSound, levelInteractionSound;
    FMOD.Studio.EventInstance fModConfirmLevelEvent, fModLevelInteractionEvent;

    InteractiveMenu interactiveMenuSingleton;

    // Save file values
    SaveManager.LevelStats levelSaveFile;

    [Header("UI Elements")]
    [SerializeField] GameObject uiCompletedLevelInfo;
    [SerializeField] GameObject completedLevelInfo;
    [SerializeField] GameObject lockedLevelInfo;
    [SerializeField] SpriteRenderer bonusCollectableSprite, bonusCollectableSprite2, bonusCollectableSprite3;  //cig
    [SerializeField] TextMesh timeCompletedText;
    [SerializeField] TextMesh camperCollectedText;
    [SerializeField] TextMesh levelCompNumbers;
    //  Level Completed Badge
    //  Total collected campers/ total amount of campers in level
    //  Cig badge
    //  Fastest time completed
    NewControllerManager controllerManagerInstance;

    // Use this for initialization
    void Start()
    {
        controllerManagerInstance = NewControllerManager.instance;

        fModConfirmLevelEvent = FMODUnity.RuntimeManager.CreateInstance(confirmLevelSound);
        fModLevelInteractionEvent = FMODUnity.RuntimeManager.CreateInstance(levelInteractionSound);

        // Get a referecne to the pause menu so that it can be disabled when players try to laod a level
        if (InteractiveMenu.instance != null)
        {
            interactiveMenuSingleton = InteractiveMenu.instance;
        }
        if (SaveManager.instance)
        {
            if (SaveManager.instance.save.levelByName[sceneName] != null)
            {
                levelSaveFile = SaveManager.instance.save.levelByName[sceneName];
            }
            else
            {
                print(sceneName);
            }
        }
        // Update the level information displayed if a level is completed
        UpdateLevelInformation();
    }

    // Update is called once per frame
    void Update()
    {
        if (completedOnce == false)
        {
            if (players.Count >= 2)
            {
                if (controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.OnePlayerKeyboard
                     || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.OnePlayerController
                     || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.Undecided)
                {
                    if (playerIndicatorScripts[0].playerIsHoldingDownButton || playerIndicatorScripts[1].playerIsHoldingDownButton)
                    {
                        OnPlayerChooseLevel();
                    }
                }

                else if (controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerControllerAndKeyboard
                    || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerController
                    || controllerManagerInstance.controllerTypeInputted == CurrentControllerSetup.TwoPlayerOneKeyboard)
                {
                    if (playerIndicatorScripts[0].playerIsHoldingDownButton && playerIndicatorScripts[1].playerIsHoldingDownButton)
                    {
                        OnPlayerChooseLevel();
                    }
                }
            }
        }
    }

    void OnPlayerChooseLevel()
    {
        // Get the other components on the player to prevent them from moving or doing anything as the scene changes
        for (int j = 0; j < players.Count; j++)
        {
            PlayerThrowing playerThrow = players[j].GetComponent<PlayerThrowing>();
            playerThrowingScripts.Add(playerThrow);
            PlayerHealth health = players[j].GetComponent<PlayerHealth>();
            playerHealthScripts.Add(health);
        }

        // Lock the player's Movment
        for (int i = 0; i < players.Count; i++)
        {
            playerMovementScripts[i].StopMovementNotCollision();
            playerThrowingScripts[i].StopThrowing();
            playerHealthScripts[i].StopCatching();
        }

        // Change the scene
        Fader.instance.FadeToColor(Color.black, delay);
        Invoke("LoadTheScene", delay + .1f);

        // Disable the player speach bubbles
        for (int l = 0; l < playerIndicatorScripts.Count; l++)
        {
            playerIndicatorScripts[l].DisableImage();
        }

        //Play level Comfirm Sound
        fModConfirmLevelEvent.start();

        // prevent players from accessing the menu
        if (interactiveMenuSingleton != null)
        {
            interactiveMenuSingleton.DisableMenu();
        }

        completedOnce = true;
    }



    void LoadTheScene()
    {
        if (sceneName != null)
        {

            SaveManager.instance.StartLevel(sceneName);
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        else
        {
            print("THERE IS NO SCENE TO TRANSITION TO, PLEASE ADD A NAME");
        }
    }

    void UpdateLevelInformation()
    {
        // update the information to reflect the information in the save file
        if (levelSaveFile.complete == true)
        {
            // bonus collectable
            bonusCollectableSprite.enabled = levelSaveFile.Cig;
            bonusCollectableSprite2.enabled = levelSaveFile.Lipstick;
            if (levelSaveFile.highestBoyCount >= levelSaveFile.maxBoyCount)
            {
                bonusCollectableSprite3.enabled = true;
            }
            else
            {
                bonusCollectableSprite3.enabled = false;
            }
            // level completion time
            string timeString = string.Format("{0}:{1:00}:{2:00}", (int)levelSaveFile.fastestTime / 3600, ((int)levelSaveFile.fastestTime / 60) % 60, (int)levelSaveFile.fastestTime % 60);
            timeCompletedText.text = "Time: " + timeString;

            // campers collected
            camperCollectedText.text = levelSaveFile.highestBoyCount + "/" + levelSaveFile.maxBoyCount + " Campers Collected";
        }
    }

    // function to make the information bubbles appear
    void ChooseLevelInfoToDisplay(bool enable)
    {
        if (SaveManager.instance.save.levelsComplete >= levelCompletionRequirements)
        {
            if (levelSaveFile.complete == true)
            {
                completedLevelInfo.SetActive(enable);
            }
            else if (levelSaveFile.complete == false)
            {
                uiCompletedLevelInfo.SetActive(enable);
            }
        }
        else
        {
            levelCompNumbers.text = SaveManager.instance.save.levelsComplete + "/" + levelCompletionRequirements;
            lockedLevelInfo.SetActive(enable);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //print("player is in");

            GameObject enteredPlayer = collision.gameObject.transform.parent.root.gameObject;

            // check if the player that entered te fire is the same as one that just entered
            if (players.Contains(enteredPlayer) == false)
            {
                players.Add(enteredPlayer);
                playerMovementScripts.Add(enteredPlayer.GetComponent<Movement>());

                PlayerIndicator indicator = enteredPlayer.GetComponentInChildren<PlayerIndicator>();
                playerIndicatorScripts.Add(indicator);
                indicator.ActivateImage();  // Make the indicators on the player visible when they enter the campfire range
            }


            // Check if both players are in the campfire
            if (players.Count >= 2)
            {
                ChooseLevelInfoToDisplay(true); // Activate the text when both players are present 

                if (InteractionMusic != null)
                {
                    InteractionMusic.Play();    // Play Music
                }

                fModLevelInteractionEvent.start();
                if (SaveManager.instance.save.levelsComplete >= levelCompletionRequirements)
                {
                    //unCompletedLevelInfo.SetActive(true); 


                    for (int i = 0; i < playerIndicatorScripts.Count; i++)
                    {
                        playerIndicatorScripts[i].ChangeImage();
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //print("player is out");
            GameObject exitedPlayer = collision.gameObject.transform.parent.root.gameObject;

            if (players.Contains(exitedPlayer) == true)
            {
                int exitIndex = players.IndexOf(exitedPlayer);
                players.RemoveAt(exitIndex);
                playerMovementScripts.RemoveAt(exitIndex);

                playerIndicatorScripts[exitIndex].RevertImage();
                playerIndicatorScripts[exitIndex].DisableImage();   // make the campfire indicators disappear when a player leaves the campfire
                playerIndicatorScripts.RemoveAt(exitIndex);
            }

            if (players.Count < 2)
            {
                //unCompletedLevelInfo.SetActive(false);
                ChooseLevelInfoToDisplay(false);

                if (InteractionMusic != null)
                {
                    InteractionMusic.Stop();    // Stop Music
                }

                for (int i = 0; i < playerIndicatorScripts.Count; i++)
                {
                    playerIndicatorScripts[i].RevertImage();
                }
            }
        }
    }
}
