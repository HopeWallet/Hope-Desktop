using Hope.Utils.EthereumUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public sealed class LockedPRPSManager : IPeriodicUpdater
{
    public event Action OnLockedPRPSUpdated;

    private readonly List<HodlerMimic.Output.Item> lockedItems = new List<HodlerMimic.Output.Item>();

    private readonly UserWalletManager userWalletManager;
    private readonly HodlerMimic hodlerContract;
    private readonly EthereumNetwork ethereumNetwork;

    private const string PRPS_MAINNET_ADDRESS = "0xd94F2778e2B3913C53637Ae60647598bE588c570";
    private const string PRPS_RINKEBY_ADDRESS = "0x5831819C84C05DdcD2568dE72963AC9f1e6831b6";

    private int updateCount;
    private int updateCounter;

    public float UpdateInterval => 10f;

    public string PRPSAddress { get; }

    public List<HodlerMimic.Output.Item> UnfulfilledItems => lockedItems.Where(item => !item.Fulfilled).ToList();

    public List<HodlerMimic.Output.Item> UnlockableItems => lockedItems.Where(item => !item.Fulfilled && item.ReleaseTime < DateTimeUtils.GetCurrentUnixTime()).ToList();

    public LockedPRPSManager(
        UserWalletManager userWalletManager,
        HodlerMimic hodlerContract,
        EthereumNetworkManager ethereumNetworkManager,
        EthereumNetworkManager.Settings ethereumNetworkSettings,
        PeriodicUpdateManager periodicUpdateManager)
    {
        this.userWalletManager = userWalletManager;
        this.hodlerContract = hodlerContract;
        ethereumNetwork = ethereumNetworkManager.CurrentNetwork;

        PRPSAddress = ethereumNetworkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet ? PRPS_MAINNET_ADDRESS : PRPS_RINKEBY_ADDRESS;

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
        UnityWebUtils.DownloadString(ethereumNetwork.Api.GetTokenTransfersFromAndToUrl(PRPSAddress,
                                                                                       userWalletManager.WalletAddress,
                                                                                       hodlerContract.ContractAddress),
                                                                                       ProcessTxList);
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

    private void UpdateItemList(HodlerMimic.Output.Item item, BigInteger lockedTimeStamp)
    {
        AddNewItem(item, lockedTimeStamp);
        CheckItemUpdateCounter();
    }

    private void AddNewItem(HodlerMimic.Output.Item item, BigInteger lockedTimeStamp)
    {
        item.LockedTimeStamp = lockedTimeStamp;

        if (!lockedItems.Select(lockedItem => lockedItem.LockedTimeStamp).Contains(lockedTimeStamp))
        {
            lockedItems.Add(item);
            lockedItems.Sort((i1, i2) => i1.LockedTimeStamp.CompareTo(i2.LockedTimeStamp));
        }
    }

    private void CheckItemUpdateCounter()
    {
        if (++updateCounter == updateCount)
            OnLockedPRPSUpdated?.Invoke();
    }

}