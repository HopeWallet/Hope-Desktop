using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Utils.EthereumUtils;
using Nethereum.HdWallet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class WalletCreator : IUpdater
{
    private static readonly string WALLET_NUM_PREF = HashGenerator.GetSHA512Hash("wallet_count");

    private readonly PlayerPrefPassword playerPrefPassword;
    private readonly ProtectedStringDataCache protectedStringDataCache;
    private readonly UpdateManager updateManager;

    private PrefData walletCreationData;

    private Action onWalletCreated;

    private bool completed;

    public WalletCreator(PlayerPrefPassword playerPrefPassword, ProtectedStringDataCache protectedStringDataCache, UpdateManager updateManager)
    {
        this.playerPrefPassword = playerPrefPassword;
        this.protectedStringDataCache = protectedStringDataCache;
        this.updateManager = updateManager;

        walletCreationData = new PrefData();
    }

    public void UpdaterUpdate()
    {
        if (!completed)
            return;

        int currentWalletNum = /*SecurePlayerPrefs.GetInt(WALLET_NUM_PREF) + 1*/ 1;

        SecurePlayerPrefs.SetInt(WALLET_NUM_PREF, currentWalletNum);
        SecurePlayerPrefs.SetString(PasswordEncryption.PWD_PREF_NAME + "_" + currentWalletNum, walletCreationData.saltedPasswordHash);
        SecurePlayerPrefs.SetString("wallet_" + currentWalletNum + "_h1", walletCreationData.hashLvl1);
        SecurePlayerPrefs.SetString("wallet_" + currentWalletNum + "_h2", walletCreationData.hashLvl2);
        SecurePlayerPrefs.SetString("wallet_" + currentWalletNum + "_h3", walletCreationData.hashLvl3);
        SecurePlayerPrefs.SetString("wallet_" + currentWalletNum + "_h4", walletCreationData.hashLvl4);
        SecurePlayerPrefs.SetString("wallet_" + currentWalletNum, walletCreationData.encryptedPhrase);
        playerPrefPassword.SetupPlayerPrefs(currentWalletNum, onWalletCreated);

        updateManager.RemoveUpdater(this);
        completed = false;
    }

    public void CreateWallet(string mnemonic, Action onWalletCreated)
    {
        this.onWalletCreated = onWalletCreated;
        updateManager.AddUpdater(this);

        using (var pass = protectedStringDataCache.GetData(0).CreateDisposableData())
        {
            CreateWalletCountPref();
            StartWalletEncryption(pass.Value, mnemonic);
        }
    }

    private void StartWalletEncryption(string basePass, string mnemonic)
    {
        TryCreateWallet(mnemonic, () => AsyncTaskScheduler.Schedule(() => EncryptWalletData(mnemonic, basePass)));
    }

    private async Task EncryptWalletData(string mnemonic, string basePass)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.GenerateEncryptionPassword(basePass)).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.firstHalf.SplitHalf();
        var lvl34string = splitPass.secondHalf.SplitHalf();

        walletCreationData.saltedPasswordHash = await Task.Run(() => PasswordEncryption.GetSaltedPasswordHash(basePass)).ConfigureAwait(false);
        walletCreationData.hashLvl1 = await Task.Run(() => lvl12string.firstHalf.GetSHA384Hash()).ConfigureAwait(false);
        walletCreationData.hashLvl2 = await Task.Run(() => lvl12string.secondHalf.GetSHA384Hash()).ConfigureAwait(false);
        walletCreationData.hashLvl3 = await Task.Run(() => lvl34string.firstHalf.GetSHA384Hash()).ConfigureAwait(false);
        walletCreationData.hashLvl4 = await Task.Run(() => lvl34string.secondHalf.GetSHA384Hash()).ConfigureAwait(false);

        string combinedHashes = walletCreationData.hashLvl1 + walletCreationData.hashLvl2 + walletCreationData.hashLvl3 + walletCreationData.hashLvl4;

        walletCreationData.encryptedPhrase = await Task.Run(() => mnemonic.AESEncrypt(combinedHashes).Protect(combinedHashes)).ConfigureAwait(false);

        completed = true;

        UnityEngine.Debug.Log(lvl12string.firstHalf + " " + lvl12string.secondHalf + " " + lvl34string.firstHalf + " " + lvl34string.secondHalf);
        UnityEngine.Debug.Log(walletCreationData.hashLvl1);
        UnityEngine.Debug.Log(walletCreationData.hashLvl2);
        UnityEngine.Debug.Log(walletCreationData.hashLvl3);
        UnityEngine.Debug.Log(walletCreationData.hashLvl4);
        UnityEngine.Debug.Log(walletCreationData.encryptedPhrase);
        UnityEngine.Debug.Log(walletCreationData.saltedPasswordHash);
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
    /// <param name="onWalletCreatedSuccessfully"> Action to call if the wallet was created successfully. </param>
    private void TryCreateWallet(string mnemonic, Action onWalletCreatedSuccessfully)
    {
        try
        {
            var wallet = new Wallet(mnemonic, null, WalletUtils.DetermineCorrectPath(mnemonic));
            onWalletCreatedSuccessfully?.Invoke();
        }
        catch
        {
            ExceptionManager.DisplayException(new Exception("Unable to create wallet with that seed. Please try again."));
        }
    }

    private struct PrefData
    {
        public string saltedPasswordHash;
        public string hashLvl1;
        public string hashLvl2;
        public string hashLvl3;
        public string hashLvl4;
        public string encryptedPhrase;
    }
}
