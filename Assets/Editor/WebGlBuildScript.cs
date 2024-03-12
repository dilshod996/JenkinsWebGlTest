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


    private const string branchKeyGit = "-branchkey";
    private static string branchKeyInUse = string.Empty;
    private static string branchKey = "DEV"; // for testing 

    // WebGl build folder and after build all files will be inside of the folder
    private static string BuildResultName = "Builds";


    private const string ASSETFOLDERNAME = "Assets";
    private const string AddressableProfileId = "Remote";

    private static string Version = string.Empty;
    // Working with branches for addressables and build files/ must include brach key here
    private const string Devbranch = "DeveloperMode";

    
    // explain which symbolssalin@2020
    private static readonly string[] DEFINE_SYMBOLE = new string[]
    {

    };

    [MenuItem("Build/Build_WebGL")]
    public static void Build_WebGL()
    {
        // if needed to add symbols
        //branchKeyInUse = GetArgumentFromJenkinsCommandLine(branchKeyGit);
        //if (string.IsNullOrEmpty(branchKeyInUse))
        //{
        //    Debug.Log("BranchKey is not exist");
        //    return;
        //}
        List<string> allDefines = new List<string>();

        foreach (var SYMBOLE in DEFINE_SYMBOLE)
        {
            allDefines.Add(SYMBOLE);
        }

        allDefines.Add(branchKey);


        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", allDefines.ToArray()));

        WebGLBuild();
    }
    static void WebGLBuild()
    {



        Version = PlayerSettings.bundleVersion;
        // Addressablesss
        settings = AddressableAssetSettingsDefaultObject.Settings;
        settings.OverridePlayerVersion = branchKey; // only for test you can change it with actually branch key name
        settings.ContentStateBuildPath = "";
        string addressableBuildLoc = Path.Combine(Devbranch, Version, "WebGL");
       
        //settings.buildSettings.bundleBuildPath = addressableBuildLoc;
        string profileId = settings.profileSettings.GetProfileId(Devbranch);
        settings.activeProfileId = profileId;
        settings.profileSettings.SetValue(profileId, "RemoteBuildPath", addressableBuildLoc);

        AddressableAssetSettings.BuildPlayerContent();

        // App Settings

        PlayerSettings.SplashScreen.show = false;

        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WebGL, ApiCompatibilityLevel.NET_Unity_4_8);

        PlayerSettings.companyName = COMPANYNAME;
        PlayerSettings.productName = PRODUCTNAME;


        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = FindEnabledEditorScenes();

        buildPlayerOptions.locationPathName =Devbranch +"/"+ Version + "/"+ BuildResultName;
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
    static string GetArgumentFromJenkinsCommandLine(string name)
    {
        var _angs = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < _angs.Length; i++)
        {
            if (_angs[i] == name && _angs.Length > i + 1)
            {
                return _angs[i + 1];
            }
        }

        return null;
    }
}
