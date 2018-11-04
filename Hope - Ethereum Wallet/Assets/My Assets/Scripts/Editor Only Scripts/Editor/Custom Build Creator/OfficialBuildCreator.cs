using Hope.Security.ProtectedTypes.SecurePlayerPrefs.Base;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class OfficialBuildCreator : EditorWindow
{
    private string version,
                   buildPath;

    private static AppSettingsInstaller AppSettings;
    private static SecurePlayerPrefsSettings SecurePlayerPrefsSettings;

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

    [PostProcessBuild(1)]
    private static void RestorePlayerPrefSettings(BuildTarget buildTarget, string result)
    {
        AppSettings.playerPrefSettings.securedPlayerPrefsSettings = null;
        AssetDatabase.Refresh();
    }

    private void CreateBuild()
    {
        AppSettings = Resources.Load("AppSettings") as AppSettingsInstaller;
        SecurePlayerPrefsSettings = Resources.Load("Settings/SecuredPlayerPrefsSettings") as SecurePlayerPrefsSettings;
        buildPath = (Resources.Load("Data/build_path") as TextAsset)?.text;

        EnsureValidDirectory();

        AppSettings.playerPrefSettings.securedPlayerPrefsSettings = SecurePlayerPrefsSettings;

        if (!AppSettings.versionSettings.version.EqualsIgnoreCase(version))
            AppSettings.versionSettings.version = version;

        AssetDatabase.Refresh();

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            options = BuildOptions.None,
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone,
            locationPathName = $@"{GetBuildPath()}\Hope-{version}.exe"
        };

        BuildPipeline.BuildPlayer(options);
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