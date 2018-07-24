using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public sealed partial class SendAssetPopup : OkCancelPopupComponent<SendAssetPopup>
{
    public sealed class GasManager : IStandardGasPriceObservable
    {

        public GasPrice StandardGasPrice { get; set; }

        public GasManager(
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
}