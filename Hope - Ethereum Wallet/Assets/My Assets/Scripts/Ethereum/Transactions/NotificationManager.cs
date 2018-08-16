using System;

public sealed class NotificationManager
{
    public event Action OnNotificationsUpdated;

    private readonly SecurePlayerPrefList<AddressTransactionCount> transactionsByAddress;

    private readonly LockedPRPSManager lockedPrpsManager;

    public NotificationManager(Settings settings, LockedPRPSManager lockedPrpsManager)
    {
        this.lockedPrpsManager = lockedPrpsManager;
        transactionsByAddress = new SecurePlayerPrefList<AddressTransactionCount>(settings.prefName);

        EthereumTransactionManager.OnTransactionsAdded += TransactionsUpdated;
    }

    private void TransactionsUpdated()
    {

    }

    [Serializable]
    private sealed class AddressTransactionCount
    {
        public string address;
        public int transactionCount;
    }

    [Serializable]
    public sealed class Settings
    {
        [RandomizeText] public string prefName;
    }
}