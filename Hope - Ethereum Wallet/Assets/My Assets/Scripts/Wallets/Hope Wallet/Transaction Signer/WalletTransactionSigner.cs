using Nethereum.HdWallet;
using Nethereum.JsonRpc.UnityClient;
using System;

/// <summary>
/// Class used for signing transactions before sending them.
/// </summary>
public sealed class WalletTransactionSigner
{
    private readonly MemoryEncryptor passwordEncryptor;
    private readonly WalletDecryptor walletDecryptor;
    private readonly EthereumNetwork ethereumNetwork;

    /// <summary>
    /// Initializes the <see cref="WalletTransactionSigner"/> by assigning all references.
    /// </summary>
    /// <param name="playerPrefPassword"> The <see cref="PlayerPrefPasswordDerivation"/> instance to assign to the <see cref="WalletDecryptor"/>. </param>
    /// <param name="dynamicDataCache"> The active <see cref="DynamicDataCache"/> to assign to the <see cref="WalletDecryptor"/>. </param>
    /// <param name="ethereumNetwork"> The active <see cref="EthereumNetwork"/>. </param>
    /// <param name="passwordEncryptor"> The <see cref="MemoryEncryptor"/> instance used to encrypt the password. </param>
    /// <param name="walletSettings"> The settings for the <see cref="HopeWallet"/>. </param>
    public WalletTransactionSigner(
        PlayerPrefPasswordDerivation playerPrefPassword,
        DynamicDataCache dynamicDataCache,
        EthereumNetwork ethereumNetwork,
        MemoryEncryptor passwordEncryptor,
        HopeWalletInfoManager.Settings walletSettings)
    {
        this.ethereumNetwork = ethereumNetwork;
        this.passwordEncryptor = passwordEncryptor;

        walletDecryptor = new WalletDecryptor(playerPrefPassword, dynamicDataCache, walletSettings);
    }

    /// <summary>
    /// Signs a transaction and passes the result through an Action as a <see cref="TransactionSignedUnityRequest"/>.
    /// </summary>
    /// <param name="walletAddress"> The wallet address to sign the transaction with. </param>
    /// <param name="encryptedPasswordBytes"> The encrypted password to use to decrypt the wallet. </param>
    /// <param name="onRequestReceived"> Action called with the <see cref="TransactionSignedUnityRequest"/> once the transaction was signed. </param>
    [SecureCallEnd]
    public void SignTransaction(string walletAddress, byte[] encryptedPasswordBytes, Action<TransactionSignedUnityRequest> onRequestReceived)
    {
        var plainTextBytes = passwordEncryptor.Decrypt(encryptedPasswordBytes);
        walletDecryptor.DecryptWallet(plainTextBytes, (seed, path) =>
        {
            TransactionSignedUnityRequest request = new TransactionSignedUnityRequest(new Wallet(seed, path).GetAccount(walletAddress), ethereumNetwork.NetworkUrl);
            MainThreadExecutor.QueueAction(() => onRequestReceived?.Invoke(request));
            ClearData(seed, plainTextBytes);
        });
    }

    /// <summary>
    /// Clears the <see langword="byte"/>[] data of the wallet seed and collects any garbage.
    /// </summary>
    /// <param name="seed"> The <see langword="byte"/>[] seed used to sign the transaction. </param>
    private void ClearData(params byte[][] seed)
    {
        foreach (var byteData in seed)
            byteData.ClearBytes();

        GC.Collect();
    }
}