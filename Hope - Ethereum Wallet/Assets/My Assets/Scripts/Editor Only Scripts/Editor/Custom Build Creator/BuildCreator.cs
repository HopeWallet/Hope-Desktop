using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class BuildCreator : EditorWindow
{

    private const string BUILD_PATH = "C:/Users/Matthew/Downloads/Test";
    private const string BUILD_NAME = "Test";

    private const string SHOULD_BUILD_PREF_NAME = "build";

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
        PlayerPrefs.SetInt(SHOULD_BUILD_PREF_NAME, 1);

        InvokeMethodsWithAttribute<ModifyCodeAttribute>();

        AssetDatabase.Refresh();
    }

    [DidReloadScripts]
    public static void Build()
    {
        if (PlayerPrefs.GetInt(SHOULD_BUILD_PREF_NAME) == 0)
            return;

        PlayerPrefs.SetInt(SHOULD_BUILD_PREF_NAME, 0);

        EnsureValidDirectory();
        CreateBuild();
    }

    [PostProcessBuild(99)]
    public static void Restore(BuildTarget target, string result)
    {
        InvokeMethodsWithAttribute<RestoreCodeAttribute>();
        AssetDatabase.Refresh();
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
    }

    private static void EnsureValidDirectory()
    {
        if (!Directory.Exists(BUILD_PATH))
            Directory.CreateDirectory(BUILD_PATH);
    }

    private static void InvokeMethodsWithAttribute<T>() where T : Attribute
    {
        AssemblyUtils.GetTypesWithMethodAttribute<ModifyCodeAttribute>()
                     .Where(ReflectionHelper.HasMethodAttributes<ModifyCodeAttribute, RestoreCodeAttribute>)
                     .ForEach(type => ReflectionHelper.InvokeAttributeMethod<T>(type, null));
    }
}
