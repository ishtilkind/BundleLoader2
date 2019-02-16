using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor((typeof(DbInfoLoader)))]
public class DbInfoLoaderEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DbInfoLoader info = target as DbInfoLoader;
        if (null == info) return;

        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Get Data"))
        {
            info.GetData();
            //var text = info.GetComponent<Text>();
            //if (null == text) return;

            //text.text = info.BuildInfo;
        }

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Load Data"))
        {
            //info.LoadDataFromFile();

            info.Restore();
            //var text = info.GetComponent<Text>();
            //if (null == text) return;

            //text.text = info.BuildInfo;
        }
        if (GUI.changed)
        {
            //EditorUtility.SetDirty(info.GetComponent<Text>());
        }
    }
}
