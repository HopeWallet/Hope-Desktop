using Hope.Utils.EthereumUtils;
using Nethereum.Hex.HexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed partial class ERC20 : DynamicSmartContract
{
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
        SimpleContractQueries.QueryUInt256Output<Queries.BalanceOf>(ContractAddress, address,
            balance => onBalanceReceived?.Invoke(SolidityUtils.ConvertFromUInt(balance, TokenDecimals)), address);
    }

    /// <summary>
    /// Gets the total supply of this ERC20 token contract.
    /// </summary>
    /// <param name="onSupplyReceived"> Callback action which should pass in the total supply of this token. </param>
    public void TotalSupply(Action<dynamic> onSupplyReceived)
    {
        SimpleContractQueries.QueryUInt256Output<Queries.TotalSupply>(ContractAddress, null, supply => onSupplyReceived?.Invoke(SolidityUtils.ConvertFromUInt(supply, TokenDecimals)));
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
            ContractUtils.SendContractMessage<Messages.Transfer>(ContractAddress,
                                                                        request,
                                                                        gasPrice,
                                                                        gasLimit,
                                                                        () => Debug.Log("Successfully sent " + amount + " " + TokenSymbol + " to address " + address), address,
                                                                        SolidityUtils.ConvertToUInt(amount, TokenDecimals));
        }, gasLimit, gasPrice, address, ContractAddress, amount, TokenSymbol);
    }

}