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

    // TODO
    // Create new classes
    // SendAssetPopupAddressManager
    // SendAssetPopupAmountManager
    // Move validation logic to each class
    // Remove validation logic from SendTokenPopupAnimator

    public TMP_InputField addressField,
                          amountField,
                          gasLimitField,
                          gasPriceField;

    public TMP_Text assetBalance,
                    assetSymbol;

    public Toggle advancedModeToggle;
    public Image assetImage;
    public Slider transactionSpeedSlider;

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
        Gas = new GasManager(gasPriceObserver, transactionSpeedSlider, gasLimitField, gasPriceField);

    }

    protected override void Awake()
    {
        base.Awake();


    }

    private void OnDestroy()
    {
        Asset.Destroy();
        Gas.Destroy();
    }
}
