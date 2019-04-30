using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// stores the data that determines which tiles are of which material
/// </summary>
[CreateAssetMenu]
public class GroundMaterialProfile : ScriptableObject {
    
    public List<TileBase> dirtTiles,grassTiles,leafTiles,woodTiles,snowTiles,rockTiles;
    public GameObject dirtParticle,grassParticle,leafParticle,woodParticle,snowParticle,rockParticle;
}
