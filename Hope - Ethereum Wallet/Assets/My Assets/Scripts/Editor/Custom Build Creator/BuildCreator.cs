using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class BuildCreator : EditorWindow
{

    private const string BUILD_PATH = "C:/Users/Matthew/Downloads/Test";
    private const string BUILD_NAME = "Test";

    private readonly static Stack<Type> codeModificationTypes = new Stack<Type>();

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

        var types = AssemblyUtils.GetTypesWithMethodAttribute<ModifyCodeAttribute>()
                                 .Where(ReflectionHelper.HasMethodAttributes<ModifyCodeAttribute, RestoreCodeAttribute>).ToArray();
                     //.ForEach(type =>
                     //{
                     //    codeModificationTypes.Push(type);
                     //    ReflectionHelper.InvokeAttributeMethod<ModifyCodeAttribute>(type, null);
                     //});

        for (int i = 0; i < types.Length; i++)
        {

        }

        RefreshAssets();
    }

    [DidReloadScripts]
    public static void Build()
    {
        Debug.Log("RELOADED => " + shouldBuild);
        Debug.Log(codeModificationTypes.Count);
        if (!shouldBuild)
            return;

        EnsureValidDirectory();
        CreateBuild();
        RestoreCode();
        RefreshAssets();

        shouldBuild = false;
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

    private static void RestoreCode()
    {
        while (codeModificationTypes.Count > 0)
            ReflectionHelper.InvokeAttributeMethod<RestoreCodeAttribute>(codeModificationTypes.Pop(), null);
    }

    private static void RefreshAssets()
    {
        AssetDatabase.Refresh();
    }
}
