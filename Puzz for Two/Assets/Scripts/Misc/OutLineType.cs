using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
class OutLineType : ScriptableObject
{

    [Header("up, down, left, right")]
    [Tooltip("this is the scenario in which this type of tile should be generated. order is up down left right")]
    public Vector4 identifier;
    [Tooltip("a random tile from this list will be used")]
    public Tile[] variants;
    
}