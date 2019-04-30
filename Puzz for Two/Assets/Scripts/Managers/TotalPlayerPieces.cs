using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalPlayerPieces : MonoBehaviour {

    public static TotalPlayerPieces instance;

    public GameObject[] players;
    public GameObject[] detatchedPlayerPieces;

    public int totalPlayerPieces;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");

        List<GameObject> actualPlayers = new List<GameObject>();
        for (int i = 0; i< players.Length; i++)
        {
            if (!actualPlayers.Contains(players[i].transform.root.gameObject))
            {
                actualPlayers.Add(players[i].transform.root.gameObject);
            }
        }
        players = actualPlayers.ToArray();

        detatchedPlayerPieces = GameObject.FindGameObjectsWithTag("PlayerPiece");

        FindTotalPlayerPieces();
	}
	
    void FindTotalPlayerPieces()
    {
        for (int i = 0; i < players.Length; i++)
        {
            PlayerHealth healthScript = players[i].GetComponent<PlayerHealth>();
            totalPlayerPieces += (int)healthScript.health;
        }

        totalPlayerPieces += detatchedPlayerPieces.Length;
    }
}
