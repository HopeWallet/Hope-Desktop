using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class which represents a slider for transaction speeds.
/// </summary>
public sealed class TransactionSpeedSlider : IStandardGasPriceObservable
{
    private readonly GasPriceObserver gasPriceObserver;
    private readonly Slider slider;
    private readonly Action<GasPrice> onGasPriceChanged;

    /// <summary>
    /// The current estimated standard gas price.
    /// </summary>
    public GasPrice StandardGasPrice { get; set; }

    /// <summary>
    /// Initializes the TransactionSpeedSlider.
    /// </summary>
    /// <param name="gasPriceObserver"> The current GasPriceObserver. </param>
    /// <param name="slider"> The slider which is used to represent transaction speed. </param>
    /// <param name="onGasPriceChanged"> Action called once the gas price is changed. </param>
    public TransactionSpeedSlider(
        GasPriceObserver gasPriceObserver,
        Slider slider,
        Action<GasPrice> onGasPriceChanged)
    {
        this.gasPriceObserver = gasPriceObserver;
        this.slider = slider;
        this.onGasPriceChanged = onGasPriceChanged;
    }

    /// <summary>
    /// Starts the TransactionSpeedSlider.
    /// </summary>
    public void Start()
    {
        gasPriceObserver.SubscribeObservable(this);
        slider.onValueChanged.AddListener(UpdateGasPriceEstimate);

        UpdateGasPriceEstimate(slider.value);
    }

    /// <summary>
    /// Stops the TransactionSpeedSlider.
    /// </summary>
    public void Stop()
    {
        gasPriceObserver.UnsubscribeObservable(this);
        slider.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Updates the newest gas price estimates.
    /// </summary>
    /// <param name="value"> The current value of the slider to use to estimate the gas price. </param>
    private void UpdateGasPriceEstimate(float value)
    {
        if (EqualityComparer<GasPrice>.Default.Equals(StandardGasPrice, default(GasPrice)))
            return;

        decimal multiplier = decimal.Round((decimal)Mathf.Lerp(0.65f, 1.4f, value) * (decimal)Mathf.Lerp(1f, 4f, value - 0.45f), 2, MidpointRounding.AwayFromZero);
        onGasPriceChanged?.Invoke(new GasPrice(new BigInteger(multiplier * (decimal)StandardGasPrice.FunctionalGasPrice.Value)));
    }

    /// <summary>
    /// Called when the gas price estimate is changed.
    /// </summary>
    public void OnGasPricesUpdated() => UpdateGasPriceEstimate(slider.value);
}