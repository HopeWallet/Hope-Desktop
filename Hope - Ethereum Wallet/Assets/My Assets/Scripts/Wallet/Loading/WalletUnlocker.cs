using Hope.Security.ProtectedTypes.Types;
using System;
using System.Threading.Tasks;

public sealed class WalletUnlocker : WalletLoaderBase
{
    private readonly WalletDecryptor walletDecryptor;
    private readonly UserWalletInfoManager.Settings walletSettings;

    public WalletUnlocker(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        UserWalletInfoManager.Settings walletSettings,
        UserWalletInfoManager userWalletInfoManager) : base(popupManager, playerPrefPassword, dynamicDataCache, userWalletInfoManager)
    {
        this.walletSettings = walletSettings;
        walletDecryptor = new WalletDecryptor(playerPrefPassword, dynamicDataCache, walletSettings);
    }

    protected override void LoadWallet(string userPass)
    {
        int walletNum = (int)dynamicDataCache.GetData("walletnum");

        if (!SecurePlayerPrefs.HasKey(walletSettings.walletPasswordPrefName + walletNum))
        {
            ExceptionManager.DisplayException(new Exception("No wallet found with that number. Please try a different wallet."));
        }
        else
        {
            string saltedHash = SecurePlayerPrefs.GetString(walletSettings.walletPasswordPrefName + walletNum);
            AsyncTaskScheduler.Schedule(() => TryPassword(userPass, saltedHash));
        }
    }

    protected override void SetupPopup()
    {
        (popupManager.GetPopup<UnlockWalletPopup>().Animator as UnlockWalletPopupAnimator)?.VerifyingPassword();
    }

    private async Task TryPassword(string password, string saltedHash)
    {
        bool correctPassword = string.IsNullOrEmpty(password) ? false : await Task.Run(() => PasswordEncryption.VerifyPassword(password, saltedHash)).ConfigureAwait(false);

        if (!correctPassword)
            IncorrectPassword();
        else
            CorrectPassword(password);
    }

    private void IncorrectPassword()
    {
        var unlockWalletPopup = popupManager.GetPopup<UnlockWalletPopup>();
        unlockWalletPopup.DisableClosing = false;

        MainThreadExecutor.QueueAction(() => (unlockWalletPopup.Animator as UnlockWalletPopupAnimator)?.PasswordIncorrect());
    }

    private void CorrectPassword(string password)
    {
        AssignAddresses(userWalletInfoManager.GetWalletInfo((int)dynamicDataCache.GetData("walletnum")).WalletAddresses);
        dynamicDataCache.SetData("pass", new ProtectedString(password, this));

        MainThreadExecutor.QueueAction(onWalletLoaded);
    }
}