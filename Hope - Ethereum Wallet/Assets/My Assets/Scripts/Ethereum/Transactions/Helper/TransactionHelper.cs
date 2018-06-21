using Hope.Utils.EthereumUtils;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using System.Numerics;

public class TransactionHelper : IPeriodicUpdater
{

    private readonly PeriodicUpdateManager periodicUpdateManager;
    private readonly TradableAssetManager tradableAssetManager;
    private readonly UserWalletManager userWalletManager;

    private HexBigInteger gasLimit;
    private HexBigInteger gasPrice;

    private Function functionToEstimate;
    private Action onEstimateFinished;
    private object[] input;

    public float UpdateInterval => 10f;

    public HexBigInteger GasLimit { get { return gasLimit; } }

    public HexBigInteger GasPrice { get { return gasPrice; } }

    public bool CanExecuteTransaction { get; private set; }

    public TransactionHelper(PeriodicUpdateManager periodicUpdateManager, TradableAssetManager tradableAssetManager, UserWalletManager userWalletManager)
    {
        this.periodicUpdateManager = periodicUpdateManager;
        this.tradableAssetManager = tradableAssetManager;
        this.userWalletManager = userWalletManager;
    }

    public void Start(Function functionToEstimate, Action onEstimateFinished = null, params object[] input)
    {
        var lastFunction = this.functionToEstimate;

        this.input = input;
        this.onEstimateFinished = onEstimateFinished;
        this.functionToEstimate = functionToEstimate;

        if (lastFunction == null)
            periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    public void Stop()
    {
        periodicUpdateManager.RemovePeriodicUpdater(this);
    }

    public void PeriodicUpdate()
    {
        EstimateGasLimit();
        EstimateGasPrice();
    }

    private void EstimateGasLimit()
    {
        if (GasLimit != null)
            return;

        GasUtils.EstimateGasLimit(functionToEstimate, userWalletManager.WalletAddress, limit => UpdateValue(ref gasLimit, limit), input);
    }

    private void EstimateGasPrice()
    {
        GasUtils.EstimateGasPrice(GasUtils.GasPriceTarget.Standard, price => UpdateValue(ref gasPrice, price));
    }

    private void UpdateValue(ref HexBigInteger value, BigInteger valueToSet)
    {
        value = new HexBigInteger(valueToSet);

        if (GasLimit == null || GasPrice == null)
            return;

        EstimateTransactionViability();
    }

    private void EstimateTransactionViability()
    {
        var etherAsset = tradableAssetManager.EtherAsset;

        etherAsset.UpdateBalance(() =>
        {
            CanExecuteTransaction = etherAsset.AssetBalance > GasUtils.CalculateMaximumGasCost(GasPrice.Value, GasLimit.Value);
            onEstimateFinished?.Invoke();
        });
    }

}