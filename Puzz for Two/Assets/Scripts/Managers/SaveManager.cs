using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
/// <summary>
/// simple thing that exports and imports saves
/// </summary>
public class SaveManager : MonoBehaviour {
    public SaveFile save;
    public string filePath;
    [SerializeField] string fileName;
    string combinedPath;
    public string[] levelNames;
    public Transform player1, player2;
    public static SaveManager instance;
    public float startLevelTime;
    public float timeOfLastSave=0;
    public int levelsComplete;
    Dictionary<string, int> cigPasswords=new Dictionary<string, int>();
    int[] cigPasswordInts;
    AudioSource audioRef;
    [SerializeField] GameObject myChild;
    public class SaveFile
    {
        public int gameVersion = 1;

        public Vector2 player1Position, player2Position;
        public string currentLevel;
        public float timeOnFile;
        public Dictionary<string, LevelStats> levelByName;
        public LevelStats[] listForFile;
        public int levelsComplete;
        public int magicNumber=62939; //remember this
        public SaveFile(Vector2 p1, Vector2 p2, string lev, float time,string[] levNames,int levelsComp,int magicNum= 62939)
        {
            listForFile = new LevelStats[levNames.Length];
            player1Position = p1;
            player2Position = p2;
            currentLevel = lev;
            timeOnFile = time;
            levelByName = new Dictionary<string, LevelStats>();
            levelsComplete = levelsComp;
            magicNumber = magicNum;
            foreach (string eachLev in levNames)
            {
                levelByName.Add(eachLev, new LevelStats(eachLev));
            }
        }
        public void RemakeDict()
        {
            levelByName = new Dictionary<string, LevelStats>();
            foreach (LevelStats eachLev in listForFile)
            {

                levelByName.Add(eachLev.levelName,eachLev);
            }
        }
    }
    public class LevelStats
    {
        public string levelName;
        public bool complete=false;
        public int maxBoyCount;
        //public 
        public int highestBoyCount;
        public bool Cig;
        public bool Lipstick;
        public float fastestTime;
        public float latestTime;
        public LevelStats(string name)
        {
            levelName = name;
        }
    }
    void FindPlayers()
    {
        if(player1==null)
            player1 = GameObject.Find("Player 1").transform; //get ref to players
        if(player2==null)
            player2 = GameObject.Find("Player 2").transform;

    }

    private void Awake()
    {
        //QualitySettings.vSyncCount = 1;
        //Application.targetFrameRate = 60;
        audioRef = GetComponent<AudioSource>();
        if (instance == null) //make sure you are the only save manager
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        cigPasswordInts = new int[] { 532,920,5963,95323,1553,
                                      0643,125,12566,52354,163,
                                      7273,123,2135}; //242612
        for(int i=0;i<levelNames.Length;i++)
        {
            if (cigPasswordInts.Length > i)
            {
                cigPasswords.Add(levelNames[i], cigPasswordInts[i]);
            }
            else
            {
                cigPasswords.Add(levelNames[i], 0);
            }
        }
        FindPlayers();
#if UNITY_STANDALONE_OSX
        filePath = Application.persistentDataPath;
#endif
        //if theres no save folder make a save folder
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        combinedPath = Path.Combine(filePath, fileName);

        if (File.Exists(combinedPath))
        {
            save = JsonUtility.FromJson<SaveFile>(File.ReadAllText(combinedPath));
            save.listForFile = new LevelStats[levelNames.Length];
            for (int i = 0; i < levelNames.Length; i++)
            {
                if (File.Exists(Path.Combine(filePath, levelNames[i]+".puzz"))) //if there's already a save for that level, load it
                {
                    save.listForFile[i] = JsonUtility.FromJson<LevelStats>(File.ReadAllText(Path.Combine(filePath, levelNames[i]+".puzz")));
                }
                else
                {
                    //otherwise make a new one
                    save.listForFile[i] = new LevelStats(levelNames[i]);
                }
            }
            save.RemakeDict();
        }
        else
        {
            CreateBlankSave();

        }
        StartCoroutine(UpdateBasedOnSave());
    }

    // Use this for initialization
    void Start () {

        
    }

    /// <summary>
    /// update the pos in hub every time it's loaded
    /// </summary>
    /// <param name="level"></param>
    private void OnLevelWasLoaded(int level)
    {
        if (this == instance)
        {
            if (SceneManager.GetSceneByBuildIndex(level) == SceneManager.GetSceneByName("Level_Select")) //THIS NEEDS TO BE THE BUILD INDEX OF THE LEVEL SELECT
                StartCoroutine(UpdateBasedOnSave());
        }
    }
    /// <summary>
    /// updates positions based on the save
    /// </summary>
    public IEnumerator UpdateBasedOnSave()
    {

        player1 = null;
        player2 = null;
        while(player1==null && player2 == null)
        {
            FindPlayers(); //in case you've switched scenes and lost reference
            yield return null;
        }
        //print(save.player1Position);
        player1.position = save.player1Position;
        player2.position = save.player2Position;
        //print(player1.position);
        //auto update the camera pos so that you dont see it zoot over
        MovingCamera camRef=Camera.main.transform.root.GetComponentInChildren<MovingCamera>();
        while (!camRef)
        {

            camRef = Camera.main.transform.root.GetComponentInChildren<MovingCamera>();
            yield return null;
        }
        camRef.transform.position =new Vector3( camRef.AveragePosition().x,camRef.AveragePosition().y,camRef.transform.position.z);


        if (VerifyCigs())
        {
            audioRef.PlayDelayed(1);
            myChild = GameObject.Find("cigParticles");
            myChild.transform.position = Camera.main.transform.position + Vector3.up*20f;
            myChild.transform.parent = Camera.main.transform;
            myChild.transform.GetChild(0).gameObject.SetActive(true);

        }
    }

    public void StartLevel(string level)
    {
        FindPlayers(); //in case you've switched scenes and lost reference
        save.currentLevel = level;
        //update positions to spawn at on return
        save.player1Position = player1.position;
        save.player2Position = player2.position;
        startLevelTime = Time.time;
    }

    public LevelStats GetCurrentLevel()
    {
        return save.levelByName[save.currentLevel];
    }

    public void FinishLevel(int boysCaught,int totalBoys,bool cigFound,bool lipstickFound) //called when you beat a level or whatever
    {
        print(save.currentLevel);
        LevelStats finishedLevel = save.levelByName[save.currentLevel];
        if (!finishedLevel.complete)
        {
            save.levelsComplete++;
            finishedLevel.complete = true;
        }   
        finishedLevel.maxBoyCount = totalBoys;
        if (!finishedLevel.Cig && cigFound) //only set it if you haven't found it yet
        {
            finishedLevel.Cig = cigFound;
            save.magicNumber += cigPasswords[finishedLevel.levelName]; //for "verification"
        }
        if (!finishedLevel.Lipstick)
        {
            finishedLevel.Lipstick = lipstickFound;
        }

        if (boysCaught > finishedLevel.highestBoyCount) //only set it if it's higher than before
        {
            finishedLevel.highestBoyCount = boysCaught;
        }
        finishedLevel.latestTime = Time.time - startLevelTime;
        if ((finishedLevel.latestTime < finishedLevel.fastestTime)||finishedLevel.fastestTime==0f)
        {
            finishedLevel.fastestTime = finishedLevel.latestTime;
        }
        WriteSaveToFile();

    }
    public void BackToLevelSelect()
    {
        save.currentLevel = "Level Select";
    }

    void WriteSaveToFile()
    {
        save.timeOnFile +=  (Time.time - timeOfLastSave); //how long you've been playing, minus the last time this session you saved
        timeOfLastSave = Time.time;
        save.listForFile = save.levelByName.Values.ToArray<LevelStats>();

        if (save.currentLevel == "Level Select") //when you quit from level select just actually save all the levels just in case
        {
            foreach (LevelStats eachLevel in save.listForFile)
            {
                File.WriteAllText(Path.Combine(filePath,eachLevel.levelName + ".puzz"), JsonUtility.ToJson(eachLevel));
            }
        }
        else
        {
            //when you're just saving after leaving a level, only save that level out
            File.WriteAllText(Path.Combine(filePath,save.currentLevel + ".puzz"), JsonUtility.ToJson(save.levelByName[save.currentLevel]));
        }
        File.WriteAllText(combinedPath, JsonUtility.ToJson(save));
        
        

    }
    bool VerifyCigs()
    {
        int realAnswer = 62939;
        foreach(string eachName in levelNames)
        {
            realAnswer += cigPasswords[eachName];
        }
        return (save.magicNumber == realAnswer);
    }
    void CreateBlankSave()
    {
        //create a new one
        save = new SaveFile(player1.position,player2.position,"Level Select",0,levelNames,levelsComplete);
    }

    private void OnApplicationQuit()
    {
        //attempt to save when they close the game too

        save.currentLevel = "Level Select"; //assume they're going out to level select so it saves all the levels

        WriteSaveToFile();
    }
}
