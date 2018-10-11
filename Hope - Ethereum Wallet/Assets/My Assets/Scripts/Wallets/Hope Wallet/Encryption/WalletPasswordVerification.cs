using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using System;
using UniRx;
using UnityEngine.UI;

/// <summary>
/// Class used for verifying if a wallet password is correct.
/// </summary>
public sealed class WalletPasswordVerification
{
    private Action<string> onPasswordCorrect;
    private Action onPasswordIncorrect;

    private readonly HopeWalletInfoManager.Settings walletSettings;
    private readonly DynamicDataCache dynamicDataCache;

    private HopeInputField passwordInputField;
    private Button viewPasswordButton;

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

    public WalletPasswordVerification VerifyPassword(HopeInputField passwordInputField, Button viewPasswordButton)
    {
        this.passwordInputField = passwordInputField;
        this.viewPasswordButton = viewPasswordButton;

        if (passwordInputField?.InputFieldBase != null)
            passwordInputField.InputFieldBase.interactable = false;

        if (viewPasswordButton != null)
            viewPasswordButton.interactable = false;

        VerifyPassword(passwordInputField.Text);

        return this;
    }

    public WalletPasswordVerification VerifyPassword(string password)
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
                          PasswordIncorrect();
                      else
                          PasswordCorrect(password);
                  });
        return this;
    }

    public WalletPasswordVerification OnPasswordCorrect(Action<string> onPasswordCorrect)
    {
        this.onPasswordCorrect = onPasswordCorrect;
        return this;
    }

    public WalletPasswordVerification OnPasswordIncorrect(Action onPasswordIncorrect)
    {
        this.onPasswordIncorrect = onPasswordIncorrect;
        return this;
    }

    private void PasswordCorrect(string password)
    {
        onPasswordCorrect?.Invoke(password);
    }

    private void PasswordIncorrect()
    {
        onPasswordIncorrect?.Invoke();

        if (passwordInputField?.InputFieldBase != null)
        {
            passwordInputField.InputFieldBase.interactable = true;
            passwordInputField.Error = true;
            passwordInputField.UpdateVisuals();
        }

        if (viewPasswordButton != null)
            viewPasswordButton.interactable = true;
    }
}