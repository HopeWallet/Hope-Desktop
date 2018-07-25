using Hope.Security.ProtectedTypes.Types;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Popup used for entering the password to unlock a specific wallet.
/// </summary>
public sealed class UnlockWalletPopup : ExitablePopupComponent<UnlockWalletPopup>, IEnterButtonObservable
{
    public Button unlockWalletButton;

    public TMP_InputField passwordField;
	public InfoMessage errorMessage;

    private UIManager uiManager;
    private UserWalletManager userWalletManager;
    private DynamicDataCache dynamicDataCache;
    private ButtonClickObserver buttonClickObserver;

    /// <summary>
    /// Adds the required dependencies to this popup.
    /// </summary>
    /// <param name="uiManager"> The active UIManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="buttonClickObserver"> The active ButtonClickObserver. </param>
    [Inject]
    public void Construct(UIManager uiManager, UserWalletManager userWalletManager, DynamicDataCache dynamicDataCache, ButtonClickObserver buttonClickObserver)
    {
        this.uiManager = uiManager;
        this.userWalletManager = userWalletManager;
        this.dynamicDataCache = dynamicDataCache;
        this.buttonClickObserver = buttonClickObserver;
    }

    /// <summary>
    /// Adds the button listener.
    /// </summary>
    protected override void OnStart()
    {
		errorMessage.PopupManager = popupManager;
        unlockWalletButton.onClick.AddListener(LoadWallet);
    }

    /// <summary>
    /// Adds the OnWalletLoad method to the UserWallet.OnWalletLoadSuccessful event.
    /// </summary>
    private void OnEnable()
    {
        UserWallet.OnWalletLoadSuccessful += OnWalletLoad;
        buttonClickObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Removes the OnWalletLoad method from the UserWallet.OnWalletLoadSuccessful event.
    /// </summary>
    private void OnDisable()
    {
        UserWallet.OnWalletLoadSuccessful -= OnWalletLoad;
        buttonClickObserver.UnsubscribeObservable(this);
    }

    /// <summary>
    /// Enables the open wallet gui once the user wallet has been successfully loaded.
    /// </summary>
    private void OnWalletLoad()
    {
        uiManager.OpenMenu<OpenWalletMenu>();
    }

    /// <summary>
    /// Attempts to unlock the wallet with the password entered in the field.
    /// </summary>
    private void LoadWallet()
    {
        DisableClosing = true;
        dynamicDataCache.SetData("pass", new ProtectedString(passwordField.text));
        userWalletManager.UnlockWallet();
    }

    /// <summary>
    /// Attempts to open the wallet when enter is pressed.
    /// </summary>
    /// <param name="clickType"> The enter button click type. </param>
    public void EnterButtonPressed(ClickType clickType)
    {
        if (clickType == ClickType.Down && unlockWalletButton.interactable && !DisableClosing)
            unlockWalletButton.Press();
    }
}