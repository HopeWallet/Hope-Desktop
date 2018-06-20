﻿using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which will handle the entering of a password for unlocking a wallet.
/// </summary>
public class UnlockWalletMenu : WalletLoaderBase<UnlockWalletMenu>, IEnterButtonObserver
{

    public InputField passwordField;

    public Button unlockButton;
    public Button restoreButton;

    private ButtonObserverManager buttonObserver;

    /// <summary>
    /// Adds the dependencies required for this menu.
    /// </summary>
    /// <param name="buttonObserver"> The active ButtonObserver. </param>
    [Inject]
    public void Construct(ButtonObserverManager buttonObserver) => this.buttonObserver = buttonObserver;

    /// <summary>
    /// Adds the button click events on start.
    /// </summary>
    private void Start()
    {
        unlockButton.onClick.AddListener(LoadWallet);
        restoreButton.onClick.AddListener(RestoreWallet);
    }

    /// <summary>
    /// Adds this IEnterButtonObserver.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        buttonObserver.AddEnterButtonObserver(this);
    }

    /// <summary>
    /// Removes this IEnterButtonObserver.
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        buttonObserver.RemoveEnterButtonObserver(this);
    }

    /// <summary>
    /// Loads a wallet with the text input by the user as the password.
    /// Will not close this gui or open the next gui unless the password was correct.
    /// </summary>
    public override void LoadWallet() => userWalletManager.UnlockWallet(passwordField.text);

    /// <summary>
    /// Enables the menu for creating a new wallet from the beginning.
    /// </summary>
    public void RestoreWallet() => uiManager.OpenMenu<CreatePasswordMenu>();

    /// <summary>
    /// Loads the wallet when the enter button is pressed.
    /// </summary>
    /// <param name="clickType"> The enter button click type. </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        if (InputFieldUtils.GetActiveInputField() == passwordField && clickType == ClickType.Down)
            unlockButton.Press();

    }

    public override void OnBackPressed()
    {
    }

}
