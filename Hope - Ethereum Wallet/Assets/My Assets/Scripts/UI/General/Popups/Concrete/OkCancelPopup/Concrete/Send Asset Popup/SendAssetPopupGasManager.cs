using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public sealed class SendAssetPopupGasManager : IStandardGasPriceObservable
{

    public GasPrice StandardGasPrice { get; set; }

    public SendAssetPopupGasManager(
        GasPriceObserver gasPriceObserver,
        Slider transactionSpeedSlider,
        TMP_InputField gasLimitField,
        TMP_InputField gasPriceField)
    {

    }

    public void Destroy()
    {

    }

    public void OnGasPricesUpdated()
    {
    }
}