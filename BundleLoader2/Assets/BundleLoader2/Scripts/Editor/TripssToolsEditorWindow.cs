using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class TripssToolsEditorWindow : EditorWindow
{
    [MenuItem("Tripss/Tools Window")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TripssToolsEditorWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Convenience Controls", EditorStyles.boldLabel);

        GUI.backgroundColor = EditorApplication.isCompiling ? Color.yellow : Color.green;
        this.Repaint();

        GUI.enabled = !EditorApplication.isCompiling;

        if (GUILayout.Button("Set Version Info"))
        {
            //UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            //List<GameObject> rootObjects = new List<GameObject>(scene.rootCount + 1);
            //scene.GetRootGameObjects(rootObjects);

            foreach (var vi in FindObjectsOfType<NG.TRIPSS.UI.VersionInfo>())
            {
                var text = vi.GetComponent<Text>();
                if (null != text)
                {
                    text.text = vi.BuildInfo;
                    EditorUtility.SetDirty(text);
                }                 
                else
                {
                    Debug.LogError($"VersionInfo text not found");
                }
            }

            EditorSceneManager.SaveOpenScenes();
        }

    }
}
