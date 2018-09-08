using System;
using System.Numerics;
using Nethereum.JsonRpc.UnityClient;

public class LedgerWallet : IWallet
{
    public event Action OnWalletLoaded;

    public LedgerWallet()
    {
    }

    public string GetAddress(int addressIndex)
    {
        throw new NotImplementedException();
    }

    public void SignTransaction<T>(
        Action<TransactionSignedUnityRequest> onTransactionSigned,
        BigInteger gasLimit,
        BigInteger gasPrice,
        string signerAddress,
        params object[] transactionInput) where T : ConfirmTransactionPopupBase<T>
    {
        throw new NotImplementedException();
    }
}