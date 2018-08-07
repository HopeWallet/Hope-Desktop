using System.Numerics;

/// <summary>
/// Class which contains all necessary info of an ethereum transaction.
/// </summary>
public sealed class TransactionInfo
{

    public enum TransactionType { Send, Receive };

    /// <summary>
    /// The type of the transaction.
    /// </summary>
    public TransactionType Type { get; }

    /// <summary>
    /// The address that sent the transaction.
    /// </summary>
    public string From { get; }

    /// <summary>
    /// The destination address of the transaction.
    /// </summary>
    public string To { get; }

    /// <summary>
    /// The asset address of the transaction.
    /// </summary>
    public string AssetAddress { get; }

    /// <summary>
    /// The transaction hash of the transaction.
    /// </summary>
    public string TxHash { get; }

    /// <summary>
    /// The value that was sent with the transaction.
    /// </summary>
    public BigInteger Value { get; }

    /// <summary>
    /// The gas price of the transaction.
    /// </summary>
    public BigInteger GasPrice { get; }

    /// <summary>
    /// The amount of gas used for the transaction.
    /// </summary>
    public int GasUsed { get; }

    /// <summary>
    /// The unix time stamp when this transaction was sent.
    /// </summary>
    public int TimeStamp { get; }

    /// <summary>
    /// Initializes the TransactionInfo by assigning all properties.
    /// </summary>
    /// <param name="type"> The type of the transaction. </param>
    /// <param name="from"> The address sending the transaction. </param>
    /// <param name="to"> The destination address of the transaction. </param>
    /// <param name="assetAddress"> The asset address of the transaction. </param>
    /// <param name="txHash"> The transaction hash. </param>
    /// <param name="value"> The value of the asset sent with the transaction. </param>
    /// <param name="gasPrice"> The gas price used to send the transaction. </param>
    /// <param name="gasUsed"> The amount of gas used sending the transaction. </param>
    /// <param name="timeStamp"> The unix time stamp of the transaction. </param>
    public TransactionInfo(
        TransactionType type,
        string from,
        string to,
        string assetAddress,
        string txHash,
        BigInteger value,
        BigInteger gasPrice,
        int gasUsed,
        int timeStamp)
    {
        Type = type;
        From = from;
        To = to;
        AssetAddress = assetAddress.ToLower();
        TxHash = txHash;
        Value = value;
        GasPrice = gasPrice;
        GasUsed = gasUsed;
        TimeStamp = timeStamp;
    }

}
