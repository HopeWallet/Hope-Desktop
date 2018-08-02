using Hope.Utils.EthereumUtils;
using System;
using System.Numerics;
using System.Linq;

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

    public float UpdateInterval => 10f;

    public dynamic DUBIBalance { get; private set; }

    public dynamic PRPSBalance { get; private set; }

    public BigInteger GasLimit { get; private set; }

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

    public void PeriodicUpdate()
    {
        GetDUBIBalance(dubiContract.ContractAddress, userWalletManager.WalletAddress);
        GetPRPSBalance(prpsContract.ContractAddress, userWalletManager.WalletAddress);
    }

    private void CheckIfPRPSAdded(TradableAsset tradableAsset)
    {
        if (tradableAsset.AssetAddress.EqualsIgnoreCase(prpsContract.ContractAddress))
            periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    private void CheckIfPRPSRemoved(TradableAsset tradableAsset)
    {
        if (tradableAsset.AssetAddress.EqualsIgnoreCase(prpsContract.ContractAddress))
            periodicUpdateManager.RemovePeriodicUpdater(this);
    }

    private void GetDUBIBalance(string contractAddress, string walletAddress)
    {
        SimpleContractQueries.QueryUInt256Output<ERC20.Queries.BalanceOf>(contractAddress, walletAddress, balance => DUBIBalance = SolidityUtils.ConvertFromUInt(balance.Value, 18), walletAddress);
    }

    private void GetPRPSBalance(string contractAddress, string walletAddress)
    {
        SimpleContractQueries.QueryUInt256Output<ERC20.Queries.BalanceOf>(contractAddress, walletAddress, GetLockableGasLimit, walletAddress);
    }

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