using Hope.Utils.EthereumUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public sealed class LockedPRPSManager : IPeriodicUpdater
{
    public event Action OnLockedPRPSUpdated;

    private readonly List<Hodler.Output.Item> lockedItems = new List<Hodler.Output.Item>();

    private readonly UserWalletManager userWalletManager;
    private readonly Hodler hodlerContract;
    private readonly PRPS prpsContract;
    private readonly EthereumNetwork ethereumNetwork;

    private int updateCount;
    private int updateCounter;

    public float UpdateInterval => 10f;

    public List<Hodler.Output.Item> UnfulfilledItems => lockedItems.Where(item => !item.Fulfilled)?.ToList();

    public List<Hodler.Output.Item> UnlockableItems => lockedItems.Where(item => item.Unlockable)?.ToList();

    public List<BigInteger> UsedIds { get; } = new List<BigInteger>();

    public LockedPRPSManager(
        UserWalletManager userWalletManager,
        Hodler hodlerContract,
        PRPS prpsContract,
        EthereumNetworkManager ethereumNetworkManager,
        PeriodicUpdateManager periodicUpdateManager)
    {
        this.userWalletManager = userWalletManager;
        this.hodlerContract = hodlerContract;
        this.prpsContract = prpsContract;
        ethereumNetwork = ethereumNetworkManager.CurrentNetwork;

        UserWallet.OnWalletLoadSuccessful += () => periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    /// <summary>
    /// Searches for any new locked items, or any locked items that are now unlocked.
    /// </summary>
    public void PeriodicUpdate() => StartNewItemSearch();

    /// <summary>
    /// Searches for any token transfers from the user's address to the hodl contract.
    /// </summary>
    private void StartNewItemSearch()
    {
        string apiUrl = ethereumNetwork.Api.GetTokenTransfersFromAndToUrl(prpsContract.ContractAddress, userWalletManager.WalletAddress, hodlerContract.ContractAddress);
        UnityWebUtils.DownloadString(apiUrl, ProcessTxList);
    }

    /// <summary>
    /// Processes the list of purpose transactions sent out to the hodl contract.
    /// </summary>
    /// <param name="txList"> The list of transactions. </param>
    private void ProcessTxList(string txList)
    {
        var transactionsJson = JsonUtils.GetJsonData<EtherscanAPIJson<TokenTransactionJson>>(txList);
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
        TransactionUtils.CheckTransactionDetails(tokenTransactionJson.transactionHash,
            tx => GetItemFromHodlerContract(SolidityUtils.ExtractFunctionParameters(tx.Input), tokenTransactionJson.timeStamp.ConvertFromHex()));
    }

    private void GetItemFromHodlerContract(string[] inputData, BigInteger lockedTimeStamp)
    {
        hodlerContract.GetItem(userWalletManager.WalletAddress, inputData[1].ConvertFromHex(), item => UpdateItemList(item, lockedTimeStamp));
    }

    private void UpdateItemList(Hodler.Output.Item item, BigInteger lockedTimeStamp)
    {
        AddNewItem(item, lockedTimeStamp);
        CheckItemUpdateCounter();
    }

    private void AddNewItem(Hodler.Output.Item item, BigInteger lockedTimeStamp)
    {
        if (!lockedItems.Select(lockedItem => lockedItem.LockedTimeStamp).Contains(lockedTimeStamp))
        {
            UpdateItemInfo(item, lockedTimeStamp);
            lockedItems.Add(item);
            lockedItems.Sort((i1, i2) => i1.LockedTimeStamp.CompareTo(i2.LockedTimeStamp));
            UsedIds.Add(item.Id);
        }
        else
        {
            var oldItem = lockedItems.Single(lockedItem => lockedItem.Id == item.Id);
            oldItem.Fulfilled = item.Fulfilled;
            UpdateItemInfo(oldItem, lockedTimeStamp);
        }
    }

    private void UpdateItemInfo(Hodler.Output.Item item, BigInteger lockedTimeStamp)
    {
        item.LockedTimeStamp = lockedTimeStamp;

        if (item.Unlockable && !item.UnlockableGasLimit.HasValue)
        {
            GasUtils.EstimateGasLimit<Hodler.Messages.Release>(hodlerContract.ContractAddress,
                                                               userWalletManager.WalletAddress,
                                                               limit => item.UnlockableGasLimit = limit,
                                                               item.Id);
        }
    }

    private void CheckItemUpdateCounter()
    {
        if (++updateCounter == updateCount)
            OnLockedPRPSUpdated?.Invoke();
    }
}