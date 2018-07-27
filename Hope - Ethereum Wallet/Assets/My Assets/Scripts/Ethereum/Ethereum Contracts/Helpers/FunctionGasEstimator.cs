using Hope.Utils.EthereumUtils;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Numerics;

/// <summary>
/// Class used for estimating the gas for a function and determining if the wallet has enough ether to execute it.
/// </summary>
public class FunctionGasEstimator : IStandardGasPriceObservable, IEtherBalanceObservable
{

    private readonly UserWalletManager userWalletManager;
    private readonly TradableAssetManager tradableAssetManager;
    private readonly GasPriceObserver gasPriceObserver;
    private readonly EtherBalanceObserver etherBalanceObserver;

    private dynamic etherBalance;

    /// <summary>
    /// The currently estimated standard gas price.
    /// </summary>
    public GasPrice StandardGasPrice { get; set; }

    /// <summary>
    /// The estimated gas limit.
    /// </summary>
    public HexBigInteger GasLimit { get; private set; }

    /// <summary>
    /// Whether the transaction can be executed.
    /// </summary>
    public bool CanExecuteTransaction { get; private set; }

    /// <summary>
    /// The current ether balance of the wallet.
    /// </summary>
    public dynamic EtherBalance
    {
        get { return etherBalance; }
        set
        {
            etherBalance = value;
            RecheckIfFunctionCanBeSent();
        }
    }

    /// <summary>
    /// Initializes the FunctionGasEstimator with the required dependencies.
    /// </summary>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
    /// <param name="gasPriceObserver"> The active GasPriceObserver. </param>
    /// <param name="etherBalanceObserver"> The active EtherBalanceObserver. </param>
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

    /// <summary>
    /// Estimates a function and calls an action when the estimate is finished.
    /// </summary>
    /// <param name="function"> The function to estimate. </param>
    /// <param name="onEstimateFinished"> Action to call once the estimate is finished. </param>
    /// <param name="input"> The input of the function. </param>
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

    /// <summary>
    /// Stops the estimator ending all observables.
    /// </summary>
    public void StopEstimation() => ObserverHelpers.UnsubscribeObservables(this, gasPriceObserver, etherBalanceObserver);

    /// <summary>
    /// Called when the gas limit has been received.
    /// </summary>
    /// <param name="onEstimateFinished"> Action to call once the estimate has been received. </param>
    /// <param name="gasLimit"> The estimated gas limit. </param>
    private void OnGasLimitReceived(Action onEstimateFinished, BigInteger gasLimit)
    {
        GasLimit = new HexBigInteger(gasLimit);

        RecheckIfFunctionCanBeSent();
        onEstimateFinished?.Invoke();
    }

    /// <summary>
    /// Called when the gas prices have changed.
    /// </summary>
    public void OnGasPricesUpdated() => RecheckIfFunctionCanBeSent();

    /// <summary>
    /// Rechecks if the function can be executed once the ether balance changes, the gas price changes, or the gas limit is received.
    /// </summary>
    private void RecheckIfFunctionCanBeSent()
    {
        if (GasLimit == null)
            return;

        CanExecuteTransaction = etherBalance > GasUtils.CalculateMaximumGasCost(StandardGasPrice.FunctionalGasPrice.Value, GasLimit.Value);
    }
}