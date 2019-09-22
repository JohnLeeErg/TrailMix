using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TileSwapper))]
[CanEditMultipleObjects]
public class TileSwapperEditor : Editor
{
    Texture2D[] m_Textures; //test if it fixes the render issuelk

    public override void OnInspectorGUI()
    {
        SerializedProperty tilesReplacedProperty = this.serializedObject.FindProperty("tilesToReplace");
        SerializedProperty tilesFilledProperty = this.serializedObject.FindProperty("tilesToFill");

        DrawDefaultInspector();

        TileSwapper myScript = (TileSwapper)target;

        tilesReplacedProperty.arraySize = EditorGUILayout.IntField("Number of Tiles", tilesReplacedProperty.arraySize);

        serializedObject.ApplyModifiedProperties();

        if (tilesReplacedProperty.arraySize > 0)
        {
            if (GUILayout.Button("Swap All Tile Sprites", GUILayout.Height(30)))
            {
                myScript.SwapTiles(-1);
            }

            for (int i = 0; i < tilesReplacedProperty.arraySize; i++)
            {

                EditorGUILayout.BeginVertical("Button");
                EditorGUILayout.BeginHorizontal();

                if (myScript.tilesToReplace[i] != null)
                {
                    Texture2D myTexture;
                    myTexture = AssetPreview.GetAssetPreview(myScript.tilesToReplace[i]);
                    GUILayout.Label(myTexture, GUILayout.Width(50), GUILayout.Height(50));
                }
                else
                {
                    GUILayout.Label("Missing Tile " + i);
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label("Swap To ->"); //center this text
                GUILayout.FlexibleSpace();

                if (i < myScript.tilesToFill.Length)
                {
                    if (myScript.tilesToFill[i] != null)
                    {
                        Texture2D myTextureFill;
                        myTextureFill = AssetPreview.GetAssetPreview(myScript.tilesToFill[i]);
                        GUILayout.Label(myTextureFill, GUILayout.Width(50), GUILayout.Height(50));
                    }
                    else
                    {
                        GUILayout.Label("Missing Tile " + i);
                    }
                }
                else
                {
                    GUILayout.Label("OUT OF RANGE");
                }

                EditorGUILayout.EndHorizontal();

                SerializedProperty m_TileTestReplace = tilesReplacedProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(m_TileTestReplace, new GUIContent("Tile To Swap"));

                SerializedProperty m_TileTestFill = tilesFilledProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(m_TileTestFill, new GUIContent("Tile To Change To"));

                serializedObject.ApplyModifiedProperties();

                Color c = GUI.backgroundColor;

                if (m_TileTestReplace.objectReferenceValue != null && m_TileTestFill.objectReferenceValue != null)
                {
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Swap " + m_TileTestReplace.objectReferenceValue.name + " with " + m_TileTestFill.objectReferenceValue.name))
                    {
                        myScript.SwapTiles(i);
                    }
                }
                else
                {
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Cannot swap with Null tile value"))
                    {
                        Debug.LogError("You cannot swap a tile with a null value, or vice versa. Please check element " + i + " of your Tile Swapper.");
                    }
                }

                GUI.backgroundColor = c; //reset color to norm
                EditorGUILayout.EndVertical();
            }
        }
    }
}
