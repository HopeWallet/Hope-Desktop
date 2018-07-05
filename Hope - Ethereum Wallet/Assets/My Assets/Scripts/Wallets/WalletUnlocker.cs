using Hope.Security.Encryption;
using Hope.Security.Encryption.DPAPI;
using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Hope.Utils.EthereumUtils;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WalletUnlocker
{

    private readonly PopupManager popupManager;
    private readonly PlayerPrefPassword playerPrefPassword;
    private readonly ProtectedStringDataCache protectedStringDataCache;

    private ProtectedString[] addresses;

    private Action walletUnlocked;

    public WalletUnlocker(PopupManager popupManager, PlayerPrefPassword playerPrefPassword, ProtectedStringDataCache protectedStringDataCache)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.protectedStringDataCache = protectedStringDataCache;
    }

    public void UnlockWallet(int walletNum, Action onWalletLoaded, Action<ProtectedString[]> onWalletUnlocked)
    {
        SetupActions(onWalletLoaded, onWalletUnlocked);
        StartUnlockPopup();
        using (var pass = protectedStringDataCache.GetData(0).CreateDisposableData())
        {
            pass.Value.Log();
            AsyncTaskScheduler.Schedule(() => TryPassword(walletNum, pass.Value, SecurePlayerPrefs.GetString(PasswordEncryption.PWD_PREF_NAME + "_" + walletNum)));
        }
    }

    private void SetupActions(Action onWalletLoaded, Action<ProtectedString[]> onWalletUnlocked)
    {
        walletUnlocked = () =>
        {
            popupManager.CloseActivePopup();
            onWalletUnlocked?.Invoke(addresses);
            onWalletLoaded?.Invoke();
        };
    }

    private async Task TryPassword(int walletNum, string password, string saltedHash)
    {
        bool correctPassword = await Task.Run(() => PasswordEncryption.VerifyPassword(password, saltedHash)).ConfigureAwait(false);

        if (!correctPassword)
            IncorrectPassword();
        else
            CorrectPassword(walletNum, password);
    }

    private void IncorrectPassword()
    {
        MainThreadExecutor.QueueAction(() => ExceptionManager.DisplayException(new Exception("Unable to unlock wallet, incorrect password. ")));
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
        });

    }

    private async Task UnlockWalletAsync(string[] hashLvls, string encryptedSeed, string password, Action<Wallet> onWalletUnlocked)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.ExtractEncryptionPassword(password).GetSHA256Hash()).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.firstHalf.SplitHalf();
        var lvl34string = splitPass.secondHalf.SplitHalf();

        string unprotectedSeed = await Task.Run(() => encryptedSeed.Unprotect(hashLvls[2].Unprotect() + hashLvls[3].AESDecrypt(lvl34string.secondHalf.GetSHA512Hash()))).ConfigureAwait(false);
        byte[] decryptedSeed = await Task.Run(() => unprotectedSeed.AESDecrypt(hashLvls[0].AESDecrypt(lvl12string.firstHalf.GetSHA512Hash()) + hashLvls[1].Unprotect()).HexToByteArray()).ConfigureAwait(false);

        onWalletUnlocked?.Invoke(new Wallet(decryptedSeed));
    }

    private async Task GetAddresses(Wallet wallet)
    {
        addresses = await Task.Run(() => wallet.GetAddresses(50).Select(str => new ProtectedString(str)).ToArray()).ConfigureAwait(false);
        addresses[0].CreateDisposableData().Value.Log();
        MainThreadExecutor.QueueAction(walletUnlocked);
    }

    private void StartUnlockPopup()
    {
        popupManager.GetPopup<LoadingPopup>().loadingText.text = "Unlocking wallet...";
    }

}