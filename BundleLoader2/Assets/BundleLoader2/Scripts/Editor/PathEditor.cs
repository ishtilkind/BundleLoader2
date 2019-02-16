using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NG.TRIPSS.UI
{
    [CustomEditor((typeof(PathInfo)))]
    public class PathEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PathInfo info = target as PathInfo;
            if (null == info) return;

            GUI.backgroundColor = Color.green;

            if (GUILayout.Button("Set Path Info"))
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