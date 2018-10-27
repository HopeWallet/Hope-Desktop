using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TradableAssetNotificationManager : IDisposable
{
    public event Action OnNotificationsUpdated;

    private SecurePlayerPrefList<AddressTransactionCount> transactionsByAddress;
    private Dictionary<string, int?> notificationsByAddress = new Dictionary<string, int?>();

    private readonly EthereumNetworkManager.Settings networkSettings;

    private readonly UserWalletManager userWalletManager;
    private readonly EthereumTransactionManager ethereumTransactionManager;
    private readonly LockedPRPSManager lockedPrpsManager;
    private readonly PRPS prpsContract;

    public TradableAssetNotificationManager(
        EthereumNetworkManager.Settings networkSettings,
        DisposableComponentManager disposableComponentManager,
        UserWalletManager userWalletManager,
        TradableAssetManager tradableAssetManager,
        EthereumTransactionManager ethereumTransactionManager,
        LockedPRPSManager lockedPrpsManager,
        PRPS prpsContract)
    {
        this.networkSettings = networkSettings;
        this.userWalletManager = userWalletManager;
        this.ethereumTransactionManager = ethereumTransactionManager;
        this.lockedPrpsManager = lockedPrpsManager;
        this.prpsContract = prpsContract;

        disposableComponentManager.AddDisposable(this);

        UserWalletManager.OnWalletLoadSuccessful += LoadNewNotificationList;

        tradableAssetManager.OnTradableAssetAdded += AssetAdded;
        tradableAssetManager.OnTradableAssetRemoved += AssetRemoved;

        ethereumTransactionManager.OnTransactionsAdded += TransactionsUpdated;
        lockedPrpsManager.OnLockedPRPSUpdated += TransactionsUpdated;
    }

    public void Dispose()
    {
        notificationsByAddress.Clear();
    }

    public void LoadNewNotificationList()
    {
        transactionsByAddress = new SecurePlayerPrefList<AddressTransactionCount>(PlayerPrefConstants.ASSET_NOTIFICATIONS, (int)networkSettings.networkType + userWalletManager.GetWalletAddress());

        foreach (var address in notificationsByAddress.Keys.ToList())
            notificationsByAddress[address] = null;
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
}