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
public sealed class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>, IStandardGasPriceObservable
{
    public TMP_InputField addressField,
                          amountField,
                          gasLimitField,
                          gasPriceField;

    public TMP_Text assetBalance,
                    assetSymbol;

    public Toggle advancedModeToggle;
    public Image assetImage;
    public Slider transactionSpeedSlider;

    private SendTokenPopupAnimator sendAssetPopupAnimator;

    private SendAssetPopupAssetManager assetManager;
    private SendAssetPopupGasManager gasManager;

    public GasPrice StandardGasPrice { get; set; }

    [Inject]
    public void Construct(
        TradableAssetManager tradableAssetManager,
        TradableAssetImageManager tradableAssetImageManager,
        EtherBalanceObserver etherBalanceObserver,
        GasPriceObserver gasPriceObserver,
        UpdateManager updateManager)
    {
        assetManager = new SendAssetPopupAssetManager(tradableAssetManager, tradableAssetImageManager, etherBalanceObserver, updateManager, assetSymbol, assetBalance, assetImage);
        gasManager = new SendAssetPopupGasManager(gasPriceObserver, transactionSpeedSlider, gasLimitField, gasPriceField);

    }

    protected override void OnStart()
    {
        sendAssetPopupAnimator = GetComponent<SendTokenPopupAnimator>();
    }

    private void OnDestroy()
    {
        assetManager.Destroy();
        gasManager.Destroy();
    }

    public void OnGasPricesUpdated()
    {
    }
}
