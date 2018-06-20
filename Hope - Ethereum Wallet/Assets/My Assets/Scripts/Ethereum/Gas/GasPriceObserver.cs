using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System.Collections.Generic;

public class GasPriceObserver : IPeriodicUpdater
{

    private readonly List<IGasPriceObserverBase> gasPriceObservers = new List<IGasPriceObserverBase>();

    private GasPrice slowGasPrice;
    private GasPrice standardGasPrice;
    private GasPrice fastGasPrice;

    public float UpdateInterval => 15f;

    public GasPriceObserver(PeriodicUpdateManager periodicUpdateManager)
    {
        periodicUpdateManager.AddPeriodicUpdater(this);
        PeriodicUpdate();
    }

    public void PeriodicUpdate()
    {
        GetUpdatedGasPrices();
    }

    public void SubscribeGasPriceUpdates(IGasPriceObserverBase gasPriceObserver)
    {
        gasPriceObservers.Add(gasPriceObserver);
        UpdateObserverGasPrices(slowGasPrice, standardGasPrice, fastGasPrice);
    }

    public void UnsubscribeGasPriceUpdates(IGasPriceObserverBase gasPriceObserver)
    {
        gasPriceObservers.Remove(gasPriceObserver);
    }

    private void GetUpdatedGasPrices()
    {
        GasUtils.EstimateGasPrice(GasUtils.GasPriceTarget.Standard, price =>
        {
            UpdateObserverGasPrices(new GasPrice(new HexBigInteger((price * 2) / 3)), 
                                    new GasPrice(new HexBigInteger(price)), 
                                    new GasPrice(new HexBigInteger(price * 2)));
        });
    }

    private void UpdateObserverGasPrices(GasPrice slowPrice, GasPrice standardPrice, GasPrice fastPrice)
    {
        slowGasPrice = slowPrice;
        standardGasPrice = standardPrice;
        fastGasPrice = fastPrice;

        foreach (IGasPriceObserver observer in gasPriceObservers)
        {
            var slowObserver = observer as ISlowGasPriceObserver;
            var standardObserver = observer as IStandardGasPriceObserver;
            var fastObserver = observer as IFastGasPriceObserver;

            if (slowObserver != null)
                slowObserver.SlowGasPrice = slowPrice;
            if (standardObserver != null)
                standardObserver.StandardGasPrice = standardPrice;
            if (fastObserver != null)
                fastObserver.FastGasPrice = fastPrice;
        }
    }

}