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

    private LockPRPSManager lockPRPSManager;

    public dynamic EtherBalance { get; set; }

    public GasPrice StandardGasPrice { get; set; }

    [Inject]
    public void Construct(LockPRPSManager lockPRPSManager)
    {
        this.lockPRPSManager = lockPRPSManager;
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
    }
}