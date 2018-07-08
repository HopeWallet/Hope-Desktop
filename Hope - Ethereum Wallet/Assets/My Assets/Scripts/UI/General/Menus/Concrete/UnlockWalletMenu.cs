using Hope.Security.ProtectedTypes.Types;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which will handle the entering of a password for unlocking a wallet.
/// </summary>
public class UnlockWalletMenu : WalletLoadMenuBase<UnlockWalletMenu>, IEnterButtonObservable
{

    public InputField passwordField;

    public Button unlockButton;
    public Button restoreButton;

    private ButtonClickObserver buttonObserver;
    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// Adds the dependencies required for this menu.
    /// </summary>
    /// <param name="buttonObserver"> The active ButtonObserver. </param>
    /// <param name="dynamicDataCache"> The active ByteDataCache. </param>
    [Inject]
    public void Construct(ButtonClickObserver buttonObserver, DynamicDataCache dynamicDataCache)
    {
        this.buttonObserver = buttonObserver;
        this.dynamicDataCache = dynamicDataCache;
    }

    /// <summary>
    /// Adds the button click events on start.
    /// </summary>
    private void Start()
    {
        unlockButton.onClick.AddListener(LoadWallet);
        restoreButton.onClick.AddListener(RestoreWallet);
    }

    /// <summary>
    /// Subscribes this IEnterButtonObserver.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        buttonObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Unsubscribes this IEnterButtonObserver.
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        buttonObserver.UnsubscribeObservable(this);
    }

    /// <summary>
    /// Loads a wallet with the text input by the user as the password.
    /// Will not close this gui or open the next gui unless the password was correct.
    /// </summary>
    public override void LoadWallet()
    {
        dynamicDataCache.SetData("pass", new ProtectedString(passwordField.text));
        userWalletManager.UnlockWallet();
    }

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

    public override void GoBack()
    {
    }

}
