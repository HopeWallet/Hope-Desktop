using Hope.Utils.EthereumUtils;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Numerics;

public class FunctionGasEstimator : IStandardGasPriceObservable, IEtherBalanceObservable
{

    private readonly UserWalletManager userWalletManager;
    private readonly TradableAssetManager tradableAssetManager;
    private readonly GasPriceObserver gasPriceObserver;
    private readonly EtherBalanceObserver etherBalanceObserver;

    private dynamic etherBalance;

    public GasPrice StandardGasPrice { get; set; }

    public HexBigInteger GasLimit { get; private set; }

    public bool CanExecuteTransaction { get; private set; }

    public dynamic EtherBalance
    {
        get
        {
            return etherBalance;
        }
        set
        {
            etherBalance = value;
            RecheckIfFunctionCanBeSent();
        }
    }

    public FunctionGasEstimator(UserWalletManager userWalletManager, 
        TradableAssetManager tradableAssetManager,
        GasPriceObserver gasPriceObserver,
        EtherBalanceObserver etherBalanceObserver)
    {
        this.userWalletManager = userWalletManager;
        this.tradableAssetManager = tradableAssetManager;
        this.gasPriceObserver = gasPriceObserver;
        this.etherBalanceObserver = etherBalanceObserver;
    }

    public void Estimate(Function function, Action onEstimateFinished, params object[] input)
    {
        if (GasLimit == null)
        {
            ObserverHelpers.SubscribeObservables(this, gasPriceObserver, etherBalanceObserver);
            GasUtils.EstimateGasLimit(function, userWalletManager.WalletAddress, limit => OnGasLimitReceived(onEstimateFinished, limit), input);
        }
        else
        {
            OnGasLimitReceived(onEstimateFinished, GasLimit.Value);
        }
    }

    public void StopEstimation()
    {
        ObserverHelpers.UnsubscribeObservables(this, gasPriceObserver, etherBalanceObserver);
    }

    private void OnGasLimitReceived(Action onEstimateFinished, BigInteger gasLimit)
    {
        GasLimit = new HexBigInteger(gasLimit);

        RecheckIfFunctionCanBeSent();
        onEstimateFinished?.Invoke();
    }

    public void OnGasPricesUpdated() => RecheckIfFunctionCanBeSent();

    private void RecheckIfFunctionCanBeSent()
    {
        if (GasLimit == null)
            return;

        CanExecuteTransaction = etherBalance > GasUtils.CalculateMaximumGasCost(StandardGasPrice.FunctionalGasPrice.Value, GasLimit.Value);
    }
}