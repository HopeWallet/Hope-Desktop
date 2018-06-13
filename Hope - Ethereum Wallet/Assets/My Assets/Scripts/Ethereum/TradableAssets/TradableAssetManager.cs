using System;
using System.Collections.Generic;
using Zenject;

/// <summary>
/// Class which manages all TradableAssets.
/// </summary>
public class TradableAssetManager : IPeriodicUpdater
{

    public static event Action OnBalancesUpdated;
    public static event Action<TradableAsset> OnTradableAssetAdded;
    public static event Action<TradableAsset> OnTradableAssetRemoved;

    /// <summary>
    /// The dictionary of active tradable assets.
    /// </summary>
    public Dictionary<string, TradableAsset> TradableAssets { get; private set; }

    /// <summary>
    /// The actively selected TradableAsset in the wallet.
    /// </summary>
    public TradableAsset ActiveTradableAsset { get; private set; }

    /// <summary>
    /// The TradableAsset that is Ether.
    /// </summary>
    public TradableAsset EtherAsset { get; private set; }

    /// <summary>
    /// The amount of time between each PeriodicUpdate.
    /// </summary>
    public float UpdateInterval => 10f;

    /// <summary>
    /// Initializes the TradableAssetManager by adding required methods to events and adding this class to the PeriodicUpdateManager.
    /// </summary>
    /// <param name="periodicUpdateManager"> The PeriodicUpdateManager to use to run this class's periodic updates. </param>
    [Inject]
    public TradableAssetManager(PeriodicUpdateManager periodicUpdateManager) : base()
    {
        TradableAssets = new Dictionary<string, TradableAsset>();

        TokenContractManager.OnTokenAdded += AddTradableAsset;
        TokenContractManager.OnTokenRemoved += RemoveTradableAsset;

        periodicUpdateManager.AddPeriodicUpdater(this);
    }

    /// <summary>
    /// Adds a new asset to the collection of TradableAssets to manage.
    /// </summary>
    /// <param name="tradableAsset"> The TradableAsset to add to the manager. </param>
    public void AddTradableAsset(TradableAsset tradableAsset)
    {
        var address = tradableAsset.AssetAddress;

        if (TradableAssets.ContainsKey(address))
            return;

        if (tradableAsset is EtherAsset)
        {
            EtherAsset = tradableAsset;
            SetNewActiveAsset(tradableAsset);
        }

        TradableAssets.Add(address, tradableAsset);
        OnTradableAssetAdded?.Invoke(tradableAsset);
    }

    /// <summary>
    /// Sets a new active asset given a TradableAsset.
    /// Active asset is determined based on which button was selected.
    /// </summary>
    /// <param name="tradableAsset"> The new TradableAsset ot be our active asset. </param>
    public void SetNewActiveAsset(TradableAsset tradableAsset)
    {
        ActiveTradableAsset = tradableAsset;
        OnBalancesUpdated?.Invoke();
    }

    /// <summary>
    /// Removecs a TradbaleAsset from this class.
    /// </summary>
    /// <param name="address"> The address of the asset to remove. </param>
    public void RemoveTradableAsset(string address)
    {
        if (!TradableAssets.ContainsKey(address))
            return;

        var asset = TradableAssets[address];
        TradableAssets.Remove(address);
        OnTradableAssetRemoved?.Invoke(asset);
    }

    /// <summary>
    /// Updates the balance of a TradableAsset every so often to reflect realtime changes that might go on.
    /// </summary>
    public void PeriodicUpdate() => TradableAssets.Values.ForEach(asset => asset.UpdateBalance(() => OnBalancesUpdated?.Invoke()));

    /// <summary>
    /// Gets a TradableAsset given its address.
    /// </summary>
    /// <param name="address"> The address of the asset to receive. </param>
    /// <returns></returns>
    public TradableAsset GetTradableAsset(string address) => TradableAssets.ContainsKey(address) ? TradableAssets[address] : null;

}
