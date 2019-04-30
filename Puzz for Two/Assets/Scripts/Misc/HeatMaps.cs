using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class HeatMaps : MonoBehaviour {
    TrailRenderer trailRef;
    LineRenderer lineRef;
    [SerializeField] HeatMapMode currentMode;
    [SerializeField] int playerNumber;
    enum HeatMapMode
    {
        recording,
        viewing,
        disabled
    }
	// Use this for initialization
	void Start () {

       
            lineRef = GetComponent<LineRenderer>();
        trailRef = GetComponent<TrailRenderer>();

        if (currentMode==HeatMapMode.viewing)
        {
            string folder="saves";
            if (SaveManager.instance)
            {
                folder=SaveManager.instance.filePath;
            }
            if (File.Exists( Path.Combine(folder,SceneManager.GetActiveScene().name+playerNumber+"heatmaptest.txt")))
            {
                List<string> vectorsAsWords = new List<string>();
                vectorsAsWords.AddRange(File.ReadAllLines(Path.Combine(folder, SceneManager.GetActiveScene().name + playerNumber + "heatmaptest.txt")));
                List<Vector3> wordsAsVectors = new List<Vector3>();
                foreach (string eachLine in vectorsAsWords)
                {
                    wordsAsVectors.Add(StringToVector3(eachLine));
                }
                lineRef.positionCount = wordsAsVectors.Count;
                lineRef.SetPositions(wordsAsVectors.ToArray());
                lineRef.enabled = true;
            }
        }
	}
    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
    // Update is called once per frame
    void Update () {
        
	}
    private void OnDestroy()
    {
        if (currentMode==HeatMapMode.recording)
        {
            Vector3[] tempArray = new Vector3[trailRef.positionCount];
            trailRef.GetPositions(tempArray);
            lineRef.SetPositions(tempArray);
            print("saving the lines");

            string folder = "";
            if (SaveManager.instance)
            {
                folder = SaveManager.instance.filePath;
            }

            List<string> vectorsAsWords = new List<string>();
            foreach (Vector3 eachPos in tempArray)
            {
                vectorsAsWords.Add(eachPos.ToString());
            }
            File.WriteAllLines(Path.Combine(folder, SceneManager.GetActiveScene().name + playerNumber + "heatmaptest.txt"), vectorsAsWords.ToArray());
        }

    }
}
