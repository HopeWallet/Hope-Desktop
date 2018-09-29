using System;

public sealed class EthereumPendingTransactionManager
{
    public event Action<string, string> OnNewTransactionPending;
    public event Action OnTransactionSuccessful;
    public event Action OnTransactionUnsuccessful;

    private string pendingTxHash;

    public void StartNewPendingTransaction(string txHash, string message)
    {
        pendingTxHash = txHash;
        OnNewTransactionPending?.Invoke(txHash, message);
    }

    public void PendingTransactionSuccessful(string txHash)
    {
        if (txHash != pendingTxHash)
            return;

        OnTransactionSuccessful?.Invoke();
    }

    public void PendingTransactionUnsuccessful(string txHash)
    {
        if (txHash != pendingTxHash)
            return;

        OnTransactionUnsuccessful?.Invoke();
    }
}