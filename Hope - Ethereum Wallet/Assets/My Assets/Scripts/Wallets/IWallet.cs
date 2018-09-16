using Nethereum.JsonRpc.UnityClient;
using System;
using System.Numerics;

/// <summary>
/// Base interface to use for a wallet that contains a list of addresses and can sign a transaction.
/// </summary>
public interface IWallet
{
    /// <summary>
    /// Event called if the wallet is successfully loaded.
    /// </summary>
    event Action OnWalletLoadSuccessful;

    /// <summary>
    /// Event called if the wallet is unsuccessfully loaded.
    /// </summary>
    event Action OnWalletLoadUnsuccessful;

    /// <summary>
    /// Gets the address of the wallet given the index of the address.
    /// </summary>
    /// <param name="addressIndex"> The index to use to retrieve the address. Valid indices range from 0-49. </param>
    /// <returns> Returns the address found at the index. </returns>
    string GetAddress(int addressIndex);

    /// <summary>
    /// Signs a transaction using this IWallet.
    /// </summary>
    /// <typeparam name="T"> The type of the popup to display the transaction confirmation for. </typeparam>
    /// <param name="onTransactionSigned"> The action to call if the transaction is confirmed and signed. </param>
    /// <param name="gasLimit"> The gas limit to use with the transaction. </param>
    /// <param name="gasPrice"> The gas price to use with the transaction. </param>
    /// <param name="value"> The amount of ether in wei being sent along with the transaction. </param>
    /// <param name="addressFrom"> The address of the wallet signing the transaction. </param>
    /// <param name="addressTo"> The address the transaction is being sent to. </param>
    /// <param name="data"> The data sent along with the transaction. </param>
    /// <param name="path"> The path of the wallet to sign the transaction with. </param>
    /// <param name="displayInput"> The display input that goes along with the transaction request. </param>
    void SignTransaction<T>(Action<TransactionSignedUnityRequest> onTransactionSigned,
                            BigInteger gasLimit,
                            BigInteger gasPrice,
                            BigInteger value,
                            string addressFrom,
                            string addressTo,
                            string data,
                            string path,
                            params object[] displayInput) where T : ConfirmTransactionPopupBase<T>;
}