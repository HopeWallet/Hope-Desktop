using Nethereum.Hex.HexTypes;
using System;
using System.Numerics;
using UnityEngine;

/// <summary>
/// Class which represents an asset that can be traded to and from other ethereum addresses.
/// </summary>
public abstract class TradableAsset
{
    public event Action<dynamic> OnAssetBalanceChanged;

    protected readonly TradableAssetImageManager tradableAssetImageManager;
    protected readonly UserWalletManager userWalletManager;

    /// <summary>
    /// The ethereum address of this asset. Using address(0) for ether.
    /// </summary>
    public string AssetAddress { get; private set; }

    /// <summary>
    /// The symbol of this asset.
    /// </summary>
    public string AssetSymbol { get; private set; }

    /// <summary>
    /// The name of this asset.
    /// </summary>
    public string AssetName { get; private set; }

    /// <summary>
    /// The number of decimal places for this asset.
    /// </summary>
    public int AssetDecimals { get; private set; }

    /// <summary>
    /// The balance of this asset type existing in the user's wallet.
    /// </summary>
    public dynamic AssetBalance { get; private set; }

    /// <summary>
    /// The image of the tradable asset.
    /// </summary>
    public Sprite AssetImage { get; private set; }

    /// <summary>
    /// Initializes the TradableAsset by setting up the address and symbol.
    /// </summary>
    /// <param name="address"> The address of this asset. </param>
    /// <param name="symbol"> The symbol of this asset. </param>
    /// <param name="name"> The name of this asset. </param>
    /// <param name="decimals"> The number of decimal places of the asset. </param>
    /// <param name="tradableAssetImageManager"> The active TradableAssetImageManager. </param>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    protected TradableAsset(
        string address,
        string symbol,
        string name,
        int decimals,
        TradableAssetImageManager tradableAssetImageManager,
        UserWalletManager userWalletManager)
    {
        AssetAddress = address.ToLower();
        AssetSymbol = symbol;
        AssetName = name;
        AssetDecimals = decimals;
        this.tradableAssetImageManager = tradableAssetImageManager;
        this.userWalletManager = userWalletManager;
    }

    /// <summary>
    /// Updates this asset balance belonging in the user's wallet. 
    /// </summary>
    /// <param name="onBalanceReceived"> Called when the balance has successfully been retrieved. </param>
    public void UpdateBalance(Action onBalanceReceived = null)
    {
        userWalletManager.GetAssetBalance(this, balance =>
        {
            if (balance != AssetBalance)
                OnAssetBalanceChanged?.Invoke(balance);
            AssetBalance = balance;
            onBalanceReceived?.Invoke();
        });
    }

    /// <summary>
    /// Gets the image of this tradable asset and the current balance of this asset in the wallet.
    /// </summary>
    /// <param name="onInfoInitialized"> Action to call once the info has been initialized. </param>
    protected void InitializeBasicInfo(Action<TradableAsset> onInfoInitialized)
    {
        if (AssetSymbol == null)
        {
            onInfoInitialized(this);
            return;
        }

        tradableAssetImageManager.LoadImage(AssetSymbol, image =>
        {
            AssetImage = image;
            UpdateBalance(() => onInfoInitialized(this));
        });
    }

    /// <summary>
    /// Gets the balance of this asset currently in a UserWallet.
    /// </summary>
    /// <param name="userWallet"> The walletto get the balance for. </param>
    /// <param name="onBalanceReceived"> Callback to execute once the balnce has been received, with the amount as a param. </param>
    public abstract void GetBalance(UserWallet userWallet, Action<dynamic> onBalanceReceived);

    /// <summary>
    /// Transfers a certain number of this asset from a UserWallet to a specified ethereum address.
    /// </summary>
    /// <param name="userWallet"> The UserWallet to transfer the asset from. </param>
    /// <param name="gasLimit"> The gas limit to use for this transfer transaction. </param>
    /// <param name="gasPrice"> The gas price to use for this transfer transaction. </param>
    /// <param name="address"> The address to send the asset to. </param>
    /// <param name="amount"> The amount of this asset to send to the address. </param>
    public abstract void Transfer(UserWallet userWallet, HexBigInteger gasLimit, HexBigInteger gasPrice, string address, decimal amount);

    /// <summary>
    /// Gets the gas limit of transferring this asset from the user's address to another.
    /// </summary>
    /// <param name="receivingAddress"> The address to receive the asset. </param>
    /// <param name="amount"> The amount of the asset to send. </param>
    /// <param name="onLimitReceived"> The action to call once the gas limit has been received. </param>
    public abstract void GetTransferGasLimit(string receivingAddress, dynamic amount, Action<BigInteger> onLimitReceived);

}
