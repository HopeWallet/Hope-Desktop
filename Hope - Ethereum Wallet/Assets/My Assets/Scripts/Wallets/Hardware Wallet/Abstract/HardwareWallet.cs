using System;
using System.Numerics;
using Nethereum.JsonRpc.UnityClient;

public abstract class HardwareWallet : IWallet
{
    public event Action OnWalletLoadSuccessful;
    public event Action OnWalletLoadUnsuccessful;

    protected readonly string[] addresses = new string[50];

    protected readonly EthereumNetworkManager.Settings ethereumNetworkSettings;

    protected HardwareWallet(EthereumNetworkManager.Settings ethereumNetworkSettings)
    {
        this.ethereumNetworkSettings = ethereumNetworkSettings;
    }

    public abstract void InitializeAddresses();

    public string GetAddress(int addressIndex) => addresses[addressIndex];

    public abstract void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        BigInteger gasLimit,
        BigInteger gasPrice,
        string signerAddress,
        params object[] transactionInput) where T : ConfirmTransactionPopupBase<T>;
}
