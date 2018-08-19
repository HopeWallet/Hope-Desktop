using Hope.Security.ProtectedTypes.Types;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu for importing an ethereum wallet.
/// </summary>
public sealed class ImportMnemonicMenu : WalletLoadMenuBase<ImportMnemonicMenu>, IEnterButtonObservable
{
	[SerializeField] private Button importButton;

	[SerializeField] private TMP_InputField[] wordFields;

    private ButtonClickObserver buttonObserver;
    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Injects the required dependencies into this menu.
    /// </summary>
    /// <param name="buttonObserver"> The active ButtonObserver. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache </param>
    [Inject]
    public void Construct(ButtonClickObserver buttonObserver, DynamicDataCache dynamicDataCache)
    {
        this.buttonObserver = buttonObserver;
        this.dynamicDataCache = dynamicDataCache;
    }

	/// <summary>
	/// Adds the button click events.
	/// </summary>
	private void Start() => importButton.onClick.AddListener(LoadWallet);

	/// <summary>
	/// Subscribes this IEnterButtonObserver.
	/// </summary>
	protected override void OnEnable()
    {
        base.OnEnable();
        buttonObserver.SubscribeObservable(this);
    }

	/// <summary>
	/// Opens the exit confirmation popup and enables the note text
	/// </summary>
	protected override void OpenExitConfirmationPopup() => popupManager.GetPopup<ExitConfirmationPopup>(true)?.SetDetails(true);

	/// <summary>
	/// Unsubscribes this IEnterButtonObserver.
	/// </summary>
	protected override void OnDisable()
    {
        base.OnDisable();
        buttonObserver.UnsubscribeObservable(this);
    }

    /// <summary>
    /// Loads the wallet if the button is enabled.
    /// </summary>
    /// <param name="clickType"> The enter button click type. </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        //if (wordFields.Contains(InputFieldUtils.GetActiveInputField()) && importButton.interactable && clickType == ClickType.Down)
            //importButton.Press();
    }

    /// <summary>
    /// Sends a message to the UserWalletManager to create a wallet with the InputField text as the mnemonic phrase.
    /// </summary>
    [SecureCallEnd]
    public override void LoadWallet()
    {
        if (CheckCreatedMnemonic())
            userWalletManager.CreateWallet();
    }

    /// <summary>
    /// Checks if data already exists in the DynamicDataCache.
    /// If the data is equal to the mnemonic entered in the input fields, display the ConfirmMnemonicMenu.
    /// </summary>
    /// <returns> True if the mnemonic was unique and the wallet can be directly imported. </returns>
    [SecureCaller]
    [ReflectionProtect(typeof(bool))]
    private bool CheckCreatedMnemonic()
    {
        var mnemonicData = dynamicDataCache.GetData("mnemonic");
        var newMnemonic = string.Join(" ", wordFields.Select(field => field.text)).Trim();

        if (mnemonicData == null)
        {
            dynamicDataCache.SetData("mnemonic", new ProtectedString(newMnemonic));
            return true;
        }

        using (var mnemonic = (mnemonicData as ProtectedString)?.CreateDisposableData())
        {
            if (mnemonic.Value.EqualsIgnoreCase(newMnemonic, true))
            {
                uiManager.OpenMenu<ConfirmMnemonicMenu>();
                return false;
            }
            else
            {
                dynamicDataCache.SetData("mnemonic", new ProtectedString(newMnemonic));
                return true;
            }
        }
    }

}
