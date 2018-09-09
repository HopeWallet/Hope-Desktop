using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Hope.Utils.Ethereum;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RLP;
using Nethereum.Signer;

public abstract class HardwareWallet : IWallet
{
    public event Action OnWalletLoadSuccessful;
    public event Action OnWalletLoadUnsuccessful;

    protected readonly string[] addresses = new string[50];

    protected readonly EthereumNetworkManager ethereumNetworkManager;
    protected readonly EthereumNetworkManager.Settings ethereumNetworkSettings;
    protected readonly PopupManager popupManager;

    protected HardwareWallet(
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings,
        PopupManager popupManager)
    {
        this.ethereumNetworkManager = ethereumNetworkManager;
        this.ethereumNetworkSettings = ethereumNetworkSettings;
        this.popupManager = popupManager;
    }

    protected void WalletLoadSuccessful() => OnWalletLoadSuccessful?.Invoke();

    protected void WalletLoadUnsuccessful() => OnWalletLoadUnsuccessful?.Invoke();

    public string GetAddress(int addressIndex) => addresses[addressIndex];

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
    /// <param name="displayInput"> The display input that goes along with the transaction request. </param>
    public void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        BigInteger gasLimit,
        BigInteger gasPrice,
        BigInteger value,
        string addressFrom,
        string addressTo,
        string data,
        params object[] displayInput) where T : ConfirmTransactionPopupBase<T>
    {
        TransactionUtils.GetAddressTransactionCount(addressFrom).OnSuccess(txCount =>
        {
            byte[] nonceBytes = txCount.ToBytesForRLPEncoding();
            byte[] gasPriceBytes = gasPrice.ToBytesForRLPEncoding();
            byte[] gasLimitBytes = gasLimit.ToBytesForRLPEncoding();
            byte[] addressBytes = addressTo.HexToByteArray();
            byte[] valueBytes = value.ToBytesForRLPEncoding();
            byte[] dataBytes = data.HexToByteArray();

            uint index = (uint)addresses.ToList().IndexOf(addresses.Where(address => address.EqualsIgnoreCase(addressFrom)).First());

            SignTransaction(
                onTransactionSigned,
                new Transaction(nonceBytes, gasPriceBytes, gasLimitBytes, addressBytes, valueBytes, dataBytes, new byte[] { 0 }, new byte[] { 0 }, (byte)ethereumNetworkSettings.networkType),
                index);
        });
    }

    public abstract void InitializeAddresses();

    protected abstract void SignTransaction(Action<TransactionSignedUnityRequest> onTransactionSigned, Transaction transaction, uint addressIndex);
}