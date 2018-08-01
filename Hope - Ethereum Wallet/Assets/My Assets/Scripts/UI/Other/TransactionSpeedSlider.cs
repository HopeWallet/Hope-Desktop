using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public sealed class TransactionSpeedSlider : IStandardGasPriceObservable
{
    private readonly GasPriceObserver gasPriceObserver;
    private readonly Slider slider;
    private readonly Action<GasPrice> onGasPriceChanged;

    public GasPrice StandardGasPrice { get; set; }

    public TransactionSpeedSlider(
        GasPriceObserver gasPriceObserver,
        Slider slider,
        Action<GasPrice> onGasPriceChanged)
    {
        this.gasPriceObserver = gasPriceObserver;
        this.slider = slider;
        this.onGasPriceChanged = onGasPriceChanged;
    }

    public void Start()
    {
        gasPriceObserver.SubscribeObservable(this);
        slider.onValueChanged.AddListener(UpdateGasPriceEstimate);

        UpdateGasPriceEstimate(slider.value);
    }

    public void Stop()
    {
        gasPriceObserver.UnsubscribeObservable(this);
        slider.onValueChanged.RemoveAllListeners();
    }

    private void UpdateGasPriceEstimate(float value)
    {
        decimal multiplier = decimal.Round((decimal)Mathf.Lerp(0.6f, 1.4f, value) * (decimal)Mathf.Lerp(1f, 4f, value - 0.45f), 2, MidpointRounding.AwayFromZero);
        onGasPriceChanged?.Invoke(new GasPrice(new BigInteger(multiplier * (decimal)StandardGasPrice.FunctionalGasPrice.Value)));
    }

    public void OnGasPricesUpdated()
    {
        UpdateGasPriceEstimate(slider.value);
    }
}