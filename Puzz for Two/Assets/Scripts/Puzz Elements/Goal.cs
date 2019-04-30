using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public List<GameObject> playersInZone = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D collided)
    {
        if(collided.gameObject.tag == "Player")
        {
            if (!playersInZone.Contains(collided.gameObject))
            {
                playersInZone.Add(collided.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collided)
    {
        if (collided.gameObject.tag == "Player")
        {
            if (playersInZone.Contains(collided.gameObject))
            {
                playersInZone.Remove(collided.gameObject);
            }
        }
    }

    void Update()
    {
        if (playersInZone.Count >= 2)
        {
            Level1Manager.instance.LoadNextScene();
        }
    }
}
