using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CollectableType
{
    cig,
    lipstick
}
public class Collectible : MonoBehaviour {
    Campfire campRef;
    [SerializeField] CollectableType type;
    [SerializeField] FaceProfile faces;
    GameObject player1, player2;
    FMODUnity.StudioEventEmitter fmodRef;
    public LookAtObject[] p1Faces, p2Faces;
	// Use this for initialization
	void Awake () {
        campRef = GameObject.Find("Campfire").GetComponent<Campfire>();
        player1 = GameObject.Find("Player 1");
        player2 = GameObject.Find("Player 2");
        if (player1)
        {
            p1Faces = player1.GetComponentsInChildren<LookAtObject>(true);
        }
        if (player2)
        {
            p2Faces = player2.GetComponentsInChildren<LookAtObject>(true);
        }
        fmodRef = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //tell the campfire you've been collected
            switch (type)
            {
                case CollectableType.cig:
                    campRef.cigFound = true;
                    break;
                case CollectableType.lipstick:
                    campRef.lipstickFound = true;
                    break;
            }
            if (player1)
            {
                player1.GetComponentInChildren<PlayerHealth>().latestFaceProfile = faces;
                foreach (LookAtObject eachFace in p1Faces)
                {
                    eachFace.SetRandomFace(faces);
                }
            }
            if (player2)
            {

                player2.GetComponentInChildren<PlayerHealth>().latestFaceProfile = faces;
                foreach (LookAtObject eachFace in p2Faces)
                {
                    eachFace.SetRandomFace(faces);
                }
            }
            //destroy and do whatever effect you need it to do to their faces or whatever
            gameObject.SetActive(false);
            fmodRef.Play();
        }
    }
}
