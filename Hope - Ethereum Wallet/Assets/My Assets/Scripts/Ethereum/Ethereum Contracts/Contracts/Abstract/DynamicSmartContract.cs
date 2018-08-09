/// <summary>
/// Class which represents a smart contract that is created dynamically on the fly.
/// </summary>
public abstract class DynamicSmartContract
{
    /// <summary>
    /// The address of this contract.
    /// </summary>
    public string ContractAddress { get; }

    /// <summary>
    /// Initializes the <see cref="DynamicSmartContract"/> with the contract address.
    /// </summary>
    /// <param name="contractAddress"> The address of this contract. </param>
    protected DynamicSmartContract(string contractAddress)
    {
        ContractAddress = contractAddress;
    }
}