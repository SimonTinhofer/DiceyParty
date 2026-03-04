using System;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;

public static class BuildScript
{
    private const string WebProfilePath = "Assets/Settings/Build Profiles/WebRelease.asset";

    // Keep this list independent from Editor Build Settings so local test scenes never affect prod builds.
    private static readonly string[] ProductionScenes =
    {
        "Assets/DiceyParty/0_Menu/0_Menu.unity",
        "Assets/DiceyParty/1_Lobby/1_Lobby.unity",
        "Assets/DiceyParty/2+_MiniGame/2_PaintTheBall/2_PaintTheBall.unity",
        "Assets/DiceyParty/2+_MiniGame/3_GrabABox/3_GrabABox.unity",
        "Assets/DiceyParty/2+_MiniGame/4_CoinDilemma/4_CoinDilemma.unity",
        "Assets/DiceyParty/2+_MiniGame/5_RollOff/5_RollOff.unity",
        "Assets/DiceyParty/2+_MiniGame/6_QuickMath/6_QuickMath.unity",
        "Assets/DiceyParty/2+_MiniGame/7_TugTheRope/7_TugTheRope.unity",
    };

    [MenuItem("Build/All Production (WebGL + Edgegap)")]
    public static void BuildAllProduction()
    {
        BuildWebClient();
        BuildEdgegapServer();
    }

    [MenuItem("Build/Web Client")]
    public static void BuildWebClient()
    {
        BuildProfile buildProfile = AssetDatabase.LoadAssetAtPath<BuildProfile>(WebProfilePath);
        if (buildProfile == null)
        {
            throw new Exception($"Web build profile not found: {WebProfilePath}");
        }

        var options = new BuildPlayerWithProfileOptions
        {
            buildProfile = buildProfile,
            locationPathName = "Builds/WebClient",
            options = BuildOptions.None,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"WebGL build failed: {report.summary.result}");
        }

        UnityEngine.Debug.Log($"WebGL build succeeded: {report.summary.totalSize} bytes");
    }

    [MenuItem("Build/Edgegap Server")]
    public static void BuildEdgegapServer()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetValidatedProductionScenes(),
            target = BuildTarget.StandaloneLinux64,
            subtarget = (int)StandaloneBuildSubtarget.Server,
            locationPathName = "Builds/EdgegapServer/ServerBuild"
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            UnityEngine.Debug.LogError($"Edgegap server build failed: {report.summary.result}");
            throw new Exception("Edgegap server build failed.");
        }

        UnityEngine.Debug.Log("Edgegap server build succeeded.");
    }

    private static string[] GetValidatedProductionScenes()
    {
        foreach (string scenePath in ProductionScenes)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
                throw new Exception($"Build scene not found: {scenePath}");
            }
        }

        return ProductionScenes;
    }
}
