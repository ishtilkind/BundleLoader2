using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NG.TRIPSS.UI
{
    [CustomEditor((typeof(VersionInfo)))]
    public class VersionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            VersionInfo info = target as VersionInfo;
            if (null == info) return;

            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Set Version Info"))
            {
                var text = info.GetComponent<Text>();
                if (null == text) return;

                text.text = info.BuildInfo;


            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(info.GetComponent<Text>());
            }
        }
    }
}
