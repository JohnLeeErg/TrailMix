using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OutLineGenerator))]
public class OutlineGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        OutLineGenerator myScript = (OutLineGenerator)target;
        if (GUILayout.Button("Generate Outline Sprites"))
        {
            myScript.GenerateOutlines();
        }
    }
}
