using Hope.Utils.EthereumUtils;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Numerics;

public class FunctionHelper : IStandardGasPriceObservable, IEtherBalanceObservable
{

    private readonly UserWalletManager userWalletManager;
    private readonly TradableAssetManager tradableAssetManager;

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

    public FunctionHelper(UserWalletManager userWalletManager, TradableAssetManager tradableAssetManager)
    {
        this.userWalletManager = userWalletManager;
        this.tradableAssetManager = tradableAssetManager;
    }

    public void EstimateFunctionCost(Function function, Action onEstimateFinished, params object[] input)
    {
        if (GasLimit == null)
            GasUtils.EstimateGasLimit(function, userWalletManager.WalletAddress, limit => OnGasLimitReceived(onEstimateFinished, limit), input);
        else
            OnGasLimitReceived(onEstimateFinished, GasLimit.Value);
    }

    public void Stop()
    {
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