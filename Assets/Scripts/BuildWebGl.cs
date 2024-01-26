using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildWebGl
{
    [MenuItem("Build/WebGL")]
    public static void BuildWebGL()
    {
        string[] scenes = GetScenePaths(); // Add your scene paths here
        string buildPath = "Builds/WebGLBuild"; // Change this path as needed
        BuildTarget buildTarget = BuildTarget.WebGL;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = buildTarget,
            options = BuildOptions.None
        };

        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = buildReport.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("WebGL build succeeded: " + buildReport.summary.totalSize + " bytes");
        }
        else
        {
            UnityEngine.Debug.LogError("WebGL build failed with errors!" + buildReport.summary.totalErrors);
        }
    }

    private static string[] GetScenePaths()
    {
        // Add your scene paths here
        return new string[]
        {
            "Assets/Scenes/Scene1.unity",
            "Assets/Scenes/Scene2.unity"
            // Add more scenes as needed
        };
    }
}
