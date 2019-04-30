using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomGrid : MonoBehaviour {

    public Tilemap field, genMap;
    public TileBase tileClone;
    List<Vector3> fieldCoordinates;

    public float percentOfTileToNonTile;

	// Use this for initialization
	void Start () {
        fieldCoordinates = new List<Vector3>();
        foreach (var position in field.cellBounds.allPositionsWithin)
        {
            Vector3Int coordinates = new Vector3Int(position.x, position.y, position.z);
            if (field.HasTile(coordinates))
            {
                fieldCoordinates.Add(coordinates);
            }
        }

        //GenerateMap();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GenerateMap()
    {
        foreach (Vector3 coordinate in fieldCoordinates)
        {
            genMap.SetTile(new Vector3Int((int)coordinate.x, (int)coordinate.y, (int)coordinate.z), null);
            if (Random.Range(0, 100) < percentOfTileToNonTile)
            {
                genMap.SetTile(new Vector3Int((int)coordinate.x, (int)coordinate.y, (int)coordinate.z), tileClone);
            }
        }
    }
}
