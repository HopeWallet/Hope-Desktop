using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TradableAssetNotificationManager
{
    public event Action OnNotificationsUpdated;

    private readonly SecurePlayerPrefList<AddressTransactionCount> transactionsByAddress;
    private readonly Dictionary<string, int?> notificationsByAddress;

    private readonly EthereumTransactionManager ethereumTransactionManager;
    private readonly LockedPRPSManager lockedPrpsManager;
    private readonly PRPS prpsContract;

    public TradableAssetNotificationManager(
        Settings settings,
        EthereumTransactionManager ethereumTransactionManager,
        LockedPRPSManager lockedPrpsManager,
        PRPS prpsContract)
    {
        this.ethereumTransactionManager = ethereumTransactionManager;
        this.lockedPrpsManager = lockedPrpsManager;
        this.prpsContract = prpsContract;

        transactionsByAddress = new SecurePlayerPrefList<AddressTransactionCount>(settings.prefName);
        notificationsByAddress = new Dictionary<string, int?>();

        TradableAssetManager.OnTradableAssetAdded += AssetAdded;
        TradableAssetManager.OnTradableAssetRemoved += AssetRemoved;
        EthereumTransactionManager.OnTransactionsAdded += TransactionsUpdated;
        lockedPrpsManager.OnLockedPRPSUpdated += TransactionsUpdated;
    }

    public int? GetAssetNotificationCount(string assetAddress)
    {
        return notificationsByAddress.ContainsKey(assetAddress) ? notificationsByAddress[assetAddress] : null;
    }

    public void SaveTransactionCount(string assetAddress)
    {
        if (!notificationsByAddress.ContainsKey(assetAddress))
            return;

        var txCount = ethereumTransactionManager.GetTransactionListByAddress(assetAddress)?.Count;

        if (txCount == null)
            return;

        var addressTxCount = new AddressTransactionCount(assetAddress, txCount.Value);

        if (transactionsByAddress.Contains(assetAddress))
            transactionsByAddress[assetAddress] = addressTxCount;
        else
            transactionsByAddress.Add(addressTxCount);

        TransactionsUpdated();
    }

    private void TransactionsUpdated()
    {
        foreach (var address in notificationsByAddress.Keys.ToList())
        {
            var txCount = ethereumTransactionManager.GetTransactionListByAddress(address)?.Count;

            if (txCount == null)
                continue;

            notificationsByAddress[address] = txCount - (transactionsByAddress.Contains(address) ? transactionsByAddress[address].transactionCount : 0);

            if (address == prpsContract.ContractAddress)
                notificationsByAddress[address] += lockedPrpsManager.UnlockableItems.Count;
        }

        OnNotificationsUpdated?.Invoke();
    }

    private void AssetAdded(TradableAsset tradableAsset)
    {
        notificationsByAddress.Add(tradableAsset.AssetAddress.ToLower(), null);
    }

    private void AssetRemoved(TradableAsset tradableAsset)
    {
        notificationsByAddress.Remove(tradableAsset.AssetAddress);
    }

    [Serializable]
    private sealed class AddressTransactionCount
    {
        public string address;
        public int transactionCount;

        public AddressTransactionCount(string address, int transactionCount)
        {
            this.address = address;
            this.transactionCount = transactionCount;
        }
    }

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string prefName;
    }
}