using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using Hope.Security.ProtectedTypes.Types;
using System;
using UniRx;

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
        PlayerPrefPasswordDerivation playerPrefPassword,
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
            var saltedHash = SecurePlayerPrefs.GetString(walletSettings.walletPasswordPrefName + walletNum);

            Observable.WhenAll(Observable.Start(() => string.IsNullOrEmpty(userPass) ? false : new PBKDF2PasswordHashing(new Blake2b_512_Engine()).VerifyPassword(userPass, saltedHash)))
                      .ObserveOnMainThread()
                      .Subscribe(correctPass =>
                      {
                          if (!correctPass[0])
                              IncorrectPassword();
                          else
                              CorrectPassword(userPass);
                      });
        }
    }

    /// <summary>
    /// Called if the password entered was incorrect.
    /// </summary>
    private void IncorrectPassword()
    {
        var unlockWalletPopup = popupManager.GetPopup<UnlockWalletPopup>();
        unlockWalletPopup.DisableClosing = false;

        (unlockWalletPopup.Animator as UnlockWalletPopupAnimator)?.PasswordIncorrect();
    }

    /// <summary>
    /// Loads the wallet with the correct password.
    /// </summary>
    /// <param name="password"> The entered password. </param>
    private void CorrectPassword(string password)
    {
        var walletInfo = userWalletInfoManager.GetWalletInfo((int)dynamicDataCache.GetData("walletnum"));
        AssignAddresses(walletInfo.WalletAddresses[0], walletInfo.WalletAddresses[1]);

        dynamicDataCache.SetData("pass", new ProtectedString(password, this));

        onWalletLoaded?.Invoke();
    }
}