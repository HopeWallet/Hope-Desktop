using Hope.Utils.Ethereum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public sealed class LockedPRPSManager : IPeriodicUpdater, IDisposable
{
    public event Action OnLockedPRPSUpdated;

    private readonly List<Hodler.Output.Item> lockedItems = new List<Hodler.Output.Item>();

    private readonly UserWalletManager userWalletManager;
    private readonly PeriodicUpdateManager periodicUpdateManager;
    private readonly Hodler hodlerContract;
    private readonly PRPS prpsContract;
    private readonly EtherscanApiService apiService;

    private int updateCount;
    private int updateCounter;

    public float UpdateInterval => 10f;

    public List<Hodler.Output.Item> UnfulfilledItems => lockedItems.Where(item => !item.Fulfilled)?.ToList();

    public List<Hodler.Output.Item> UnlockableItems => lockedItems.Where(item => item.Unlockable)?.ToList();

    public List<BigInteger> UsedIds { get; } = new List<BigInteger>();

    public LockedPRPSManager(
        DisposableComponentManager disposableComponentManager,
        UserWalletManager userWalletManager,
        Hodler hodlerContract,
        PRPS prpsContract,
        PeriodicUpdateManager periodicUpdateManager,
        EtherscanApiService apiService)
    {
        this.userWalletManager = userWalletManager;
        this.periodicUpdateManager = periodicUpdateManager;
        this.hodlerContract = hodlerContract;
        this.prpsContract = prpsContract;
        this.apiService = apiService;

        disposableComponentManager.AddDisposable(this);

        TradableAssetManager.OnTradableAssetAdded += CheckIfPRPSAdded;
        TradableAssetManager.OnTradableAssetRemoved += CheckIfPRPSRemoved;
    }

    /// <summary>
    /// Disposes of the current locked items and stops updating.
    /// </summary>
    public void Dispose()
    {
        lockedItems.Clear();
        UsedIds.Clear();
    }

    /// <summary>
    /// Searches for any new locked items, or any locked items that are now unlocked.
    /// </summary>
    public void PeriodicUpdate()
    {
        StartNewItemSearch();
    }

    /// <summary>
    /// Clears the list of items.
    /// </summary>
    public void ClearList()
    {
        lockedItems.Clear();
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

    /// <summary>
    /// Searches for any token transfers from the user's address to the hodl contract.
    /// </summary>
    private void StartNewItemSearch()
    {
        apiService.SendTokenTransfersFromAndToAddressRequest(userWalletManager.GetWalletAddress(), hodlerContract.ContractAddress, prpsContract.ContractAddress)
                  .OnSuccess(ProcessTxList);
    }

    /// <summary>
    /// Processes the list of purpose transactions sent out to the hodl contract.
    /// </summary>
    /// <param name="txList"> The list of transactions. </param>
    private void ProcessTxList(string txList)
    {
        var transactionsJson = JsonUtils.Deserialize<EtherscanAPIJson<TokenTransactionJson>>(txList);
        if (transactionsJson == null)
            return;

        updateCount = transactionsJson.result.Length;
        updateCounter = 0;

        transactionsJson.result.Reverse().ForEach(GetTransactionInputData);
    }

    /// <summary>
    /// Checks the details of each purpose transaction sent to the hodl contract.
    /// </summary>
    /// <param name="tokenTransactionJson"> The json of the token transaction. </param>
    private void GetTransactionInputData(TokenTransactionJson tokenTransactionJson)
    {
        TransactionUtils.GetTransactionDetails(tokenTransactionJson.transactionHash)
                        .OnSuccess(tx => GetItemFromHodlerContract(SolidityUtils.ExtractFunctionParameters(tx.Input), tokenTransactionJson.timeStamp.ConvertFromHex()));
    }

    private void GetItemFromHodlerContract(string[] inputData, BigInteger lockedTimeStamp)
    {
        hodlerContract.GetItem(userWalletManager.GetWalletAddress(), inputData[1].ConvertFromHex(), item => UpdateItemList(item, lockedTimeStamp));
    }

    private void UpdateItemList(Hodler.Output.Item item, BigInteger lockedTimeStamp)
    {
        AddNewItem(item, lockedTimeStamp);
        CheckItemUpdateCounter();
    }

    private void AddNewItem(Hodler.Output.Item item, BigInteger lockedTimeStamp)
    {
        if (item.ReleaseTime == 0)
            return;

        if (!lockedItems.Select(lockedItem => lockedItem.LockedTimeStamp).Contains(lockedTimeStamp))
        {
            UpdateItemInfo(item, lockedTimeStamp);
            lockedItems.Add(item);
            lockedItems.Sort((i1, i2) => i1.LockedTimeStamp.CompareTo(i2.LockedTimeStamp));
            UsedIds.Add(item.Id);
        }
        else
        {
            var oldItem = lockedItems.Single(lockedItem => lockedItem.LockedTimeStamp == lockedTimeStamp);
            oldItem.Fulfilled = item.Fulfilled;
            UpdateItemInfo(oldItem, lockedTimeStamp);
        }
    }

    private void UpdateItemInfo(Hodler.Output.Item item, BigInteger lockedTimeStamp)
    {
        item.LockedTimeStamp = lockedTimeStamp;

        if (item.Unlockable && !item.UnlockableGasLimit.HasValue)
        {
            GasUtils.EstimateContractGasLimit<Hodler.Messages.Release>(hodlerContract.ContractAddress,
                                                                       userWalletManager.GetWalletAddress(),
                                                                       item.Id).OnSuccess(limit => item.UnlockableGasLimit = limit);
        }
    }

    private void CheckItemUpdateCounter()
    {
        if (++updateCounter == updateCount)
            OnLockedPRPSUpdated?.Invoke();
    }
}