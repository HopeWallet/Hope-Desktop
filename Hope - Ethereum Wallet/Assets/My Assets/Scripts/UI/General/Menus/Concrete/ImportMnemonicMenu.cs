using Hope.Utils.Ethereum;
using Nethereum.HdWallet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Menu for importing an ethereum wallet.
/// </summary>
public sealed class ImportMnemonicMenu : WalletLoadMenuBase<ImportMnemonicMenu>, IEnterButtonObservable, ITabButtonObservable
{
	public override event Action OnWalletLoading;

	[SerializeField] private Button nextButton;

	[SerializeField] private HopeInputField[] wordFields;

    private ImportMnemonicMenuAnimator importMneomonicMenuAnimator;
    private ButtonClickObserver buttonObserver;
    private DynamicDataCache dynamicDataCache;

    private bool checkingWallet;

	public List<Selectable> SelectableFields { get; } = new List<Selectable>();

	public Selectable LastSelectableField { get; set; }

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
	private void Start()
	{
		importMneomonicMenuAnimator = transform.GetComponent<ImportMnemonicMenuAnimator>();
		nextButton.onClick.AddListener(LoadWallet);
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

        wordFields.ForEach(inputField => inputField.Text = string.Empty);
    }

    /// <summary>
    /// Loads the wallet if the button is enabled.
    /// </summary>
    /// <param name="clickType"> The enter button ClickType </param>
    public void EnterButtonPressed(ClickType clickType)
    {
		if (clickType != ClickType.Down || checkingWallet)
			return;

		if (InputFieldUtils.GetActiveInputField() == LastSelectableField && nextButton.interactable)
			nextButton.Press();
		else
			SelectableFields.MoveToNextSelectable();
	}

	/// <summary>
	/// Moves to the next input field
	/// </summary>
	/// <param name="clickType"> The tab button ClickType </param>
	public void TabButtonPressed(ClickType clickType)
	{
		if (clickType != ClickType.Down || checkingWallet)
			return;

		SelectableFields.MoveToNextSelectable();
	}

	/// <summary>
	/// Sends a message to the UserWalletManager to create a wallet with the InputField text as the mnemonic phrase.
	/// </summary>
    public override void LoadWallet()
    {
        checkingWallet = true;

        if (CheckCreatedMnemonic())
		{
			OnWalletLoading?.Invoke();
			userWalletManager.CreateWallet();
		}
        else
        {
            checkingWallet = false;
        }
	}

    /// <summary>
    /// Checks if data already exists in the DynamicDataCache.
    /// If the data is equal to the mnemonic entered in the input fields, display the ConfirmMnemonicMenu.
    /// </summary>
    /// <returns> True if the mnemonic was unique and the wallet can be directly imported. </returns>
    private bool CheckCreatedMnemonic()
    {
        Wallet wallet = null;

        string newMnemonic = string.Join(" ", wordFields.Select(field => field.Text)).Trim();
        byte[] seed = (byte[])dynamicDataCache.GetData("seed");

        try
        {
            wallet = new Wallet(newMnemonic, null, WalletUtils.DetermineCorrectPath(newMnemonic));
			nextButton.GetComponent<InteractableButton>().OnCustomPointerExit();
		}
        catch
        {
			nextButton.interactable = false;
			importMneomonicMenuAnimator.AnimateIcon(importMneomonicMenuAnimator.nextButtonErrorIcon);
			importMneomonicMenuAnimator.AnimateIcon(importMneomonicMenuAnimator.nextButtonErrorMessage);
			importMneomonicMenuAnimator.OpeningWallet = false;
			return false;
        }

        if (seed?.SequenceEqual(wallet.Seed) == true)
        {
            uiManager.OpenMenu<ConfirmMnemonicMenu>();
            return false;
        }
        else
        {
            SetWalletInfo(wallet);
            return true;
        }
    }

    /// <summary>
    /// Sets the wallet info to the dynamic data cache.
    /// </summary>
    /// <param name="wallet"> The wallet we are importing. </param>
    private void SetWalletInfo(Wallet wallet)
    {
        dynamicDataCache.SetData("seed", wallet.Seed);
        dynamicDataCache.SetData("mnemonic", null);
    }
}