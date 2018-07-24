using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System.Linq;
using System.Numerics;
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

    public AssetManager Asset { get; private set; }

    public AmountManager Amount { get; private set; }

    public AddressManager Address { get; private set; }

    public GasManager Gas { get; private set; }

    [Inject]
    public void Construct(
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        EtherBalanceObserver etherBalanceObserver,
        GasPriceObserver gasPriceObserver,
        UpdateManager updateManager)
    {
        Asset = new AssetManager(tradableAssetManager, tradableAssetImageManager, etherBalanceObserver, updateManager, assetSymbol, assetBalance, assetImage);
        Gas = new GasManager(tradableAssetManager, gasPriceObserver, advancedModeToggle, transactionSpeedSlider, gasLimitField, gasPriceField);
        Address = new AddressManager(addressField);
        Amount = new AmountManager(this, tradableAssetManager, maxToggle, amountField);
    }

    private void Update()
    {
        bool isValid = Gas.IsValid && Address.IsValid && Amount.IsValid;
        Debug.Log(isValid);
    }

    private void OnDestroy()
    {
        Asset.Destroy();
        Gas.Destroy();
    }
}
