using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.EthereumUtils;
using Nethereum.HdWallet;
using System;
using System.Linq;
using System.Threading.Tasks;
using SecureRandom = Org.BouncyCastle.Security.SecureRandom;

public class WalletCreator : WalletLoaderBase
{

    private static readonly string WALLET_NUM_PREF = HashGenerator.GetSHA512Hash("wallet_count");

    private string[] encryptedHashLvls;
    private string saltedPasswordHash;
    private string encryptedSeed;

    protected override string LoadingText => "Creating wallet...";

    public WalletCreator(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        ProtectedStringDataCache protectedStringDataCache) : base(popupManager, playerPrefPassword, protectedStringDataCache)
    {
    }

    protected override void LoadWallet(object data, string userPass)
    {
        CreateWalletCountPref();
        TryCredentials((string)data, userPass);
    }

    private void SetWalletPlayerPrefs()
    {
        UnityEngine.Debug.Log("pt. 9");

        int walletNum = SecurePlayerPrefs.GetInt(WALLET_NUM_PREF) + 1;

        SecurePlayerPrefs.SetInt(WALLET_NUM_PREF, walletNum);
        SecurePlayerPrefs.SetString(PasswordEncryption.PWD_PREF_NAME + "_" + walletNum, saltedPasswordHash);
        SecurePlayerPrefs.SetString("wallet_" + walletNum, encryptedSeed);

        for (int i = 0; i < encryptedHashLvls.Length; i++)
            SecurePlayerPrefs.SetString("wallet_" + walletNum + "_h" + (i + 1), encryptedHashLvls[i]);

        UnityEngine.Debug.Log("pt. 10");

        ClearSensitiveData();

        playerPrefPassword.SetupPlayerPrefs(walletNum, onWalletLoaded);
        //playerPrefPassword.SetupPlayerPrefs(walletNum, () => { onWalletLoaded?.Invoke(); UnityEngine.Debug.Log("WALLET #" + walletNum + " => " + addresses[0].CreateDisposableData().Value); });
    }

    private void ClearSensitiveData()
    {
        Array.Clear(encryptedHashLvls, 0, encryptedHashLvls.Length);

        encryptedHashLvls = null;
        saltedPasswordHash = null;
        encryptedSeed = null;

        GC.Collect();
    }

    private void CreateWalletCountPref()
    {
        if (!SecurePlayerPrefs.HasKey(WALLET_NUM_PREF))
            SecurePlayerPrefs.SetInt(WALLET_NUM_PREF, 0);
    }

    /// <summary>
    /// Attempts to create a wallet given a mnemonic phrase.
    /// </summary>
    /// <param name="mnemonic"> The phrase to attempt to create a wallet with. </param>
    /// <param name="basePass"> The password that was entered by the user. </param>
    private void TryCredentials(string mnemonic, string basePass)
    {
        if (string.IsNullOrEmpty(basePass) || basePass.Length < AESEncryption.MIN_PASSWORD_LENGTH)
        {
            ExceptionManager.DisplayException(new Exception("Invalid wallet password. Please use a password with more than 8 characters!"));
            return;
        }

        try
        {
            var wallet = new Wallet(mnemonic, null, WalletUtils.DetermineCorrectPath(mnemonic));
            AsyncTaskScheduler.Schedule(() => GetAddresses(wallet));
            AsyncTaskScheduler.Schedule(() => EncryptWalletData(wallet.Seed, basePass));
        }
        catch
        {
            ExceptionManager.DisplayException(new Exception("Unable to create wallet with that phrase. Please try again."));
        }
    }

    private async Task EncryptWalletData(byte[] seed, string basePass)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.GenerateEncryptionPassword(basePass).GetSHA256Hash()).ConfigureAwait(false); UnityEngine.Debug.Log("pt. 1");
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.Item1.SplitHalf();
        var lvl34string = splitPass.Item2.SplitHalf();

        SecureRandom secureRandom = new SecureRandom();

        string h1 = await Task.Run(() => lvl12string.Item1.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false); UnityEngine.Debug.Log("pt. 2");
        string h2 = await Task.Run(() => lvl12string.Item2.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false); UnityEngine.Debug.Log("pt. 3");
        string h3 = await Task.Run(() => lvl34string.Item1.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false); UnityEngine.Debug.Log("pt. 4");
        string h4 = await Task.Run(() => lvl34string.Item2.GetSHA256Hash().CombineAndRandomize(SecureRandom.GetNextBytes(secureRandom, 30).GetHexString())).ConfigureAwait(false); UnityEngine.Debug.Log("pt. 5");
        encryptedSeed = await Task.Run(() => seed.GetHexString().AESEncrypt(h1 + h2).Protect(h3 + h4)).ConfigureAwait(false); UnityEngine.Debug.Log("pt. 6");
        saltedPasswordHash = await Task.Run(() => PasswordEncryption.GetSaltedPasswordHash(basePass)).ConfigureAwait(false); UnityEngine.Debug.Log("pt. 7");
        encryptedHashLvls = await Task.Run(() => new string[] { h1.AESEncrypt(lvl12string.Item1.GetSHA512Hash()), h2.Protect(), h3.Protect(), h4.AESEncrypt(lvl34string.Item2.GetSHA512Hash()) }).ConfigureAwait(false); UnityEngine.Debug.Log("pt. 8");

        MainThreadExecutor.QueueAction(SetWalletPlayerPrefs, "setting wallet player prefs");
    }
}
