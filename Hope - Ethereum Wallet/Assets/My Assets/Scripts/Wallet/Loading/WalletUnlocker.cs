using Hope.Security.ProtectedTypes.Types;
using Nethereum.HdWallet;
using System;
using System.Threading.Tasks;

public sealed class WalletUnlocker : WalletLoaderBase
{
    private readonly WalletDecryptor walletDecryptor;

    public WalletUnlocker(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache protectedStringDataCache) : base(popupManager, playerPrefPassword, protectedStringDataCache)
    {
        walletDecryptor = new WalletDecryptor(playerPrefPassword, dynamicDataCache);
    }

    protected override void LoadWallet(string userPass)
    {
        int walletNum = (int)dynamicDataCache.GetData("walletnum");

        if (!SecurePlayerPrefs.HasKey("password_" + walletNum))
        {
            ExceptionManager.DisplayException(new Exception("No wallet found with that number. Please try a different wallet."));
        }

        else
        {
            string saltedHash = SecurePlayerPrefs.GetString("password_" + walletNum);
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
        walletDecryptor.DecryptWallet(password, async (seed, derivation) =>
        {
            await GetAddresses(new Wallet(seed, derivation)).ConfigureAwait(false);
            await Task.Run(() => dynamicDataCache.SetData("pass", new ProtectedString(password, this))).ConfigureAwait(false);

            MainThreadExecutor.QueueAction(onWalletLoaded);

            seed.ClearBytes();
        });
    }
}