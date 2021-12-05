using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            mapGen.DrawMapInEditor();
        }
        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
    }
}
