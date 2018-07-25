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

    [SerializeField] private Toggle advancedModeToggle;
    [SerializeField] private Toggle maxToggle;

    [SerializeField] private Image assetImage;
    [SerializeField] private Slider transactionSpeedSlider;
    [SerializeField] private Button contactsButton;

    private UserWalletManager userWalletManager;

    public AssetManager Asset { get; private set; }

    public AmountManager Amount { get; private set; }

    public AddressManager Address { get; private set; }

    public GasManager Gas { get; private set; }

    [Inject]
    public void Construct(
        UserWalletManager userWalletManager,
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        EtherBalanceObserver etherBalanceObserver,
        GasPriceObserver gasPriceObserver,
        UpdateManager updateManager,
        PeriodicUpdateManager periodicUpdateManager)
    {
        this.userWalletManager = userWalletManager;

        Asset = new AssetManager(tradableAssetManager, tradableAssetImageManager, etherBalanceObserver, updateManager, assetSymbol, assetBalance, assetImage);
        Gas = new GasManager(tradableAssetManager, gasPriceObserver, periodicUpdateManager, advancedModeToggle, transactionSpeedSlider, gasLimitField, gasPriceField);
        Address = new AddressManager(addressField);
        Amount = new AmountManager(this, maxToggle, amountField);
    }

    private void Start()
    {
        contactsButton.onClick.AddListener(() => popupManager.GetPopup<ContactsPopup>(true));
    }

    private void Update()
    {
        okButton.interactable = Gas.IsValid && Address.IsValid && Amount.IsValid;
    }

    public override void OkButton()
    {
        userWalletManager.TransferAsset(Asset.ActiveAsset,
                                        new HexBigInteger(Gas.TransactionGasLimit),
                                        Gas.TransactionGasPrice.FunctionalGasPrice,
                                        Address.SendAddress,
                                        Amount.SendableAmount);
    }

    private void OnDestroy()
    {
        Asset.Destroy();
        Gas.Destroy();
    }
}
