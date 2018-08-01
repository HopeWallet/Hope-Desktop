using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Zenject;

public sealed class LockPRPSPopup : ExitablePopupComponent<LockPRPSPopup>, IStandardGasPriceObservable, IEtherBalanceObservable
{
	public InfoMessage infoMessage;

    private BigInteger gasLimit;

    public dynamic EtherBalance { get; set; }

    public GasPrice StandardGasPrice { get; set; }

    [Inject]
    public void Construct(TradableAssetManager tradableAssetManager)
    {
    }

    protected override void OnStart()
    {
        infoMessage.PopupManager = popupManager;
    }

    private void EstimateGasLimit(dynamic assetBalance)
    {

    }

    public void OnGasPricesUpdated()
    {
        throw new NotImplementedException();
    }
}