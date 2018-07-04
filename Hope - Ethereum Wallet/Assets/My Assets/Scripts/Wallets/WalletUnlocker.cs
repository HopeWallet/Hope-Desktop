using Hope.Security.HashGeneration;
using Hope.Security.ProtectedTypes.Types;
using Nethereum.HdWallet;
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

    public WalletUnlocker(PopupManager popupManager, PlayerPrefPassword playerPrefPassword, ProtectedStringDataCache protectedStringDataCache)
    {
        this.popupManager = popupManager;
        this.playerPrefPassword = playerPrefPassword;
        this.protectedStringDataCache = protectedStringDataCache;
    }

    public void UnlockWallet(int walletNum, Action onWalletLoaded, Action<ProtectedString[]> onWalletUnlocked)
    {
        StartUnlockPopup();
        using (var pass = protectedStringDataCache.GetData(0).CreateDisposableData())
        {
            AsyncTaskScheduler.Schedule(() => TryPassword(walletNum, pass.Value));
        }
    }

    private async Task TryPassword(int walletNum, string password)
    {
        bool correctPassword = await Task.Run(() => PasswordEncryption.VerifyPassword(password, SecurePlayerPrefs.GetString(PasswordEncryption.PWD_PREF_NAME + "_" + walletNum))).ConfigureAwait(false);

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
        playerPrefPassword.PopulatePrefDictionary(walletNum);

        string[] hashLvls = new string[4];

        for (int i = 0; i < hashLvls.Length; i++)
            hashLvls[i] = SecurePlayerPrefs.GetString("wallet_" + walletNum + "_h" + (i + 1));

        AsyncTaskScheduler.Schedule(() => UnlockWalletAsync(walletNum, password, wallet => AsyncTaskScheduler.Schedule(() => GetAddresses(wallet))));
    }

    private async Task UnlockWalletAsync(int walletNum, string password, Action<Wallet> onWalletUnlocked)
    {
        var encryptionPassword = await Task.Run(() => playerPrefPassword.ExtractEncryptionPassword(password).GetSHA256Hash()).ConfigureAwait(false);
        var splitPass = encryptionPassword.SplitHalf();
        var lvl12string = splitPass.firstHalf.SplitHalf();
        var lvl34string = splitPass.secondHalf.SplitHalf();


    }

    private async Task GetAddresses(Wallet wallet)
    {

    }

    private void StartUnlockPopup()
    {
        popupManager.GetPopup<LoadingPopup>().SetLoadingText(" password", "Checking");
    }

}