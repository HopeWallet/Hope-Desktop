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
public sealed class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>, IStandardGasPriceObservable, IEtherBalanceObservable
{
    public TMP_InputField addressField,
                          amountField,
                          gasLimitField,
                          gasPriceField;

    public Toggle advancedModeToggle;

    public Image assetImage;

    public TMP_Text assetBalance;

    public Slider transactionSpeedSlider;

    public GasPrice StandardGasPrice { get; set; }

    public dynamic EtherBalance { get; set; }

    protected override void OnStart()
    {

    }

    public void OnGasPricesUpdated()
    {
    }
}
