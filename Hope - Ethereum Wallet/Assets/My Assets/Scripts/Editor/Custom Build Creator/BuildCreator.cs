using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class BuildCreator : EditorWindow
{

    private const string BUILD_PATH = "C:/Users/Matthew/Downloads/Test";
    private const string BUILD_NAME = "Test";

    private static bool shouldBuild;

    [MenuItem("Window/Build Creator")]
    public static void Init()
    {
        BuildCreator window = (BuildCreator)GetWindow(typeof(BuildCreator));
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("BUILD"))
            ModifyScripts();
    }

    private static void ModifyScripts()
    {
        shouldBuild = true;
    }

    [DidReloadScripts]
    public static void Build()
    {
        if (!shouldBuild)
            return;

        EnsureValidDirectory();
        CreateBuild();
    }

    private static void CreateBuild()
    {
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            options = BuildOptions.None,
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone,
            locationPathName = BUILD_PATH + "/" + BUILD_NAME + ".exe"
        };

        BuildPipeline.BuildPlayer(options);

        shouldBuild = false;
    }

    private static void EnsureValidDirectory()
    {
        if (!Directory.Exists(BUILD_PATH))
            Directory.CreateDirectory(BUILD_PATH);
    }
}
