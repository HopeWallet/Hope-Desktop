using Nethereum.Hex.HexTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Class which displays the popup for sending a TradableAsset.
/// </summary>
public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    [SerializeField] private TMP_InputField addressField;
    [SerializeField] private TMP_InputField amountField;
    [SerializeField] private TMP_InputField gasLimitField;
    [SerializeField] private TMP_InputField gasPriceField;

	[SerializeField] private TMP_Text assetBalance;
    [SerializeField] private TMP_Text assetSymbol;
    [SerializeField] private TMP_Text transactionFee;
	[SerializeField] private TMP_Text contactName;

	[SerializeField] private Toggle advancedModeToggle;
    [SerializeField] private Toggle maxToggle;

    [SerializeField] private Image assetImage;
    [SerializeField] private Slider transactionSpeedSlider;
    [SerializeField] private Button contactsButton;

	[SerializeField] private InfoMessage infoMessage;

    private UserWalletManager userWalletManager;
    private DynamicDataCache dynamicDataCache;

    /// <summary>
    /// The <see cref="AssetManager"/> of this <see cref="SendAssetPopup"/>.
    /// </summary>
    public AssetManager Asset { get; private set; }

    /// <summary>
    /// The <see cref="AmountManager"/> of this <see cref="SendAssetPopup"/>.
    /// </summary>
    public AmountManager Amount { get; private set; }

    /// <summary>
    /// The <see cref="AddressManager"/> of this <see cref="SendAssetPopup"/>.
    /// </summary>
    public AddressManager Address { get; private set; }

    /// <summary>
    /// The <see cref="GasManager"/> of this <see cref="SendAssetPopup"/>.
    /// </summary>
    public GasManager Gas { get; private set; }

    /// <summary>
    /// Adds the required dependencies to the SendAssetPopup.
    /// </summary>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="etherBalanceObserver"> The active EtherBalanceObserver. </param>
    /// <param name="gasPriceObserver"> The active GasPriceObserver. </param>
    /// <param name="updateManager"> The active UpdateManager. </param>
    /// <param name="dynamicDataCache"> The active DynamicDataCache. </param>
    /// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
    [Inject]
    public void Construct(
        UserWalletManager userWalletManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        EtherBalanceObserver etherBalanceObserver,
        GasPriceObserver gasPriceObserver,
        UpdateManager updateManager,
        DynamicDataCache dynamicDataCache,
        PeriodicUpdateManager periodicUpdateManager)
    {
        this.userWalletManager = userWalletManager;
        this.dynamicDataCache = dynamicDataCache;

        Asset = new AssetManager(tradableAssetManager, tradableAssetImageManager, etherBalanceObserver, updateManager, assetSymbol, assetBalance, assetImage);
        Gas = new GasManager(tradableAssetManager, gasPriceObserver, periodicUpdateManager, advancedModeToggle, transactionSpeedSlider, gasLimitField, gasPriceField, transactionFee);
        Address = new AddressManager(addressField, contactName);
        Amount = new AmountManager(this, maxToggle, amountField);
    }

    /// <summary>
    /// Sets up the contacts button and info message.
    /// </summary>
    protected override void OnStart()
    {
        contactsButton.onClick.AddListener(() => popupManager.GetPopup<ContactsPopup>(true).SetSendAssetPopup(this));
		infoMessage.PopupManager = popupManager;
	}

    /// <summary>
    /// Updates the send button interactability based on the GasManager, AddressManager, AmountManager IsValid properties.
    /// </summary>
    private void Update()
    {
        okButton.interactable = Gas.IsValid && Address.IsValid && Amount.IsValid;
    }

    /// <summary>
    /// Starts the asset transfer.
    /// </summary>
    public override void OkButton()
    {
        dynamicDataCache.SetData("txfee", Gas.TransactionFee.ToString());
        userWalletManager.TransferAsset(Asset.ActiveAsset,
                                        new HexBigInteger(Gas.TransactionGasLimit),
                                        Gas.TransactionGasPrice.FunctionalGasPrice,
                                        Address.SendAddress,
                                        Amount.SendableAmount);
    }

    /// <summary>
    /// Destroys the AssetManager and GasManager.
    /// </summary>
    private void OnDestroy()
    {
        Asset.Destroy();
        Gas.Destroy();
    }
}
