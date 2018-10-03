using Hope.Utils.Promises;
using System;
using System.Collections.Generic;
using NBitcoin;

/// <summary>
/// Class used for managing the pending transactions.
/// </summary>
public sealed partial class EthereumPendingTransactionManager
{
    public event Action<string, string> OnNewTransactionPending;
    public event Action<string> OnTransactionSuccessful;
    public event Action<string> OnTransactionUnsuccessful;

    private readonly Dictionary<string, PendingTransaction> transactionsByAddress = new Dictionary<string, PendingTransaction>();
    private readonly Dictionary<string, PendingTransaction> transactionsByHash = new Dictionary<string, PendingTransaction>();

    /// <summary>
    /// Gets a PendingTransaction given the address or transaction hash.
    /// </summary>
    /// <param name="addressOrHashValue"> The address or transaction hash to get the PendingTransaction for. </param>
    /// <returns> The PendingTransaction instance found at the address or tx hash. </returns>
    public PendingTransaction GetPendingTransaction(string addressOrHashValue)
    {
        addressOrHashValue = addressOrHashValue.ToLower();

        if (transactionsByAddress.ContainsKey(addressOrHashValue))
            return transactionsByAddress[addressOrHashValue];
        if (transactionsByHash.ContainsKey(addressOrHashValue))
            return transactionsByHash[addressOrHashValue];

        return null;
    }

    /// <summary>
    /// Starts a new pending transaction.
    /// </summary>
    /// <param name="ethTransactionPromise"> The transaction promise returning the result of the transaction. </param>
    /// <param name="txHash"> The transaction hash of the transaction. </param>
    /// <param name="addressFrom"> The address the transaction was sent from. </param>
    /// <param name="message"> The message representing the transaction. </param>
    public void StartNewPendingTransaction(EthTransactionPromise ethTransactionPromise, string txHash, string addressFrom, string message)
    {
        addressFrom = addressFrom.ToLower();
        txHash = txHash.ToLower();

        var pendingTransaction = new PendingTransaction(addressFrom, txHash, message);
        transactionsByAddress.AddOrReplace(addressFrom, pendingTransaction);
        transactionsByHash.AddOrReplace(txHash, pendingTransaction);

        OnNewTransactionPending?.Invoke(txHash, message);

        ethTransactionPromise.OnSuccess(_ => PendingTransactionSuccessful(txHash)).OnError(_ => PendingTransactionUnsuccessful(txHash));
    }

    /// <summary>
    /// Called when the transaction returns a successful result.
    /// </summary>
    /// <param name="txHash"> The transaction hash of the transaction. </param>
    private void PendingTransactionSuccessful(string txHash)
    {
        txHash = txHash.ToLower();

        var txByHash = transactionsByHash[txHash];
        var txByAddress = transactionsByAddress[txByHash.addressFrom];

        txByHash.isPending = false;
        txByHash.result = PendingTransaction.TransactionResult.Success;

        if (txByHash.txHash != txByAddress.txHash)
            return;

        OnTransactionSuccessful?.Invoke(txHash);
    }

    /// <summary>
    /// Called when the transaction returns an unsuccessful result.
    /// </summary>
    /// <param name="txHash"> The transaction of the transaction. </param>
    private void PendingTransactionUnsuccessful(string txHash)
    {
        txHash = txHash.ToLower();

        var txByHash = transactionsByHash[txHash];
        var txByAddress = transactionsByAddress[txByHash.addressFrom];

        txByHash.isPending = false;
        txByHash.result = PendingTransaction.TransactionResult.Failure;

        if (txByHash.txHash != txByAddress.txHash)
            return;

        OnTransactionUnsuccessful?.Invoke(txHash);
    }
}