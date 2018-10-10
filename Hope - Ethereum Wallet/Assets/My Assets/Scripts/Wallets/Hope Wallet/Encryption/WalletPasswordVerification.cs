using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using System;
using UniRx;

/// <summary>
/// Class used for verifying if a wallet password is correct.
/// </summary>
public sealed class WalletPasswordVerification
{
    private readonly HopeWalletInfoManager.Settings walletSettings;
    private readonly DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Whether the password is currently being verified.
    /// </summary>
    public bool VerifyingPassword { get; private set; }

    /// <summary>
    /// Initializes the WalletPasswordVerification instance with the wallet settings and data cache.
    /// </summary>
    /// <param name="walletSettings"> The active HopeWalletInfoManager.Settings. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    public WalletPasswordVerification(HopeWalletInfoManager.Settings walletSettings, DynamicDataCache dynamicDataCache)
    {
        this.walletSettings = walletSettings;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Verifies if a password is the correct password for a wallet.
    /// </summary>
    /// <param name="password"> The password to check. </param>
    /// <param name="walletCorrectAction"> The action to execute if the password is correct. </param>
    /// <param name="walletIncorrectAction"> The action to execute if the password is incorrect. </param>
    public void VerifyPassword(string password, Action walletCorrectAction, Action walletIncorrectAction)
    {
        var saltedHash = SecurePlayerPrefs.GetString(walletSettings.walletPasswordPrefName + (int)dynamicDataCache.GetData("walletnum"));
        var pbkdf2 = new PBKDF2PasswordHashing(new Blake2b_512_Engine());

        VerifyingPassword = true;

        Observable.WhenAll(Observable.Start(() => string.IsNullOrEmpty(password) ? false : pbkdf2.VerifyPassword(password, saltedHash)))
                  .ObserveOnMainThread()
                  .Subscribe(correctPass =>
                  {
                      VerifyingPassword = false;
                      if (!correctPass[0])
                          walletIncorrectAction?.Invoke();
                      else
                          walletCorrectAction?.Invoke();
                  });
    }
}