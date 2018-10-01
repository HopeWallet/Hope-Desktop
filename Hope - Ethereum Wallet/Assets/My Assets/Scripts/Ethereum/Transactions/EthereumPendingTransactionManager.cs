using Hope.Utils.Promises;
using System;

/// <summary>
/// Class used for managing the pending transactions.
/// </summary>
public sealed class EthereumPendingTransactionManager
{
    public event Action<string, string> OnNewTransactionPending;
    public event Action OnTransactionSuccessful;
    public event Action OnTransactionUnsuccessful;

    private string pendingTxHash;

    /// <summary>
    /// Whether a transaction is currently pending or not.
    /// </summary>
    public bool IsTransactionPending { get; private set; }

    /// <summary>
    /// Starts a new pending transaction.
    /// </summary>
    /// <param name="ethTransactionPromise"> The transaction promise returning the result of the transaction. </param>
    /// <param name="txHash"> The transaction hash of the transaction. </param>
    /// <param name="message"> The message representing the transaction. </param>
    public void StartNewPendingTransaction(EthTransactionPromise ethTransactionPromise, string txHash, string message)
    {
        pendingTxHash = txHash;
        OnNewTransactionPending?.Invoke(txHash, message);

        IsTransactionPending = true;

        ethTransactionPromise.OnSuccess(_ => PendingTransactionSuccessful(txHash)).OnError(_ => PendingTransactionUnsuccessful(txHash));
    }

    /// <summary>
    /// Called when the transaction returns a successful result.
    /// </summary>
    /// <param name="txHash"> The transaction hash of the transaction. </param>
    private void PendingTransactionSuccessful(string txHash)
    {
        if (txHash != pendingTxHash)
            return;

        IsTransactionPending = false;
        OnTransactionSuccessful?.Invoke();
    }

    /// <summary>
    /// Called when the transaction returns an unsuccessful result.
    /// </summary>
    /// <param name="txHash"> The transaction of the transaction. </param>
    private void PendingTransactionUnsuccessful(string txHash)
    {
        if (txHash != pendingTxHash)
            return;

        IsTransactionPending = false;
        OnTransactionUnsuccessful?.Invoke();
    }
}