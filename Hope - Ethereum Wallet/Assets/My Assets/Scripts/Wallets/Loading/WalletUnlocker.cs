using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

public class WalletUnlocker : WalletLoaderBase
{

    protected override string LoadingText => "Unlocking wallet...";

    public WalletUnlocker(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        ProtectedStringDataCache protectedStringDataCache) : base(popupManager, playerPrefPassword, protectedStringDataCache)
    {
    }

    protected override void LoadWallet(object data, string userPass)
    {
        int walletNum = (int)data;

        if (!SecurePlayerPrefs.HasKey(PasswordEncryption.PWD_PREF_NAME + "_" + walletNum))
        {
            ExceptionManager.DisplayException(new Exception("No wallet found with that number. Please try a different wallet."));
        }

        else
        {
            string saltedHash = SecurePlayerPrefs.GetString(PasswordEncryption.PWD_PREF_NAME + "_" + walletNum);
            AsyncTaskScheduler.Schedule(() => TryPassword(walletNum, userPass, saltedHash));
        }
    }

    private async Task TryPassword(int walletNum, string password, string saltedHash)
    {
        bool correctPassword = string.IsNullOrEmpty(password) ? false : await Task.Run(() => PasswordEncryption.VerifyPassword(password, saltedHash)).ConfigureAwait(false);

        if (!correctPassword)
            IncorrectPassword();
        else
            CorrectPassword(walletNum, password);
    }

    private void IncorrectPassword()
    {
        MainThreadExecutor.QueueAction(() => ExceptionManager.DisplayException(new Exception("Unable to unlock wallet, incorrect password. ")), "incorrect password");
    }

    private void CorrectPassword(int walletNum, string password)
    {
        MainThreadExecutor.QueueAction(() =>
        {
            playerPrefPassword.PopulatePrefDictionary(walletNum);

            string[] hashLvls = new string[4];
            for (int i = 0; i < hashLvls.Length; i++)
                hashLvls[i] = SecurePlayerPrefs.GetString("wallet_" + walletNum + "_h" + (i + 1));

            AsyncTaskScheduler.Schedule(() => UnlockWalletAsync(hashLvls, SecurePlayerPrefs.GetString("wallet_" + walletNum), password, wallet => AsyncTaskScheduler.Schedule(() => GetAddresses(wallet))));
        }, "correct password");

    }

    private async Task UnlockWalletAsync(string[] hashLvls, string encryptedSeed, string password, Action<Wallet> onWalletUnlocked)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.ExtractEncryptionPassword(password).GetSHA256Hash()).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.Item1.SplitHalf();
        var lvl34string = splitPass.Item2.SplitHalf();

        string unprotectedSeed = await Task.Run(() => encryptedSeed.Unprotect(hashLvls[2].Unprotect() + hashLvls[3].AESDecrypt(lvl34string.Item2.GetSHA512Hash()))).ConfigureAwait(false);
        byte[] decryptedSeed = await Task.Run(() => unprotectedSeed.AESDecrypt(hashLvls[0].AESDecrypt(lvl12string.Item1.GetSHA512Hash()) + hashLvls[1].Unprotect()).HexToByteArray()).ConfigureAwait(false);

        onWalletUnlocked?.Invoke(new Wallet(decryptedSeed));
    }

    protected override void OnAddressesReceived()
    {
        addresses[0].CreateDisposableData().Value.Log();
        MainThreadExecutor.QueueAction(onWalletLoaded, "wallet unlocked action");
    }
}