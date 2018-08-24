﻿using Hope.Utils.Ethereum;
using Nethereum.Hex.HexTypes;
using System;
using UnityEngine;

/// <summary>
/// Class which contains methods for interacting with ERC20 tokens.
/// </summary>
public sealed partial class ERC20 : Token
{
    /// <summary>
    /// Initializes the ERC20 token with all required values.
    /// </summary>
    /// <param name="contractAddress"> The contract address of this ERC20 token. </param>
    /// <param name="name"> The name of this ERC20 token. </param>
    /// <param name="symbol"> The symbol of this ERC20 token. </param>
    /// <param name="decimals"> The decimal count of this ERC20 token. </param>
    public ERC20(string contractAddress, string name, string symbol, int decimals) : base(contractAddress, name, symbol, decimals)
    {
    }

    /// <summary>
    /// Gets the token balance of an address.
    /// </summary>
    /// <param name="address"> The address to check the balance of. </param>
    /// <param name="onBalanceReceived"> Callback action which should pass in the received balance of Gold tokens on the address. </param>
    public void BalanceOf(string address, Action<dynamic> onBalanceReceived)
    {
        SimpleContractQueries.QueryUInt256Output<Queries.BalanceOf>(ContractAddress,
                                                                    address,
                                                                    balance => onBalanceReceived?.Invoke(SolidityUtils.ConvertFromUInt(balance, Decimals.Value)),
                                                                    address);
    }

    /// <summary>
    /// Gets the total supply of this ERC20 token contract.
    /// </summary>
    /// <param name="onSupplyReceived"> Callback action which should pass in the total supply of this token. </param>
    public void TotalSupply(Action<dynamic> onSupplyReceived)
    {
        SimpleContractQueries.QueryUInt256Output<Queries.TotalSupply>(ContractAddress,
                                                                      null,
                                                                      supply => onSupplyReceived?.Invoke(SolidityUtils.ConvertFromUInt(supply, Decimals.Value)));
    }

    /// <summary>
    /// Transfers a certain number of tokens of this contract from a wallet to another address.
    /// </summary>
    /// <param name="userWalletManager"> The wallet to transfer the tokens from. </param>
    /// <param name="gasLimit"> The gas limit to use when sending the tokens. </param>
    /// <param name="gasPrice"> The gas price to use when sending the tokens. </param>
    /// <param name="address"> The address to transfer the tokens to. </param>
    /// <param name="amount"> The amount of tokens to transfer. </param>
    public void Transfer(UserWalletManager userWalletManager, HexBigInteger gasLimit, HexBigInteger gasPrice, string address, decimal amount)
    {
        userWalletManager.SignTransaction<ConfirmTransactionPopup>(request =>
        {
            var promise = ContractUtils.SendContractMessage<Messages.Transfer>(ContractAddress,
                                                                               request,
                                                                               gasPrice,
                                                                               gasLimit,
                                                                               address,
                                                                               SolidityUtils.ConvertToUInt(amount, Decimals.Value));

            promise.OnSuccess(_ => Debug.Log("Successfully sent " + amount + " " + Symbol + " to address " + address));
            promise.OnError(_ => Debug.Log("Transaction failed! " + amount + " " + Symbol + " was not sent."));
        }, gasLimit, gasPrice, address, ContractAddress, amount, Symbol);
    }
}