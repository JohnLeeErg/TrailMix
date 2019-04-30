using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSwapper : MonoBehaviour
{
    Tilemap myTiles;
    TileBase[] allTilesInMap; //the best part is this is non runtime so I can be sloppppy
    BoundsInt bounds;
    [SerializeField] OutLineType tileToReplace;
    [SerializeField] OutLineType tileToFill;
    Dictionary<Tile, int> tileToReplaceDict = new Dictionary<Tile, int>();
    Dictionary<int, Tile> tileToFillDict = new Dictionary<int, Tile>();

    public void SwapTiles()
    {
        myTiles = GetComponent<Tilemap>();
        myTiles.CompressBounds();
        tileToReplaceDict.Clear();
        tileToFillDict.Clear();
        FillTileDictionary();
        bounds = myTiles.cellBounds;
        foreach (Vector3Int position in myTiles.cellBounds.allPositionsWithin)
        {
            if (doesTileExist(position))
            {
                UpdateSprite(position);
            }
        }

        myTiles.RefreshAllTiles();
    }

    bool doesTileExist(Vector3Int pos)
    {
        if (myTiles.GetTile(pos))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdateSprite(Vector3Int pos)
    {
        TileBase tile = myTiles.GetTile(pos);
        if (tileToReplaceDict.ContainsKey((Tile)tile))
        {
            myTiles.SetTile(pos, tileToFillDict[tileToReplaceDict[(Tile)tile]]);
        }
    }

    void FillTileDictionary()
    {
        for (int i = 0; i < tileToReplace.variants.Length; i++)
        {
            if(i >= tileToFill.variants.Length)
            {
                Debug.LogWarning("Your tileToFill scriptable object you're trying to swap with is smaller than your tileToReplace. Did not replace tiles beyond tileToFill Element " + (i - 1));
                break;
            }
            tileToReplaceDict.Add(tileToReplace.variants[i], i);
            tileToFillDict.Add(i, tileToFill.variants[i]);
        }
    }
}
