using Hope.Utils.Ethereum;
using Nethereum.Hex.HexTypes;
using System;
using System.Numerics;

/// <summary>
/// Class which represents a the TradableAsset ether.
/// </summary>
public sealed class EtherAsset : TradableAsset
{
    public const string ETHER_ADDRESS = "0x0000000000000000000000000000000000000000";

    /// <summary>
    /// Initializes this TradableAsset with the address of 0x0 and the symbol ETH.
    /// </summary>
    /// <param name="onAssetCreated"> Callback to execute once this asset has been created and the current balance received. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    public EtherAsset(
        Action<TradableAsset> onAssetCreated,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager) : base(ETHER_ADDRESS, "ETH", "Ether", 18, tradableAssetImageManager, userWalletManager)
    {
        InitializeBasicInfo(onAssetCreated);
    }

    /// <summary>
    /// Gets the ether balance of a UserWallet using the WalletExtensions method for getting ether balances.
    /// </summary>
    /// <param name="userWalletManager"> The wallet to get the current amount of ether for. </param>
    /// <param name="onBalanceReceived"> Callback to execute once the balance has been received, with the amount as a parameter. </param>
    public override void GetBalance(UserWalletManager userWalletManager, Action<dynamic> onBalanceReceived)
    {
        EthUtils.GetEtherBalance(userWalletManager.WalletAddress).OnSuccess(onBalanceReceived);
    }

    /// <summary>
    /// Transfers a specified amount of ether from the input UserWallet to a specified address.
    /// </summary>
    /// <param name="userWalletManager"> The wallet to send the ether from. </param>
    /// <param name="gasLimit"> The gas limit to use for this ether send transaction. </param>
    /// <param name="gasPrice"> The gas price to use for this ether send transaction. </param>
    /// <param name="address"> The address to send the ether to. </param>
    /// <param name="amount"> The amount of ether to send. </param>
    public override void Transfer(UserWalletManager userWalletManager, HexBigInteger gasLimit, HexBigInteger gasPrice, string address, decimal amount)
    {
        userWalletManager.SignTransaction<ConfirmTransactionPopup>(
                              request => EthUtils.SendEther(request, gasLimit, gasPrice, userWalletManager.WalletAddress, address, amount).OnSuccess(UnityEngine.Debug.Log),
                              gasLimit,
                              gasPrice,
                              address,
                              AssetAddress,
                              amount,
                              "ETH");
    }

    /// <summary>
    /// Estimates the gas limit of transfering eth from one address to another.
    /// </summary>
    /// <param name="receivingAddress"> The address to receive the ether. </param>
    /// <param name="amount"> The amount of ether that is requesting to be sent. </param>
    /// <param name="onLimitReceived"> The action to execute when the gas limit has been received. </param>
    public override void GetTransferGasLimit(string receivingAddress, dynamic amount, Action<BigInteger> onLimitReceived)
    {
        GasUtils.EstimateEthGasLimit(receivingAddress, SolidityUtils.ConvertToUInt(amount, 18), onLimitReceived);
    }
}