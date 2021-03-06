﻿
/// <summary>
/// Base interface to implement for classes that require gas prices.
/// </summary>
public interface IGasPriceObservableBase
{
    /// <summary>
    /// Called after the gas prices have been updated again.
    /// </summary>
    void OnGasPricesUpdated();
}

/// <summary>
/// Interface to extend when you need to know the current standard transaction speed gas price.
/// </summary>
public interface IStandardGasPriceObservable : IGasPriceObservableBase
{
    /// <summary>
    /// The gas price for medium transaction speed.
    /// </summary>
    GasPrice StandardGasPrice { get; set; }
}

/// <summary>
/// Interface to extend when you need to know the current slow transaction speed gas price.
/// </summary>
public interface ISlowGasPriceObservable : IGasPriceObservableBase
{
    /// <summary>
    /// The gas price for slow transaction speed.
    /// </summary>
    GasPrice SlowGasPrice { get; set; }
}

/// <summary>
/// Interface to extend when you need to know the current fast transaction speed gas price.
/// </summary>
public interface IFastGasPriceObservable : IGasPriceObservableBase
{
    /// <summary>
    /// The gas price for fast transaction speed.
    /// </summary>
    GasPrice FastGasPrice { get; set; }
}

/// <summary>
/// Interface to extend when you need to know the standard, slow, and fast gas prices.
/// </summary>
public interface IGasPriceObservable : IStandardGasPriceObservable, ISlowGasPriceObservable, IFastGasPriceObservable
{
}