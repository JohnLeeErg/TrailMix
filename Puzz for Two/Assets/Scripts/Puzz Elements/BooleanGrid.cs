using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BooleanGrid : MonoBehaviour
{
    public Tilemap field, genMap;
    public TileBase tileDenotingBaseCamper;
    public List<TileBase> tileDenotingAttachedCampers = new List<TileBase>();
    public List<Vector3> fieldCoordinates;
    public List<Vector2> characterShape;
    ButtonParent buttonParentComponent;
    Transform buttonPresser;
    //public int unitsToAdjustClipping = 1;

    // Use this for initialization
    void Start()
    {
        characterShape.Clear();
        buttonPresser = null;
        buttonParentComponent = GetComponentInParent<ButtonParent>();
        fieldCoordinates = new List<Vector3>();
        foreach (var position in field.cellBounds.allPositionsWithin)
        {
            Vector3Int coordinates = new Vector3Int(position.x, position.y, position.z);
            if (field.HasTile(coordinates))
            {
                fieldCoordinates.Add(coordinates);
            }
        }
    }

    public void GenerateMap()
    {
        Transform lastButtonPresser = buttonPresser;
        GetPresser();
        int sizeOfLastPresser = characterShape.Count;
        characterShape.Clear();
        foreach (Transform t in buttonPresser)
        {
            if (t.gameObject.activeInHierarchy)
            {
                Vector2 v = new Vector2(t.localPosition.x, t.localPosition.y);
                characterShape.Add(v);
            }
        }

        if (lastButtonPresser == buttonPresser && characterShape.Count == sizeOfLastPresser) //if its the same character at the same health don't change anything
        {
            return;
        }
        else
        {
            genMap.ClearAllTiles();
            Vector2 adjustedPlayerPosition = Vector2.zero;
            foreach (Vector3 coordinate in fieldCoordinates)
            {
                for (int i = 0; i < characterShape.Count; i++)
                {
                    TileBase tileUsed;
                    if (i == 0)
                    {
                        tileUsed = tileDenotingBaseCamper;
                    }
                    else
                    {
                        tileUsed = tileDenotingAttachedCampers[Random.Range(0, tileDenotingAttachedCampers.Count)];
                    }
                    genMap.SetTile(new Vector3Int((int)coordinate.x + (int)characterShape[i].x, (int)coordinate.y + (int)characterShape[i].y, (int)coordinate.z), tileUsed);
                }
            }
        }
        //add 
        //adjust based on 1 unit in all directions
    }

    void GetPresser()
    {
        buttonPresser = buttonParentComponent.buttonsInPuzzle[0].owner.transform.root.GetChild(0);
    }

    //RaycastHit2D CheckIfPlayerInSpace()
    //{
    //    RaycastHit2D pointCast =
    //    return pointCast;
    //}

    void PushPlayerUp()
    {

    }
}
