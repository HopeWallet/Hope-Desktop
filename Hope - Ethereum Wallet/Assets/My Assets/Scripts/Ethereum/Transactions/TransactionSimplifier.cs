using System.Numerics;

/// <summary>
/// Class which is used to create a simplified container of the transaction data, called TransactionInfo.
/// </summary>
public class TransactionSimplifier
{

    /// <summary>
    /// Creates a TransactionInfo object from an ethereum transaction json object.
    /// </summary>
    /// <param name="transaction"> The ethereum transaction json to convert. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <returns> The TransactionInfo created from the json. </returns>
    public static TransactionInfo CreateEtherTransaction(EtherTransactionJson transaction, UserWalletManager userWalletManager)
    {
        var value = BigInteger.Parse(transaction.value);

        if (transaction.from == transaction.to || value == 0)
            return null;

        return new TransactionInfo(GetTransactionType(transaction.from.EqualsIgnoreCase(userWalletManager.WalletAddress)),
                                         transaction.from,
                                         transaction.to,
                                         EtherAsset.ETHER_ADDRESS,
                                         transaction.hash,
                                         value,
                                         string.IsNullOrEmpty(transaction.gasPrice) ? new BigInteger(0) : BigInteger.Parse(transaction.gasPrice),
                                         transaction.gasUsed,
                                         transaction.timeStamp);
    }

    /// <summary>
    /// Creates a TransactionInfo object from a token transaction json object.
    /// </summary>
    /// <param name="tokenTransaction"> The token json to convert. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <returns> The TransactionInfo created from the json. </returns>
    public static TransactionInfo CreateTokenTransaction(TokenTransactionJson tokenTransaction, UserWalletManager userWalletManager)
    {
        var topics = tokenTransaction.topics;
        var tokenSender = topics[1].Remove(2, 24);
        var tokenReceiver = topics[2].Remove(2, 24);

        if (tokenSender == tokenReceiver)
            return null;

        return new TransactionInfo(GetTransactionType(tokenSender.EqualsIgnoreCase(userWalletManager.WalletAddress)),
                                        tokenSender,
                                        tokenReceiver,
                                        tokenTransaction.address,
                                        tokenTransaction.transactionHash,
                                        tokenTransaction.data.ConvertFromHex(),
                                        tokenTransaction.gasPrice.ConvertFromHex(),
                                        (int)tokenTransaction.gasUsed.ConvertFromHex(),
                                        (long)tokenTransaction.timeStamp.ConvertFromHex());
    }

    /// <summary>
    /// Gets the TransactionType of a transaction.
    /// </summary>
    /// <param name="isSendTransaction"> Whether the transaction is a send transaction. </param>
    /// <returns> The TransactionType of the transaction. </returns>
    private static TransactionInfo.TransactionType GetTransactionType(bool isSendTransaction) 
        => isSendTransaction ? TransactionInfo.TransactionType.Send : TransactionInfo.TransactionType.Receive;

}
