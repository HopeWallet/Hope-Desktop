using Hope.Security.ProtectedTypes.Types;
using System;
using System.Threading.Tasks;

/// <summary>
/// Class used for unlocking the hope wallet.
/// </summary>
public sealed class WalletUnlocker : WalletLoaderBase
{
    private readonly WalletDecryptor walletDecryptor;
    private readonly HopeWalletInfoManager.Settings walletSettings;

    /// <summary>
    /// Initializes the WalletUnlocker with all required references.
    /// </summary>
    /// <param name="popupManager"> The active PopupManager. </param>
    /// <param name="playerPrefPassword"> The PlayerPrefPassword used to encrypt the wallet. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="walletSettings"> The settings for the UserWallet. </param>
    /// <param name="userWalletInfoManager"> The active UserWalletInfoManager. </param>
    public WalletUnlocker(
        PopupManager popupManager,
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        HopeWalletInfoManager.Settings walletSettings,
        HopeWalletInfoManager userWalletInfoManager) : base(popupManager, playerPrefPassword, dynamicDataCache, userWalletInfoManager)
    {
        this.walletSettings = walletSettings;
        walletDecryptor = new WalletDecryptor(playerPrefPassword, dynamicDataCache, walletSettings);
    }

    /// <summary>
    /// Attempts to load the wallet given the password.
    /// </summary>
    /// <param name="userPass"> The password to attempt to unlock the wallet with. </param>
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

    /// <summary>
    /// Tests the password entered against the pbkdf2 salted hash.
    /// </summary>
    /// <param name="password"> The password entered by the user. </param>
    /// <param name="saltedHash"> The pbkdf2 salted password hash. </param>
    /// <returns> The task used to check the password. </returns>
    private async Task TryPassword(string password, string saltedHash)
    {
        bool correctPassword = string.IsNullOrEmpty(password) ? false : await Task.Run(() => PasswordEncryption.Blake2.VerifyPassword(password, saltedHash)).ConfigureAwait(false);

        if (!correctPassword)
            IncorrectPassword();
        else
            CorrectPassword(password);
    }

    /// <summary>
    /// Called if the password entered was incorrect.
    /// </summary>
    private void IncorrectPassword()
    {
        var unlockWalletPopup = popupManager.GetPopup<UnlockWalletPopup>();
        unlockWalletPopup.DisableClosing = false;

        MainThreadExecutor.QueueAction(() => (unlockWalletPopup.Animator as UnlockWalletPopupAnimator)?.PasswordIncorrect());
    }

    /// <summary>
    /// Loads the wallet with the correct password.
    /// </summary>
    /// <param name="password"> The entered password. </param>
    private void CorrectPassword(string password)
    {
        AssignAddresses(userWalletInfoManager.GetWalletInfo((int)dynamicDataCache.GetData("walletnum")).WalletAddresses);
        dynamicDataCache.SetData("pass", new ProtectedString(password, this));

        MainThreadExecutor.QueueAction(onWalletLoaded);
    }
}