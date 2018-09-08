using Hope.Security.ProtectedTypes.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Popup used for entering the password to unlock a specific wallet.
/// </summary>
public sealed class UnlockWalletPopup : ExitablePopupComponent<UnlockWalletPopup>, IEnterButtonObservable
{
	private Action popupClosed;

    [SerializeField] private Button unlockWalletButton;

	[SerializeField] private HopeInputField passwordField;

	private UnlockWalletPopupAnimator unlockWalletPopupAnimator;
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
    public void Construct(UIManager uiManager, 
						  UserWalletManager userWalletManager, 
						  DynamicDataCache dynamicDataCache, 
						  ButtonClickObserver buttonClickObserver)
    {
        this.uiManager = uiManager;
        this.userWalletManager = userWalletManager;
        this.dynamicDataCache = dynamicDataCache;
        this.buttonClickObserver = buttonClickObserver;
	}

	public void SetFinishingAction(Action popupClosed) => this.popupClosed = popupClosed;

	/// <summary>
	/// Adds the button listener.
	/// </summary>
	protected override void OnStart()
	{
		unlockWalletButton.onClick.AddListener(LoadWallet);
		unlockWalletPopupAnimator = Animator as UnlockWalletPopupAnimator;
	}

    /// <summary>
    /// Adds the OnWalletLoad method to the UserWallet.OnWalletLoadSuccessful event.
    /// </summary>
    private void OnEnable()
    {
        UserWalletManager.OnWalletLoaded += OnWalletLoad;
        buttonClickObserver.SubscribeObservable(this);
    }

    /// <summary>
    /// Removes the OnWalletLoad method from the UserWallet.OnWalletLoadSuccessful event.
    /// </summary>
    private void OnDisable()
    {
        UserWalletManager.OnWalletLoaded -= OnWalletLoad;
        buttonClickObserver.UnsubscribeObservable(this);
		popupClosed?.Invoke();
	}

    /// <summary>
    /// Enables the open wallet gui once the user wallet has been successfully loaded.
    /// </summary>
    private void OnWalletLoad() => uiManager.OpenMenu<OpenWalletMenu>();

    /// <summary>
    /// Attempts to unlock the wallet with the password entered in the field.
    /// </summary>
    private async void LoadWallet()
    {
        string text = passwordField.Text;

        await Task.Run(() =>
        {
            DisableClosing = true;

            if (dynamicDataCache.GetData("pass") != null && dynamicDataCache.GetData("pass") is ProtectedString)
                ((ProtectedString)dynamicDataCache.GetData("pass")).SetValue(text);
            else
                dynamicDataCache.SetData("pass", new ProtectedString(text));

            MainThreadExecutor.QueueAction(() => userWalletManager.UnlockWallet());
        }).ConfigureAwait(false);
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