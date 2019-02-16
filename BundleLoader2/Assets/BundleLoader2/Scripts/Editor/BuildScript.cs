using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;


// https://www.youtube.com/watch?v=1zROlULebXg
public class BuildScript : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    private static System.DateTime startTime;

    [MenuItem("Custom Utilities/Build WebGL")]
    static void PerformBuild()
    {
        string[] defaultScene = GetScenes(); // { "Assets/BundleLoader/Scenes/Main.unity" };
        foreach (var scene in defaultScene)
        {
            Debug.Log($"Scene {scene} is included in the build ");
        }

        startTime = System.DateTime.Now;

        BuildPipeline.BuildPlayer(defaultScene, "./Builds/WebGL",
            BuildTarget.WebGL, BuildOptions.None);
    }
    [MenuItem("Custom Utilities/Build AssetBundles WebGL")]
    static void PerformAssetBundleBuild()
    {
        BuildPipeline.BuildAssetBundles("./AssetBundles/WebGL/",
            // BuildAssetBundleOptions.UncompressedAssetBundle,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.WebGL);
    }

    // https://gamedev.stackexchange.com/questions/162921/unity-webgl-accessing-assetbundles-to-load-prefab-using-google-chrome-firef
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "./AssetBundles/WebGL/";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
    }

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log($"Postprocess -> Path To Built Project: {pathToBuiltProject}");
        var endTime = System.DateTime.Now;
        System.TimeSpan span = endTime - startTime;
        Debug.Log($"Build time is {span.Minutes} minutes and {span.Seconds} seconds.");

        // "   {0,-35} {1,20:N0}" -f "Value of Seconds Component:", $Interval.Seconds

    }

    // https://mrlparker.com/tutorial-build-scripts-and-unity/
    private static string[] GetScenes()
    {
        return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
    }

    public int callbackOrder { get; }
    public void OnPostprocessBuild(BuildReport report)
    {
        // Debug.Log($"Postprocess -> Path To Built Project: {pathToBuiltProject}");
        var endTime = System.DateTime.Now;
        System.TimeSpan span = endTime - startTime;

        var secondOpinion = report.summary.buildEndedAt - report.summary.buildStartedAt;

        Debug.Log($"Build time is {span.Minutes} minutes and {span.Seconds} seconds. {report.summary.buildEndedAt}");

        Debug.Log($"Build time reported {secondOpinion.Minutes} minutes and {secondOpinion.Seconds} seconds. {report.summary.buildEndedAt}");
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        startTime = System.DateTime.Now;
        Debug.Log($"Starting build for {report.summary.platform}");

    }
}
