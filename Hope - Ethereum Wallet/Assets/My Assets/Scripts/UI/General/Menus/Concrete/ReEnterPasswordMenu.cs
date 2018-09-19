using Hope.Security.PBKDF2;
using Hope.Security.PBKDF2.Engines.Blake2b;
using Hope.Security.ProtectedTypes.Types;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class ReEnterPasswordMenu : Menu<ReEnterPasswordMenu>, ITabButtonObservable, IEnterButtonObservable
{
    public event Action OnPasswordVerificationStarted;
    public event Action OnPasswordEnteredCorrect;
    public event Action OnPasswordEnteredIncorrect;

    [SerializeField] private TextMeshProUGUI walletName;

    [SerializeField] private Button unlockButton, homeButton;
    [SerializeField] private HopeInputField passwordField;

    private HopeWalletInfoManager.Settings walletSettings;
    private DynamicDataCache dynamicDataCache;
    private ButtonClickObserver buttonClickObserver;

    private bool checkingPassword;
    private int walletNum;

    [Inject]
    public void Construct(
        HopeWalletInfoManager hopeWalletInfoManager,
        UserWalletManager userWalletManager,
        HopeWalletInfoManager.Settings walletSettings,
        DynamicDataCache dynamicDataCache,
        ButtonClickObserver buttonClickObserver)
    {
        this.walletSettings = walletSettings;
        this.dynamicDataCache = dynamicDataCache;
        this.buttonClickObserver = buttonClickObserver;

        var walletInfo = hopeWalletInfoManager.GetWalletInfo(userWalletManager.GetWalletAddress());
        walletName.text = walletInfo.WalletName;
        walletNum = walletInfo.WalletNum + 1;

        (dynamicDataCache.GetData("pass") as ProtectedString)?.Dispose();
        dynamicDataCache.SetData("pass", null);
    }

    protected override void OnAwake()
    {
        homeButton.onClick.AddListener(HomeButtonClicked);
        unlockButton.onClick.AddListener(UnlockButtonClicked);

        passwordField.OnInputUpdated += PasswordFieldChanged;
    }

    private void OnEnable()
    {
        buttonClickObserver.SubscribeObservable(this);
    }

    private void OnDisable()
    {
        buttonClickObserver.UnsubscribeObservable(this);
    }

    private void PasswordFieldChanged(string text)
    {
        passwordField.Error = string.IsNullOrEmpty(text);
        unlockButton.interactable = !passwordField.Error;
    }

    private void HomeButtonClicked()
    {
        SceneManager.LoadScene("Hope Wallet");
    }

    private void UnlockButtonClicked()
    {
        OnPasswordVerificationStarted?.Invoke();

        var saltedHash = SecurePlayerPrefs.GetString(walletSettings.walletPasswordPrefName + walletNum);
        var pbkdf2 = new PBKDF2PasswordHashing(new Blake2b_512_Engine());

        checkingPassword = true;

        Observable.WhenAll(Observable.Start(() => string.IsNullOrEmpty(passwordField.Text) ? false : pbkdf2.VerifyPassword(passwordField.Text, saltedHash)))
                  .ObserveOnMainThread()
                  .Subscribe(correctPass =>
                  {
                      if (!correctPass[0])
                          IncorrectPassword();
                      else
                          CorrectPassword(passwordField.Text);

                      checkingPassword = false;
                      passwordField.InputFieldBase.interactable = true;
                  });
    }

    private void CorrectPassword(string password)
    {
        dynamicDataCache.SetData("pass", new ProtectedString(password));

        OnPasswordEnteredCorrect?.Invoke();
        uiManager.CloseMenu();
    }

    private void IncorrectPassword()
    {
        OnPasswordEnteredIncorrect?.Invoke();
    }

    public void TabButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        if (!checkingPassword)
            passwordField.InputFieldBase.SelectSelectable();
    }

    public void EnterButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        if (unlockButton.interactable && !checkingPassword)
        {
            unlockButton.Press();
            passwordField.InputFieldBase.interactable = false;
        }
        else if (!checkingPassword)
        {
            passwordField.InputFieldBase.SelectSelectable();
        }
    }
}