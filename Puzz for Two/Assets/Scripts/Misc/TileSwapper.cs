using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSwapper : MonoBehaviour
{
    public enum TargetType
    {
        Self,
        Children,
    }

    public TargetType target;

    [HideInInspector]
    public bool canRevert;

    public Tile[] tilesToReplace;
    public Tile[] tilesToFill;

    [HideInInspector]
    public List<Tilemap> tileMaps = new List<Tilemap>();
    TileBase[] allTilesInMap;
    BoundsInt bounds;

    //Dictionaries for Tiles to replace
    Dictionary<Tile, int> tileToReplaceDict = new Dictionary<Tile, int>();
    Dictionary<int, Tile> tileToFillDict = new Dictionary<int, Tile>();

    public void SwapTiles(int index)
    {
        Tilemap myTiles;
        tileMaps.Clear();

        switch (target)
        {
            case TargetType.Self:
                myTiles = GetComponent<Tilemap>();
                if (myTiles)
                {
                    myTiles.CompressBounds();
                    tileToReplaceDict.Clear();
                    tileToFillDict.Clear();
                    FillTileDictionary(index);
                    bounds = myTiles.cellBounds;
                    foreach (Vector3Int position in myTiles.cellBounds.allPositionsWithin)
                    {
                        if (doesTileExist(position, myTiles))
                        {
                            UpdateSprite(position, myTiles);
                        }
                    }
                    myTiles.RefreshAllTiles();
                }
                else
                {
                    Debug.LogError("TileSwapper is Missing a Tilemap Reference on it's own Object");
                }
                break;

            case TargetType.Children:
                foreach (Transform t in transform)
                {
                    Tilemap childTileMap = t.GetComponent<Tilemap>();
                    if (childTileMap)
                    {
                        tileMaps.Add(childTileMap);
                    }
                }
                for (int i = 0; i < tileMaps.Count; i++)
                {
                    myTiles = tileMaps[i].GetComponent<Tilemap>();
                    if (myTiles)
                    {
                        myTiles.CompressBounds();
                        tileToReplaceDict.Clear();
                        tileToFillDict.Clear();
                        FillTileDictionary(index);
                        bounds = myTiles.cellBounds;
                        foreach (Vector3Int position in myTiles.cellBounds.allPositionsWithin)
                        {
                            if (doesTileExist(position, myTiles))
                            {
                                UpdateSprite(position, myTiles);
                            }
                        }
                        myTiles.RefreshAllTiles();
                    }
                }
                break;
        }
    }

    bool doesTileExist(Vector3Int pos, Tilemap tileMapReference)
    {
        if (tileMapReference.GetTile(pos))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdateSprite(Vector3Int pos, Tilemap tileMapReference)
    {
        TileBase tile = tileMapReference.GetTile(pos);
        if (tileToReplaceDict.ContainsKey((Tile)tile))
        {
            tileMapReference.SetTile(pos, tileToFillDict[tileToReplaceDict[(Tile)tile]]);
        }
    }

    void FillTileDictionary(int index)
    {
        if (index < 0) //if -1, assume all
        {
            for (int i = 0; i < tilesToReplace.Length; i++)
            {
                if (i >= tilesToFill.Length)
                {
                    Debug.LogWarning("Your tileToFill scriptable object you're trying to swap with is smaller than your tileToReplace. TileSwapper did not replace tiles beyond tileToFill Element " + (i - 1));
                    break;
                }
                if (tilesToReplace[i] != null && tilesToFill[i] != null)
                {
                    tileToReplaceDict.Add(tilesToReplace[i], i);
                    tileToFillDict.Add(i, tilesToFill[i]);
                }
            }
        }
        else if (index >= 0)
        {
            tileToReplaceDict.Add(tilesToReplace[index], index);
            tileToFillDict.Add(index, tilesToFill[index]);
        }
    }
}
