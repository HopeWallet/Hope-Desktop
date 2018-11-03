using Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base;
using System.IO;
using UnityEditor;
using UnityEngine;

public class OfficialBuildCreator : EditorWindow
{
    private string version,
                   buildPath;

    [MenuItem("Window/Official Build Creator")]
    public static void Init()
    {
        OfficialBuildCreator window = (OfficialBuildCreator)GetWindow(typeof(OfficialBuildCreator));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Wallet Version");
        version = GUILayout.TextField(version);

        GUILayout.Space(10f);

        if (GUILayout.Button("BUILD"))
            CreateBuild();
    }

    private void CreateBuild()
    {
        var appSettings = Resources.Load("AppSettings") as AppSettingsInstaller;
        var securePlayerPrefsSettings = Resources.Load("Settings/SecuredPlayerPrefsSettings") as SecurePlayerPrefsSettings;
        buildPath = (Resources.Load("Data/build_path") as TextAsset)?.text;

        appSettings.playerPrefSettings.securedPlayerPrefsSettings = securePlayerPrefsSettings;
        appSettings.versionSettings.version = version;

        EnsureValidDirectory();

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            options = BuildOptions.None,
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone,
            locationPathName = $@"{GetBuildPath()}\Hope {version}.exe"
        };

        BuildPipeline.BuildPlayer(options);

        appSettings.playerPrefSettings.securedPlayerPrefsSettings = null;
    }

    private void EnsureValidDirectory()
    {
        if (Directory.Exists(GetBuildPath()))
            Directory.Delete(GetBuildPath());

        Directory.CreateDirectory(GetBuildPath());
    }

    private string GetBuildPath()
    {
        return $"{buildPath} {version}";
    }
}