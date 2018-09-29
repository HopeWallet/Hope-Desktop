using Hope.Utils.Ethereum;
using Hope.Utils.Promises;
using Nethereum.Contracts.Extensions;
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

    public ERC20(string contractAddress) : base(contractAddress)
    {

    }

    /// <summary>
    /// Gets the token balance of an address.
    /// </summary>
    /// <param name="address"> The address to check the balance of. </param>
    /// <param name="onBalanceReceived"> Callback action which should pass in the received balance of Gold tokens on the address. </param>
    public void BalanceOf(string address, Action<dynamic> onBalanceReceived)
    {
        SimpleContractQueries.QueryUInt256Output<Queries.BalanceOf>(ContractAddress, address, address)
                             .OnSuccess(balance => onBalanceReceived?.Invoke(SolidityUtils.ConvertFromUInt(balance, Decimals.Value)));
    }

    /// <summary>
    /// Gets the total supply of this ERC20 token contract.
    /// </summary>
    /// <param name="onSupplyReceived"> Callback action which should pass in the total supply of this token. </param>
    public void TotalSupply(Action<dynamic> onSupplyReceived)
    {
        SimpleContractQueries.QueryUInt256Output<Queries.TotalSupply>(ContractAddress, null)
                             .OnSuccess(supply => onSupplyReceived?.Invoke(SolidityUtils.ConvertFromUInt(supply, Decimals.Value)));
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
        var transactionInput = ContractFunction.CreateFunction<Messages.Transfer>(gasPrice, gasLimit, address, SolidityUtils.ConvertToUInt(amount, Decimals.Value)).CreateTransactionInput(ContractAddress);

        userWalletManager.SignTransaction<ConfirmTransactionPopup>(
            request => ContractUtils.SendContractMessage($"Sending {Symbol}", transactionInput, request),
            gasLimit,
            gasPrice,
            0,
            ContractAddress,
            transactionInput.Data,
            address,
            ContractAddress,
            amount,
            Symbol);
    }

    public override EthCallPromise<string> QueryName()
    {
        EthCallPromise<string> promise = new EthCallPromise<string>();
        SimpleContractQueries.QueryStringOutput<Queries.Name>(ContractAddress, null)
                             .OnSuccess(name => promise.Build(() => name?.Value))
                             .OnError(error => promise.Build(() => "error", () => error));

        return promise;
    }

    public override EthCallPromise<string> QuerySymbol()
    {
        EthCallPromise<string> promise = new EthCallPromise<string>();
        SimpleContractQueries.QueryStringOutput<Queries.Symbol>(ContractAddress, null)
                             .OnSuccess(symbol => promise.Build(() => symbol?.Value))
                             .OnError(error => promise.Build(() => "error", () => error));

        return promise;
    }

    public override EthCallPromise<int?> QueryDecimals()
    {
        EthCallPromise<int?> promise = new EthCallPromise<int?>();
        SimpleContractQueries.QueryUInt256Output<Queries.Decimals>(ContractAddress, null)
                             .OnSuccess(decimals => promise.Build(() => (int?)decimals?.Value))
                             .OnError(error => promise.Build(() => "error", () => error));

        return promise;
    }
}