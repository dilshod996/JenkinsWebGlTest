using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.IO;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor;

public class WebGlBuildScript : MonoBehaviour
{
    private const string COMPANYNAME = "Salin";
    private const string PRODUCTNAME = "XlabJenkins";

    private static AddressableAssetSettings settings;

    private static string unknownKey = "UnkownKey"; // for testing what to know about the folder // now  it is also addressable version name
    private static string BuildResultName = "ServerData/Builds";

    private const string ASSETFOLDERNAME = "Assets";
    private const string AddressableProfileId = "Remote";

    

    private static readonly string[] DEFINE_SYMBOLE = new string[]
    {

    };

    [MenuItem("Build/Build_WebGL")]
    public static void Build_WebGL()
    {
        List<string> allDefines = new List<string>();

        foreach (var SYMBOLE in DEFINE_SYMBOLE)
        {
            allDefines.Add(SYMBOLE);
        }

        allDefines.Add(unknownKey);


        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", allDefines.ToArray()));

        WebGLBuild();
    }
    static void WebGLBuild()
    {
        // 빌드경로
        string buildpath = Application.dataPath.Replace(ASSETFOLDERNAME, BuildResultName);

        if (Directory.Exists(buildpath))
            Directory.Delete(buildpath, true);

        // 번들경로
        string bundleBuildpath = Application.dataPath.Replace(ASSETFOLDERNAME, "Bundles");

        if (Directory.Exists(bundleBuildpath))
            Directory.Delete(bundleBuildpath, true);

        // 어드레서블
        settings = AddressableAssetSettingsDefaultObject.Settings;
        settings.OverridePlayerVersion = unknownKey; // only for test
        settings.ContentStateBuildPath = "";

        string profileId = settings.profileSettings.GetProfileId(AddressableProfileId);
        settings.activeProfileId = profileId;

        AddressableAssetSettings.BuildPlayerContent();

        // 앱세팅

        PlayerSettings.SplashScreen.show = false;

        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WebGL, ApiCompatibilityLevel.NET_Unity_4_8);

        PlayerSettings.companyName = COMPANYNAME;
        PlayerSettings.productName = PRODUCTNAME;


        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = FindEnabledEditorScenes();

        buildPlayerOptions.locationPathName = BuildResultName;
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;


        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        BuildSummary summary = report.summary;


        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }


    static string[] FindEnabledEditorScenes()
    {
        return new string[]
        {
            "Assets/Scenes/Intro.unity",
        };
    }
}
