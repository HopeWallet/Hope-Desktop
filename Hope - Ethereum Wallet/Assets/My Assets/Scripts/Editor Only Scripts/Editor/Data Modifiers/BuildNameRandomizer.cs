using UnityEditor;
using UnityEditor.Callbacks;

/// <summary>
/// Class which randomizes the name of the company and product on build.
/// </summary>
public static class BuildNameRandomizer
{
    private static string CompanyName;
    private static string ProductName;

    /// <summary>
    /// Replaces the names with a random password.
    /// </summary>
    [PostProcessScene(1)]
    public static void ReplaceBuildNames()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        CompanyName = PlayerSettings.companyName;
        ProductName = PlayerSettings.productName;

        //SetPlayerSettings(PasswordUtils.GenerateRandomPassword(), PasswordUtils.GenerateRandomPassword());
    }

    /// <summary>
    /// Restores the names of the company and product to what they were before.
    /// </summary>
    /// <param name="target"> The target platform of the build. </param>
    /// <param name="result"> The result of the build. </param>
    [PostProcessBuild(1)]
    public static void RestoreNames(BuildTarget target, string result) => SetPlayerSettings(CompanyName, ProductName);

    /// <summary>
    /// Sets the company and product names given two parameters.
    /// </summary>
    /// <param name="companyName"> The company name to assign to the PlayerSettings. </param>
    /// <param name="productName"> The product name to assign to the PlayerSettings. </param>
    private static void SetPlayerSettings(string companyName, string productName)
    {
        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = productName;
    }
}
