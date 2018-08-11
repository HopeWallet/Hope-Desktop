using Hope.Utils.Ethereum;
using System;
using System.Numerics;

/// <summary>
/// Class which manages the values for locking purpose.
/// </summary>
public sealed class LockPRPSManager : IPeriodicUpdater
{
    public event Action OnAmountsUpdated;

    private readonly PRPS prpsContract;
    private readonly DUBI dubiContract;
    private readonly Hodler hodlerContract;

    private readonly UserWalletManager userWalletManager;
    private readonly PeriodicUpdateManager periodicUpdateManager;

    private readonly BigInteger estimationId = new BigInteger(9223372036854775807); // Max long value
    private readonly BigInteger estimationMonths = new BigInteger(12);

    /// <summary>
    /// How often the dubi and purpose balances should be updated.
    /// </summary>
    public float UpdateInterval => 10f;

    /// <summary>
    /// The active DUBI balance of the wallet.
    /// </summary>
    public dynamic DUBIBalance { get; private set; }

    /// <summary>
    /// The active PRPS balance of the wallet.
    /// </summary>
    public dynamic PRPSBalance { get; private set; }

    /// <summary>
    /// The gas limit to use to lock purpose.
    /// </summary>
    public BigInteger GasLimit { get; private set; }

    /// <summary>
    /// Initializes the LockPRPSManager with all dependencies.
    /// </summary>
    /// <param name="userWalletManager"> The active UserWalletManager. </param>
    /// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
    /// <param name="prpsContract"> The active PRPS smart contract. </param>
    /// <param name="dubiContract"> The active DUBI smart contract. </param>
    /// <param name="hodlerContract"> The active Hodler smart contract. </param>
    public LockPRPSManager(
        UserWalletManager userWalletManager,
        PeriodicUpdateManager periodicUpdateManager,
        PRPS prpsContract,
        DUBI dubiContract,
        Hodler hodlerContract)
    {
        this.prpsContract = prpsContract;
        this.dubiContract = dubiContract;
        this.hodlerContract = hodlerContract;
        this.userWalletManager = userWalletManager;
        this.periodicUpdateManager = periodicUpdateManager;

        TradableAssetManager.OnTradableAssetAdded += CheckIfPRPSAdded;
        TradableAssetManager.OnTradableAssetRemoved += CheckIfPRPSRemoved;
    }

    /// <summary>
    /// Updates the dubi and purpose balances.
    /// </summary>
    public void PeriodicUpdate()
    {
        GetDUBIBalance(dubiContract.ContractAddress, userWalletManager.WalletAddress);
        GetPRPSBalance(prpsContract.ContractAddress, userWalletManager.WalletAddress);
    }

    /// <summary>
    /// Checks if purpose was added as the most recent TradableAsset, and starts to update the values for the LockPRPSManager if so.
    /// </summary>
    /// <param name="tradableAsset"> The TradableAsset which was just added to the TradableAssetManager. </param>
    private void CheckIfPRPSAdded(TradableAsset tradableAsset)
    {
        if (tradableAsset.AssetAddress.EqualsIgnoreCase(prpsContract.ContractAddress))
            periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    /// <summary>
    /// Checks if purpose was removed as the most recently removed TradableAsset, and stops updating the LockPRPSManager if so.
    /// </summary>
    /// <param name="tradableAsset"> The most recently removed TradableAsset from the TradableAssetManager. </param>
    private void CheckIfPRPSRemoved(TradableAsset tradableAsset)
    {
        if (tradableAsset.AssetAddress.EqualsIgnoreCase(prpsContract.ContractAddress))
            periodicUpdateManager.RemovePeriodicUpdater(this);
    }

    /// <summary>
    /// Gets the most recent DUBI balance.
    /// </summary>
    /// <param name="contractAddress"> The contract address for DUBI. </param>
    /// <param name="walletAddress"> The current wallet address. </param>
    private void GetDUBIBalance(string contractAddress, string walletAddress)
    {
        SimpleContractQueries.QueryUInt256Output<ERC20.Queries.BalanceOf>(contractAddress, walletAddress, balance => DUBIBalance = SolidityUtils.ConvertFromUInt(balance.Value, 18), walletAddress);
    }

    /// <summary>
    /// Gets the most recent PRPS balance.
    /// </summary>
    /// <param name="contractAddress"> The contract address for PRPS. </param>
    /// <param name="walletAddress"> The current wallet address. </param>
    private void GetPRPSBalance(string contractAddress, string walletAddress)
    {
        SimpleContractQueries.QueryUInt256Output<ERC20.Queries.BalanceOf>(contractAddress, walletAddress, GetLockableGasLimit, walletAddress);
    }

    /// <summary>
    /// Gets the gas limit which would be used to lock the curent purpose balance.
    /// </summary>
    /// <param name="prpsBalance"> The output received from the BalanceOf function of the PRPS contract. </param>
    private void GetLockableGasLimit(SimpleOutputs.UInt256 prpsBalance)
    {
        PRPSBalance = SolidityUtils.ConvertFromUInt(prpsBalance.Value, 18);

        if (prpsBalance.Value <= 0)
            return;

        object[] funcParams = new object[] { estimationId, prpsBalance.Value, estimationMonths };
        GasUtils.EstimateGasLimit<Hodler.Messages.Hodl>(hodlerContract.ContractAddress,
                                                        userWalletManager.WalletAddress,
                                                        limit => GasLimit = limit,
                                                        funcParams);

        OnAmountsUpdated?.Invoke();
    }
}