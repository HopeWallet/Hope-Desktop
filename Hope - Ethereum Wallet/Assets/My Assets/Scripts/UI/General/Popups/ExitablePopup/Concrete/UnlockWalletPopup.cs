using Hope.Security.ProtectedTypes.Types;
using TMPro;
using UnityEngine.UI;
using Zenject;

public sealed class UnlockWalletPopup : ExitablePopupComponent<UnlockWalletPopup>
{

    public Button unlockWalletButton;

    public TMP_InputField passwordField;

    private UIManager uiManager;
    private UserWalletManager userWalletManager;
    private DynamicDataCache dynamicDataCache;

    [Inject]
    public void Construct(UIManager uiManager, UserWalletManager userWalletManager, DynamicDataCache dynamicDataCache)
    {
        this.uiManager = uiManager;
        this.userWalletManager = userWalletManager;
        this.dynamicDataCache = dynamicDataCache;
    }

    protected override void OnStart()
    {
        unlockWalletButton.onClick.AddListener(LoadWallet);
    }

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


    private void LoadWallet()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(passwordField.text));
        userWalletManager.UnlockWallet();
    }
}