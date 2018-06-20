using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NethereumTesting : MonoBehaviour, IGasPriceObservable
{

    [Inject]
    private GasPriceObserver gasPriceObserver;

    public GasPrice StandardGasPrice { get; set; }
    public GasPrice SlowGasPrice { get; set; }
    public GasPrice FastGasPrice { get; set; }

    private void Start()
    {
        gasPriceObserver.SubscribeObservable(this);
    }

    private void Update()
    {
        Debug.Log("Slow => " + SlowGasPrice.ReadableGasPrice + ", Standard => " + StandardGasPrice.ReadableGasPrice + ", Fast => " + FastGasPrice.ReadableGasPrice);
    }
}