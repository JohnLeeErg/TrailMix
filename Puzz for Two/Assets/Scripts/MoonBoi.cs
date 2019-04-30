using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonBoi : MonoBehaviour {
    LookAtObject moonFace;
    GameObject lipstick, cig;
    bool gotCig, gotLip;
    [SerializeField] FaceProfile cigFace, lipFace;
	// Use this for initialization
	void Start () {
        moonFace = GetComponent<LookAtObject>();
        cig = GameObject.Find("Cig");
        lipstick = GameObject.Find("Lipstick");
	}
	
	// Update is called once per frame
	void Update () {
        if (!gotCig)
        {
            if (!cig.activeInHierarchy)
            {
                //if theres no cig object then the bois got it i guess
                moonFace.SetRandomFace(cigFace);
                gotCig = true;
            }
            
        }
        if (!gotLip)
        {

            if (!lipstick.activeInHierarchy)
            {
                moonFace.SetRandomFace(lipFace);
                gotLip = true;
            }
        }
	}
}
