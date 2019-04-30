using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerPieceAnimatorManager))]
public class PlayerPieceAnimatorEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerPieceAnimatorManager myScript = (PlayerPieceAnimatorManager)target;
        if (GUILayout.Button("Fix Player Sprites"))
        {
            myScript.FixPlayerSprites();
        }
    }
}
