using System;
using System.Numerics;
using Nethereum.JsonRpc.UnityClient;

public sealed class LedgerWallet : HardwareWallet
{
    public LedgerWallet(EthereumNetworkManager.Settings ethereumNetworkSettings) : base(ethereumNetworkSettings)
    {
    }

    public override void InitializeAddresses()
    {
    }

    public override void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        BigInteger gasLimit,
        BigInteger gasPrice,
        string signerAddress,
        params object[] transactionInput)
    {
    }
}