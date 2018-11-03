using Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class OfficialBuildCreator : EditorWindow
{
    private const string BUILD_PATH = "C:/Users/Matthew/Downloads/Hope";

    private static AppSettingsInstaller AppSettings;
    private static SecurePlayerPrefsSettings SecurePlayerPrefsSettings;

    [MenuItem("Window/Official Build Creator")]
    public static void Init()
    {
        AppSettings = Resources.Load("AppSettings") as AppSettingsInstaller;
        SecurePlayerPrefsSettings = Resources.Load("Settings/SecuredPlayerPrefsSettings") as SecurePlayerPrefsSettings;

        OfficialBuildCreator window = (OfficialBuildCreator)GetWindow(typeof(OfficialBuildCreator));
        window.Show();
    }

    private void OnGUI()
    {
        var version = GUILayout.TextField(AppSettings.versionSettings.version);

        if (GUILayout.Button("BUILD"))
            CreateBuild(version);
    }

    private static void CreateBuild(string version)
    {
        AppSettings.playerPrefSettings.securedPlayerPrefsSettings = SecurePlayerPrefsSettings;
        AppSettings.versionSettings.version = version;

        EnsureValidDirectory(version);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            options = BuildOptions.None,
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone,
            locationPathName = $"{GetBuildPath(version)}/Hope {version}.exe"
        };

        BuildPipeline.BuildPlayer(options);

        AppSettings.playerPrefSettings.securedPlayerPrefsSettings = null;
    }

    private static void EnsureValidDirectory(string version)
    {
        if (!Directory.Exists(GetBuildPath(version)))
            Directory.CreateDirectory(GetBuildPath(version));
    }

    private static string GetBuildPath(string version)
    {
        return $"{BUILD_PATH} {version}";
    }
}