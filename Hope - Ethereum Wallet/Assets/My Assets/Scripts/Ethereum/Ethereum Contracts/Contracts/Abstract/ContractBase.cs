using Nethereum.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/// <summary>
/// Class used as a base for all ethereum contracts to interact with.
/// </summary>
public abstract class ContractBase
{

    protected Dictionary<string, Function> functions;
    protected Contract contract;

    /// <summary>
    /// The address of this contract.
    /// </summary>
    public string ContractAddress { get; private set; }

    /// <summary>
    /// The abi of this contract.
    /// </summary>
    public string ContractABI { get; private set; }

    /// <summary>
    /// Gets a function of this contract given the name.
    /// </summary>
    /// <param name="functionName"> The name of the function to get. </param>
    /// <returns> The function if it exists. </returns>
    public Function this[string functionName] => functions.ContainsKey(functionName) ? functions[functionName] : null;

    /// <summary>
    /// The array of function names to initialize for this contract.
    /// </summary>
    protected abstract string[] FunctionNames { get; }

    /// <summary>
    /// Initializes the contract with the address and abi.
    /// </summary>
    /// <param name="contractAddress"> The address of the contract. </param>
    /// <param name="abi"> The abi of the contract. </param>
    /// <param name="onContractInitialized"> Action to call when the contract has been initialized. </param>
    public ContractBase(string contractAddress, string abi, Action<ContractBase, string> onContractInitialized)
    {
        StartContractInitialization(contractAddress, abi, onContractInitialized);
    }

    /// <summary>
    /// Initializes the contract with the address and abi but without any callback when it is finished.
    /// </summary>
    /// <param name="contractAddress"> The address of the contract. </param>
    /// <param name="abi"> The abi of the contract. </param>
    public ContractBase(string contractAddress, string abi)
    {
        StartContractInitialization(contractAddress, abi);
    }

    /// <summary>
    /// Initializes the contract with the address and abi.
    /// </summary>
    /// <param name="contractAddress"> The address of the contract. </param>
    /// <param name="abi"> The abi of the contract. </param>
    /// <param name="onContractInitialized"> Action to call when the contract has been initialized. </param>
    private void StartContractInitialization(string contractAddress, string abi, Action<ContractBase, string> onContractInitialized = null)
    {
        if (abi == null)
            GetContractABI(contractAddress, onContractInitialized);
        else
            Initialize(contractAddress, abi, onContractInitialized);
    }

    /// <summary>
    /// Adds an array of function names to the dictionary.
    /// </summary>
    /// <param name="functionNames"> The array of functions names to add. </param>
    protected void AddFunctions(params string[] functionNames) => functionNames.ForEach(name => functions.Add(name, contract.GetFunction(name)));

    /// <summary>
    /// Retrieves the abi for this contract from the WebClientService.
    /// </summary>
    /// <param name="contractAddress"> The contract address to get the abi for. </param>
    /// <param name="onContractInitialized"> Action to call once the contract gets intialized. </param>
    private void GetContractABI(string contractAddress, Action<ContractBase, string> onContractInitialized = null)
    {
        WebClientUtils.GetContractABI(EthereumNetworkManager.Instance.CurrentNetwork.Api.GetContractAbiUrl(contractAddress), (abi)
                   => TryContractSetup(contractAddress, abi, onContractInitialized));


        //WebClientUtils.DownloadString(EthereumNetworkManager.Instance.CurrentNetwork.Api.GetContractAbiUrl(contractAddress), json
        //           => TryContractSetup(contractAddress, JsonConvert.DeserializeAnonymousType(, onContractInitialized));
    }

    /// <summary>
    /// Attempts to setup the contract. Throws an error if the contract was invalid, or something else went wrong in the process.
    /// </summary>
    /// <param name="contractAddress"> The address of the contract. </param>
    /// <param name="abi"> The abi of the contract. </param>
    /// <param name="onContractInitialized"> Action to call when the contract has been initialized. </param>
    private void TryContractSetup(string contractAddress, string abi, Action<ContractBase, string> onContractInitialized)
    {
        try
        {
            Initialize(contractAddress, abi, onContractInitialized);
        }
        catch
        {
            ExceptionManager.DisplayException(new Exception("Failed to create contract, please use a valid contract address."));
        }
    }

    /// <summary>
    /// Method which groups together the two intialization processes together.
    /// Initializes the contract by creating the Contract object, and by intializing the contract through the inherited InitializeContract method.
    /// </summary>
    /// <param name="contractAddress"> The address of the contract to initialize. </param>
    /// <param name="abi"> The abi for the contract. </param>
    /// <param name="onContractInitialized"> Called when the contract has fully finished initializing. </param>
    private void Initialize(string contractAddress, string abi, Action<ContractBase, string> onContractInitialized = null)
    {
        InitializeBaseContract(contractAddress, abi);
        InitializeExtra(onContractInitialized);
    }

    /// <summary>
    /// Initializes the base contract with the address and abi.
    /// </summary>
    /// <param name="contractAddress"> The contract address to use for initialization. </param>
    /// <param name="abi"> The abi to use when initializing the contract. </param>
    private void InitializeBaseContract(string contractAddress, string abi)
    {
        functions = new Dictionary<string, Function>();
        contract = new Contract(null, abi, contractAddress);

        ContractAddress = contractAddress;
        ContractABI = abi;

        AddFunctions(FunctionNames);
    }

    /// <summary>
    /// Method to override which initializes all needed contract functions.
    /// </summary>
    /// <param name="onContractInitialized"> Action to call when the contract has been initialized. </param>
    protected virtual void InitializeExtra(Action<ContractBase, string> onContractInitialized) { }

}
