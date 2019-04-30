using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileSwapper))]
public class TileSwapperEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileSwapper myScript = (TileSwapper)target;
        if (GUILayout.Button("Swap Tile Sprites"))
        {
            myScript.SwapTiles();
        }
    }
}
