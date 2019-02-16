using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace NG.TRIPSS.BUILD
{
    class MyCustomBuildProcessor : IPreprocessBuildWithReport, IProcessSceneWithReport
    {
        //public Text versionInfo;

        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            //versionInfo.text = Time.time.ToString();
            Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target " + report.summary.platform + " at path " +
                      report.summary.outputPath);
        }

        public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
        {
            //Debug.Log(Camera.main.name);
            Debug.Log("MyCustomBuildProcessor.OnProcessScene " + scene.name);
        }
    }
}