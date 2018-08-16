using System;
using System.Collections.Generic;

public sealed class NotificationManager
{
    public event Action OnNotificationsUpdated;

    private readonly SecurePlayerPrefList<AddressTransactionCount> transactionsByAddress;
    private readonly Dictionary<string, int?> notificationsByAddress;

    private readonly EthereumTransactionManager ethereumTransactionManager;
    private readonly LockedPRPSManager lockedPrpsManager;
    private readonly PRPS prpsContract;

    public NotificationManager(
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

    public void SaveNewTransactions(string assetAddress)
    {
        if (!notificationsByAddress.ContainsKey(assetAddress))
            return;

        var addressTxCount = new AddressTransactionCount(assetAddress, ethereumTransactionManager.GetTransactionListByAddress(assetAddress).Count);

        if (transactionsByAddress.Contains(assetAddress))
            transactionsByAddress[assetAddress] = addressTxCount;
        else
            transactionsByAddress.Add(addressTxCount);
    }

    private void TransactionsUpdated()
    {
        foreach (var address in notificationsByAddress.Keys)
        {
            var txCount = ethereumTransactionManager.GetTransactionListByAddress(address)?.Count;

            if (txCount == null)
                continue;

            notificationsByAddress[address] = txCount - transactionsByAddress[address].transactionCount;
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