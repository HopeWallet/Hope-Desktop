using Hope.Security.ProtectedTypes.Types;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu which lets the user create a new wallet by first choosing a password and name for the wallet.
/// </summary>
public sealed class CreateWalletMenu : Menu<CreateWalletMenu>, IEnterButtonObservable, ITabButtonObservable
{
    [SerializeField] private Button nextButton;

    [SerializeField] private HopeInputField walletNameField,
                                            password1Field,
                                            password2Field;

    private DynamicDataCache dynamicDataCache;
    private HopeWalletInfoManager hopeWalletInfoManager;
    private ButtonClickObserver buttonClickObserver;

    private List<Selectable> inputFields = new List<Selectable>();

    /// <summary>
    /// Adds the required dependencies into this class.
    /// </summary>
    /// <param name="dynamicDataCache"> The active ProtectedStringDataCache. </param>
    /// <param name="hopeWalletInfoManager"> The active UserWalletInfoManager. </param>
    /// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
    [Inject]
    public void Construct(DynamicDataCache dynamicDataCache,
                          HopeWalletInfoManager hopeWalletInfoManager,
                          ButtonClickObserver buttonClickObserver)
    {
        this.dynamicDataCache = dynamicDataCache;
        this.hopeWalletInfoManager = hopeWalletInfoManager;
        this.buttonClickObserver = buttonClickObserver;
    }

    /// <summary>
    /// Subscribes the the current buttonClickObserver
    /// </summary>
    private void OnEnable() => buttonClickObserver.SubscribeObservable(this);

    /// <summary>
    /// Unsubscribes the the current buttonClickObserver and resets the text.
    /// </summary>
    private void OnDisable()
    {
        buttonClickObserver.UnsubscribeObservable(this);
        walletNameField.Text = string.Empty;
        password1Field.Text = string.Empty;
        password2Field.Text = string.Empty;
    }

    /// <summary>
    /// Adds the button listeners.
    /// </summary>
    protected override void OnAwake()
    {
        inputFields.Add(walletNameField.InputFieldBase);
        inputFields.Add(password1Field.InputFieldBase);
        inputFields.Add(password2Field.InputFieldBase);

        password1Field.OnInputUpdated += _ => PasswordsUpdated();
        password2Field.OnInputUpdated += _ => PasswordsUpdated();
        walletNameField.OnInputUpdated += WalletNameFieldChanged;
        nextButton.onClick.AddListener(CreateWalletNameAndPass);
    }

    /// <summary>
    /// Sets up the wallet name and password and opens the next menu.
    /// </summary>
    private void CreateWalletNameAndPass()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(password1Field.Text));
        dynamicDataCache.SetData("name", walletNameField.Text);

        uiManager.OpenMenu<ImportOrCreateMnemonicMenu>();
    }

    /// <summary>
    /// Checks to see if the wallet name is valid and has not been used before.
    /// </summary>
    /// <param name="walletName"> The text in the wallet name input field. </param>
    private void WalletNameFieldChanged(string walletName)
    {
        bool emptyName = string.IsNullOrEmpty(walletName.Trim());
        bool usedName = WalletNameExists(walletName);
        walletNameField.Error = emptyName || usedName;

        if (emptyName)
            walletNameField.errorMessage.text = "Invalid wallet name";
        else if (usedName)
            walletNameField.errorMessage.text = "Wallet name in use";

        SetButtonInteractable();
    }

    /// <summary>
    /// Loops through the saved wallets and checks if a wallet is already saved under the given name.
    /// </summary>
    /// <param name="walletName"> The current wallet name in the input field. </param>
    /// <returns> Whether the given walletName has been used before </returns>
    private bool WalletNameExists(string walletName)
    {
        for (int i = 1; ; i++)
        {
            if (hopeWalletInfoManager.GetWalletInfo(i).WalletName.EqualsIgnoreCase(walletName))
                return true;
            else if (string.IsNullOrEmpty(hopeWalletInfoManager.GetWalletInfo(i).WalletName))
                return false;
        }
    }

    /// <summary>
    /// Checks if the passwords valid and animates the error icon if needed
    /// </summary>
    private void PasswordsUpdated()
    {
        string password1Text = password1Field.Text;
        string password2Text = password2Field.Text;

        password1Field.Error = password1Field.Text.Length < 8;
        password2Field.Error = password1Text != password2Text;

        if (password1Field.Error)
            password1Field.errorMessage.text = "Password too short";

        if (password2Field.Error)
            password2Field.errorMessage.text = "Passwords do not match";

        password2Field.UpdateVisuals();
		SetButtonInteractable();
    }

    /// <summary>
    /// Checks if passwords match, are above 7 characters, and all fields are filled in
    /// </summary>
    private void SetButtonInteractable() => nextButton.interactable = !walletNameField.Error && !password1Field.Error && !password2Field.Error;
	
	/// <summary>
	/// Moves to the next input field
	/// </summary>
	/// <param name="clickType"> The tab button ClickType </param>
	public void EnterButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        if (InputFieldUtils.GetActiveInputField() == password2Field.InputFieldBase && nextButton.interactable)
            nextButton.Press();
        else
            SelectableExtensions.MoveToNextSelectable(inputFields);
    }

	/// <summary>
	/// Moves to next input field, unless at the last input field, then it presses the button if it is interactable
	/// </summary>
	/// <param name="clickType"> The enter button ClickType </param>
	public void TabButtonPressed(ClickType clickType)
    {
        if (clickType != ClickType.Down)
            return;

        SelectableExtensions.MoveToNextSelectable(inputFields);
    }
}