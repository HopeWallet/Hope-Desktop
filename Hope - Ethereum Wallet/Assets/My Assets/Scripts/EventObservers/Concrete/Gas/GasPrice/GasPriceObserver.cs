using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System;
using System.Linq;

public class GasPriceObserver : EventObserver<IGasPriceObservableBase>, IPeriodicUpdater
{

    private GasPrice slowGasPrice = default(GasPrice);
    private GasPrice standardGasPrice = default(GasPrice);
    private GasPrice fastGasPrice = default(GasPrice);

    public float UpdateInterval => 15f;

    public GasPriceObserver(PeriodicUpdateManager periodicUpdateManager)
    {
        periodicUpdateManager.AddPeriodicUpdater(this);
        GetUpdatedGasPrices();
    }

    public void PeriodicUpdate() => GetUpdatedGasPrices();

    protected override void OnObservableAdded(IGasPriceObservableBase observable) => UpdateAllGasObservables(standardGasPrice.FunctionalGasPrice);

    private void GetUpdatedGasPrices() => GasUtils.EstimateGasPrice(GasUtils.GasPriceTarget.Standard, price => UpdateAllGasObservables(new HexBigInteger(price)));

    private void UpdateAllGasObservables(HexBigInteger price)
    {
        if (price == null)
            return;

        UpdateObservable<ISlowGasPriceObservable>(ref slowGasPrice, 
                                                  new GasPrice(new HexBigInteger((price.Value * 2) / 3)), 
                                                  observable => observable.SlowGasPrice = slowGasPrice);

        UpdateObservable<IStandardGasPriceObservable>(ref standardGasPrice, 
                                                      new GasPrice(price), 
                                                      observable => observable.StandardGasPrice = standardGasPrice);

        UpdateObservable<IFastGasPriceObservable>(ref fastGasPrice, 
                                                  new GasPrice(new HexBigInteger(price.Value * 2)), 
                                                  observable => observable.FastGasPrice = fastGasPrice);
    }

    private void UpdateObservable<T>(ref GasPrice gasPriceVariable, GasPrice gasPriceValue, Action<T> updateObservableAction) where T : IGasPriceObservableBase
    {
        gasPriceVariable = gasPriceValue;
        observables.OfType<T>().ToList().SafeForEach(observable => updateObservableAction(observable));
    }
}