using System;

/// <summary>
/// Class which executes wallet encryption tasks asynchronously.
/// </summary>
public static class AsyncWalletEncryption
{

	/// <summary>
	/// Gets the encryption password asynchronously.
	/// </summary>
	/// <param name="safePassword"> The SafePassword object to extract the encryption password from. </param>
	/// <param name="userPassword"> The user's password for accessing the wallet, for extra layer of security. </param>
	/// <param name="generateNew"> Whether to generate a new encryption password or extract an old one. </param>
	public static async void GetEncryptionPasswordAsync(SafePasswordStorage safePassword, string userPassword, 
        Action<string> onPasswordExtracted, bool generateNew = false)
	{
		onPasswordExtracted?.Invoke(await TaskUtils.StartStringTask(() 
            => generateNew ? safePassword.GenerateEncryptionPassword(userPassword) : safePassword.ExtractEncryptionPassword(userPassword)));
	}

    /// <summary>
    /// Encrypts the wallet's private key and seed using a password.
    /// </summary>
    /// <param name="privateKey"> The private key of the wallet. </param>
    /// <param name="phrase"> The mnemonic seed of the wallet. </param>
    /// <param name="encryptionPassword"> The password to encrypt the wallet with. </param>
    /// <param name="onWalletEncrypted"> Action called once the wallet has been encrypted. </param>
    public static async void EncryptWalletAsync(string privateKey, string phrase, string encryptionPassword, Action<WalletData> onWalletEncrypted)
    {
        var splitPvtKey = privateKey.SplitHalf();
        var splitPhrase = phrase.SplitHalf();

        var pvtKey1Task = TaskUtils.StartStringTask(() => splitPvtKey.firstHalf.AESEncrypt(encryptionPassword));
        var pvtKey2Task = TaskUtils.StartStringTask(() => splitPvtKey.secondHalf.DPEncrypt(encryptionPassword));
        var phrase1Task = TaskUtils.StartStringTask(() => splitPhrase.firstHalf.AESEncrypt(encryptionPassword));
        var phrase2Task = TaskUtils.StartStringTask(() => splitPhrase.secondHalf.DPEncrypt(encryptionPassword));

        onWalletEncrypted?.Invoke(new WalletData(await pvtKey1Task, await pvtKey2Task, await phrase1Task, await phrase2Task));
    }

    /// <summary>
    /// Decrypts the data from a WalletData object asynchronously.
    /// </summary>
    /// <param name="walletData"> The wallet to decrypt. </param>
    /// <param name="encryptionPassword"> The password used to encrypt the wallet. </param>
    /// <param name="onWalletDecrypted"> Action called once the wallet has been decrypted with the private key and seed as parameters. </param>
    public static async void DecryptWalletAsync(WalletData walletData, string encryptionPassword, Action<string, string> onWalletDecrypted)
    {
        if (walletData == null)
            return;

        var pvtKey1Task = TaskUtils.StartStringTask(() => walletData.PrivateKey1.AESDecrypt(encryptionPassword));
        var pvtKey2Task = TaskUtils.StartStringTask(() => walletData.PrivateKey2.DPDecrypt(encryptionPassword));
        var phrase1Task = TaskUtils.StartStringTask(() => walletData.Phrase1.AESDecrypt(encryptionPassword));
        var phrase2Task = TaskUtils.StartStringTask(() => walletData.Phrase2.DPDecrypt(encryptionPassword));

        onWalletDecrypted?.Invoke(await pvtKey1Task + await pvtKey2Task, await phrase1Task + await phrase2Task);
    }

}
