using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System;
using UnityEngine;

/// <summary>
/// Class which inherits from TokenContract and acts as a mintable ERC20 token.
/// </summary>
public class MintableTokenContract : TokenContract
{

    /// <summary>
    /// Initializes the TokenContract with the address and abi.
    /// </summary>
    /// <param name="contractAddress"> The address of the contract. </param>
    /// <param name="abi"> The abi of the contract. </param>
    /// <param name="onContractInitialized"> Action to call when the contract has been fully initialized. </param>
    public MintableTokenContract(string contractAddress, string abi, Action<ContractBase, string> onContractInitialized = null) : base(contractAddress, abi, onContractInitialized) { }

    /// <summary>
    /// Initializes the MintableTokenContract by calling the base contract initialization and setting up the mint function.
    /// </summary>
    /// <param name="onContractInitialized"> Action to call when the contract has been fully initialized. </param>
    protected override void InitializeExtra(Action<ContractBase, string> onContractInitialized)
    {
        AddFunction("mint");
        base.InitializeExtra(onContractInitialized);
    }

    /// <summary>
    /// Mints a certain number of tokens to a given address.
    /// </summary>
    /// <param name="userWallet"> The wallet to send the mint transaction from. </param>
    /// <param name="gasLimit"> The gas limit to execute the mint tokens function with. </param>
    /// <param name="gasPrice"> The gas price to execute the mint tokens function with. </param>
    /// <param name="address"> The address to send the minted tokens to. </param>
    /// <param name="amount"> The amount of tokens to mint. </param>
    //public void Mint(UserWallet userWallet, HexBigInteger gasLimit, HexBigInteger gasPrice, string address, int amount) 
    //    => this.ExecuteContractFunction(this["mint"], userWallet, gasLimit, gasPrice, () => Debug.Log("Successfully minted " + amount + " " + TokenSymbol + " to address " + address), address, amount);

}
