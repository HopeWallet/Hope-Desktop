using Hope.Security.ProtectedTypes.Types;
using TMPro;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Popup used for entering the password to unlock a specific wallet.
/// </summary>
public sealed class UnlockWalletPopup : ExitablePopupComponent<UnlockWalletPopup>
{
    public Button unlockWalletButton;

    public TMP_InputField passwordField;

    private UIManager uiManager;
    private UserWalletManager userWalletManager;
    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Adds the required dependencies to this popup.
    /// </summary>
    /// <param name="uiManager"> The active UIManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    [Inject]
    public void Construct(UIManager uiManager, UserWalletManager userWalletManager, DynamicDataCache dynamicDataCache)
    {
        this.uiManager = uiManager;
        this.userWalletManager = userWalletManager;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Adds the button listener.
    /// </summary>
    protected override void OnStart() => unlockWalletButton.onClick.AddListener(LoadWallet);

    /// <summary>
    /// Adds the OnWalletLoad method to the UserWallet.OnWalletLoadSuccessful event.
    /// </summary>
    private void OnEnable() => UserWallet.OnWalletLoadSuccessful += OnWalletLoad;

    /// <summary>
    /// Removes the OnWalletLoad method from the UserWallet.OnWalletLoadSuccessful event.
    /// </summary>
    private void OnDisable() => UserWallet.OnWalletLoadSuccessful -= OnWalletLoad;

    /// <summary>
    /// Enables the open wallet gui once the user wallet has been successfully loaded.
    /// </summary>
    private void OnWalletLoad() => uiManager.OpenMenu<OpenWalletMenu>();

    /// <summary>
    /// Attempts to unlock the wallet with the password entered in the field.
    /// </summary>
    private void LoadWallet()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(passwordField.text));
        userWalletManager.UnlockWallet();
    }
}