using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class WebGlBuildScript
{
    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        // Set player settings
        PlayerSettings.companyName = "Salin";
        PlayerSettings.productName = "XlabJenkins";

        // Build Addressables
        BuildAddressables();

        // Set the target build path
        string buildPath = "Build/WebGL";

        string[] IncludedScenes = {
            "Assets/Scenes/Intro.unity",
        };


       // string logFilePath = System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("WORKSPACE"), "LogFiles", "build.log");
        // Ensure the build directory exists
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = IncludedScenes,
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };

        // Build the WebGL project
        BuildPipeline.BuildPlayer(buildOptions);
    }

    private static void BuildAddressables()
    {
        // Build Addressables
        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
        AddressableAssetSettings.BuildPlayerContent();
    }
}
