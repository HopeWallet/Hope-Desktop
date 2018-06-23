using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System;
using System.Linq;

/// <summary>
/// Class used for observing and giving updates to other classes on the current gas price estimates.
/// </summary>
public class GasPriceObserver : EventObserver<IGasPriceObservableBase>, IPeriodicUpdater
{

    private GasPrice slowGasPrice = default(GasPrice);
    private GasPrice standardGasPrice = default(GasPrice);
    private GasPrice fastGasPrice = default(GasPrice);

    /// <summary>
    /// The update interval in which the gas prices will be re-estimated.
    /// </summary>
    public float UpdateInterval => 15f;

    /// <summary>
    /// Initializes the GasPriceObserver.
    /// </summary>
    /// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
    public GasPriceObserver(PeriodicUpdateManager periodicUpdateManager)
    {
        periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    /// <summary>
    /// Estimates the price every 15 seconds.
    /// </summary>
    public void PeriodicUpdate() => GetUpdatedGasPrices();

    /// <summary>
    /// Updates the gas estimates of the observable instantly when it is added.
    /// </summary>
    /// <param name="observable"> The observable to update the gas prices for. </param>
    protected override void OnObservableAdded(IGasPriceObservableBase observable) => UpdateAllGasObservables(standardGasPrice.FunctionalGasPrice);

    /// <summary>
    /// Gets the newest gas price estimates.
    /// </summary>
    private void GetUpdatedGasPrices() => GasUtils.EstimateGasPrice(GasUtils.GasPriceTarget.Standard, price => UpdateAllGasObservables(new HexBigInteger(price)));

    /// <summary>
    /// Updates the gas prices for all observables.
    /// </summary>
    /// <param name="price"> The current standard gas price estimate. </param>
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

        observables.SafeForEach(observable => observable.OnGasPricesUpdated());
    }

    /// <summary>
    /// Updates all IGasPriceObservableBase with the gas prices required.
    /// </summary>
    /// <typeparam name="T"> The type of the gas price observable which inherits IGasPriceObservableBase. </typeparam>
    /// <param name="gasPriceVariable"> The variable which should get the value of the gas price. </param>
    /// <param name="gasPriceValue"> The newest gas price value. </param>
    /// <param name="updateObservableAction"> Action to call on each observable. </param>
    private void UpdateObservable<T>(ref GasPrice gasPriceVariable, GasPrice gasPriceValue, Action<T> updateObservableAction) where T : IGasPriceObservableBase
    {
        gasPriceVariable = gasPriceValue;
        observables.OfType<T>().ToList().SafeForEach(observable => updateObservableAction(observable));
    }
}