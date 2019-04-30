using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class OutLineGenerator : MonoBehaviour
{
    public Tilemap tileMapRef;
    Tilemap myTiles;
    TileBase[] allTilesInMap; //the best part is this is non runtime so I can be sloppppy
    //[SerializeField] bool doTheThing;
    [SerializeField] OutLineType[] allOutlineTypes;
    [Tooltip("uses this one if it doesnt find any matches")]
    [SerializeField] OutLineType fallbackType;
    BoundsInt bounds;

    public void GenerateOutlines()
    {
        //if (!doTheThing)
        //{
        //    return;
        //}

        
            myTiles = GetComponent<Tilemap>();
            tileMapRef.CompressBounds();

            myTiles.ClearAllTiles();

            bounds = tileMapRef.cellBounds;
            foreach (Vector3Int position in tileMapRef.cellBounds.allPositionsWithin)
            {

                if (doesTileExist(position))
                {
                    UpdateSpriteBasedOnNeighbours(position);
                }
            }

            myTiles.RefreshAllTiles();
        
    }

    bool doesTileExist(Vector3Int pos)
    {
        if (tileMapRef.GetTile(pos))
        {

            return true;


        }
        else
        {
            return false;
        }
    }

    void UpdateSpriteBasedOnNeighbours(Vector3Int pos)
    {
        TileBase tile = tileMapRef.GetTile(pos);
        Vector4 identifier = new Vector4(0, 0, 0, 0);
        if (doesTileExist(pos + Vector3Int.up)) //up
        {
            identifier.x = 1;
        }
        if (doesTileExist(pos + Vector3Int.down))
        { //down
            identifier.y = 1;
        }
        if (doesTileExist(pos + Vector3Int.left)) //left
        {
            identifier.z = 1;
        }
        if (doesTileExist(pos + Vector3Int.right)) //right
        {
            identifier.w = 1;
        }
        //print(identifier);
        
        //now figure out which one to use
        foreach (OutLineType eachType in allOutlineTypes)
        {
            if (eachType.identifier == identifier)
            {
                //print(identifier + " is a type! set something!");
                myTiles.SetTile(pos, eachType.variants[Random.Range(0, eachType.variants.Length)]);
                return;
            }
        }
        //if you didnt match any
        myTiles.SetTile(pos, fallbackType.variants[Random.Range(0, fallbackType.variants.Length)]);
    }
}
