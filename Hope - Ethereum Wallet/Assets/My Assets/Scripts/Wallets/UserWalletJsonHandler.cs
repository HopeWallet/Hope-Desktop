using UnityEngine;

/// <summary>
/// Class used for saving and loading WalletData.
/// </summary>
public static class UserWalletJsonHandler
{

    private static readonly string PATH = @"" + Application.dataPath + "/WalletStorage.json";

    /// <summary>
    /// Whether the wallet exists in json format or not.
    /// </summary>
    public static bool JsonWalletExists { get { return FileUtils.ReadFileText(PATH) != null; } }

    /// <summary>
    /// Gets the saved WalletData.
    /// </summary>
    /// <returns> The user's WalletData. </returns>
    public static WalletData GetWallet() => JsonUtils.GetJsonData<WalletData>(FileUtils.ReadFileText(PATH));

    /// <summary>
    /// Creates a json representation of the user's WalletData.
    /// </summary>
    /// <param name="walletData"> The user's WalletData. </param>
    public static void CreateWallet(WalletData walletData)
    {
        if (walletData != null)
            JsonUtils.WriteJsonFile(walletData, PATH);
    }
}
