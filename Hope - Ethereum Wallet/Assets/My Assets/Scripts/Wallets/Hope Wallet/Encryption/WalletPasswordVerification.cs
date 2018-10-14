using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using System;
using UniRx;

/// <summary>
/// Class used for verifying if a wallet password is correct.
/// </summary>
public sealed class WalletPasswordVerification
{
    private Action<byte[]> onPasswordCorrect;
    private Action onPasswordIncorrect;

    private readonly HopeWalletInfoManager hopeWalletInfoManager;
    private readonly DynamicDataCache dynamicDataCache;

    private HopeInputField passwordInputField;

    /// <summary>
    /// Whether the password is currently being verified.
    /// </summary>
    public bool VerifyingPassword { get; private set; }

    /// <summary>
    /// Initializes the WalletPasswordVerification instance with the wallet settings and data cache.
    /// </summary>
    /// <param name="hopeWalletInfoManager"> The active HopeWalletInfoManager. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    public WalletPasswordVerification(HopeWalletInfoManager hopeWalletInfoManager, DynamicDataCache dynamicDataCache)
    {
        this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Verifies the password entered in the password input field.
    /// </summary>
    /// <param name="passwordInputField"> The input field containing the password. </param>
    /// <returns> The current instance of WalletPasswordVerification. </returns>
    public WalletPasswordVerification VerifyPassword(HopeInputField passwordInputField)
    {
        this.passwordInputField = passwordInputField;

        if (passwordInputField?.InputFieldBase != null)
            passwordInputField.InputFieldBase.interactable = false;

        VerifyPassword(passwordInputField.InputFieldBytes);

        return this;
    }

    /// <summary>
    /// Verifies a password.
    /// </summary>
    /// <param name="password"> The password to verify. </param>
    /// <returns> The current instance of WalletPasswordVerification. </returns>
    public WalletPasswordVerification VerifyPassword(byte[] password)
    {
        var saltedHash = hopeWalletInfoManager.GetWalletInfo((int)dynamicDataCache.GetData("walletnum")).EncryptedWalletData.PasswordHash;
        var pbkdf2 = new PBKDF2PasswordHashing(new Blake2b_512_Engine());

        VerifyingPassword = true;

        onPasswordCorrect = null;
        onPasswordIncorrect = null;

        Observable.WhenAll(Observable.Start(() => password == null || password.Length == 0 ? false : pbkdf2.VerifyPassword(password, saltedHash)))
                  .ObserveOnMainThread()
                  .Subscribe(correctPass =>
                  {
                      VerifyingPassword = false;
                      if (!correctPass[0])
                          PasswordIncorrect();
                      else
                          PasswordCorrect(password);
                  });

        return this;
    }

    /// <summary>
    /// Calls an action if the password is correct.
    /// </summary>
    /// <param name="onPasswordCorrect"> The action to call on correct password. </param>
    /// <returns> The current instance of WalletPasswordVerification. </returns>
    public WalletPasswordVerification OnPasswordCorrect(Action<byte[]> onPasswordCorrect)
    {
        this.onPasswordCorrect = onPasswordCorrect;
        return this;
    }

    /// <summary>
    /// Calls an action if the password is incorrect.
    /// </summary>
    /// <param name="onPasswordIncorrect"> The action to call on incorrect password. </param>
    /// <returns> The current instance of WalletPasswordVerification. </returns>
    public WalletPasswordVerification OnPasswordIncorrect(Action onPasswordIncorrect)
    {
        this.onPasswordIncorrect = onPasswordIncorrect;
        return this;
    }

    /// <summary>
    /// Called if the password verified is correct.
    /// </summary>
    /// <param name="password"> The correct password. </param>
    private void PasswordCorrect(byte[] password)
    {
        onPasswordCorrect?.Invoke(password);
    }

    /// <summary>
    /// Called if the password verified is incorrect.
    /// </summary>
    private void PasswordIncorrect()
    {
        onPasswordIncorrect?.Invoke();

        if (passwordInputField?.InputFieldBase != null)
        {
            passwordInputField.InputFieldBase.interactable = true;
            passwordInputField.Error = true;
            passwordInputField.UpdateVisuals();
        }
    }
}