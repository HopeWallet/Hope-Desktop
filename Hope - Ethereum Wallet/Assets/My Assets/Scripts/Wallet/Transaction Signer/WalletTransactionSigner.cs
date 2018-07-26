using Nethereum.HdWallet;
using Nethereum.JsonRpc.UnityClient;
using System;

public sealed class WalletTransactionSigner
{
    private readonly EphemeralEncryption passwordEncryptor;

    private readonly WalletDecryptor walletDecryptor;

    private readonly DynamicDataCache dynamicDataCache;
    private readonly EthereumNetwork ethereumNetwork;

    public WalletTransactionSigner(
        PlayerPrefPassword playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        EthereumNetwork ethereumNetwork,
        EphemeralEncryption passwordEncryptor)
    {
        this.dynamicDataCache = dynamicDataCache;
        this.ethereumNetwork = ethereumNetwork;
        this.passwordEncryptor = passwordEncryptor;

        walletDecryptor = new WalletDecryptor(playerPrefPassword, dynamicDataCache);
    }

    [SecureCallEnd]
    public void SignTransaction(string walletAddress, string encryptedPassword, Action<TransactionSignedUnityRequest> onRequestReceived)
    {
        walletDecryptor.DecryptWallet(passwordEncryptor.Decrypt(encryptedPassword), seed =>
        {
            TransactionSignedUnityRequest request = new TransactionSignedUnityRequest(new Wallet(seed).GetAccount(walletAddress), ethereumNetwork.NetworkUrl);
            MainThreadExecutor.QueueAction(() => onRequestReceived?.Invoke(request));
            ClearData(seed);
        });
    }

    private void ClearData(byte[] seed)
    {
        seed.ClearBytes();
        GC.Collect();
    }
}