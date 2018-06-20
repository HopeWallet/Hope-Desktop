using Hope.Utils.EthereumUtils;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Vector3 = UnityEngine.Vector3;

public class PRPSLockPopup : ExitablePopupComponent<PRPSLockPopup>, IPeriodicUpdater
{

    public Text prpsBalanceText;

    public Transform itemSpawnTransform;

    private readonly List<LockedPRPSItemButton> items = new List<LockedPRPSItemButton>();

    private HodlerContract hodlerContract;
    private TradableAssetManager tradableAssetManager;
    private UserWalletManager userWalletManager;
    private PeriodicUpdateManager periodicUpdateManager;
    private EthereumNetwork ethereumNetwork;
    private LockedPRPSItemButton.Factory lockedPRPSItemFactory;
    private TransactionHelper lockPurposeHelper;

    public float UpdateInterval => 10f;

    public TransactionHelper ReleasePurposeHelper { get; private set; }

    [Inject]
    public void Construct(HodlerContract hodlerContract, 
        TradableAssetManager tradableAssetManager, 
        UserWalletManager userWalletManager, 
        PeriodicUpdateManager periodicUpdateManager,
        EthereumNetworkManager ethereumNetworkManager,
        LockedPRPSItemButton.Factory lockedPRPSItemFactory,
        TransactionHelper lockPurposeHelper,
        TransactionHelper releasePurposeHelper)
    {
        this.hodlerContract = hodlerContract;
        this.tradableAssetManager = tradableAssetManager;
        this.userWalletManager = userWalletManager;
        this.periodicUpdateManager = periodicUpdateManager;
        this.lockedPRPSItemFactory = lockedPRPSItemFactory;
        this.lockPurposeHelper = lockPurposeHelper;
        ReleasePurposeHelper = releasePurposeHelper;
        ethereumNetwork = ethereumNetworkManager.CurrentNetwork;
    }

    private void Awake()
    {
        StartNewItemSearch();
        StartLockPurposeHelper();
    }

    private void OnEnable() => periodicUpdateManager.AddPeriodicUpdater(this);

    private void OnDisable()
    {
        periodicUpdateManager.RemovePeriodicUpdater(this);

        lockPurposeHelper.Stop();
        ReleasePurposeHelper.Stop();
    }

    public void PeriodicUpdate() => StartNewItemSearch();

    private void StartLockPurposeHelper()
    {
        lockPurposeHelper.Start(hodlerContract[HodlerContract.FUNC_HODL],
                                null,
                                new BigInteger(999999),
                                SolidityUtils.ConvertToUInt(tradableAssetManager.ActiveTradableAsset.AssetBalance, 18),
                                12);
    }

    private void StartNewItemSearch()
    {
        prpsBalanceText.text = StringUtils.LimitEnd(tradableAssetManager.ActiveTradableAsset.AssetBalance + "", 18, "...");

        WebClientUtils.GetTransactionList(ethereumNetwork.Api.GetTokenTransfersFromAndToUrl(tradableAssetManager.ActiveTradableAsset.AssetAddress,
                                                                                    userWalletManager.WalletAddress,
                                                                                    hodlerContract.ContractAddress),
                                                                                    txList => ProcessTxList(txList));
    }

    private void ProcessTxList(string txList)
    {
        var transactionsJson = JsonUtils.GetJsonData<EtherscanAPIJson<TokenTransactionJson>>(txList);
        if (transactionsJson == null)
            return;

        transactionsJson.result.Reverse().ForEach(json => GetTransactionInputData(json));
    }

    private void GetTransactionInputData(TokenTransactionJson tokenTransactionJson)
    {
        TransactionUtils.CheckTransactionDetails(tokenTransactionJson.transactionHash, 
            tx => UpdateItems(SolidityUtils.ExtractFunctionParameters(tx.Input), tokenTransactionJson.timeStamp.ConvertFromHex()));
    }

    private void UpdateItems(string[] inputData, BigInteger timeStamp)
    {
        hodlerContract.GetItem(userWalletManager.WalletAddress, inputData[1].ConvertFromHex(), item =>
        {
            if (item.Fulfilled)
                RemoveItemButton(item);
            else
                UpdateList(item, timeStamp);
        });
    }

    private void UpdateList(HodlerItem item, BigInteger timeStamp)
    {
        ReleasePurposeHelper.Start(hodlerContract[HodlerContract.FUNC_RELEASE],
                                   OnReleaseGasEstimated,
                                   item.Id);

        item.LockedTimeStamp = timeStamp;

        var sameItems = items.Where(i => i.ButtonInfo.ReleaseTime == item.ReleaseTime);
        var currentItemButton = sameItems.Count() == 0 ? CreateItemButton(item) : sameItems.Single();

        currentItemButton.SetButtonInfo(item);

        items.Sort((i1, i2) => i1.ButtonInfo.LockedTimeStamp.CompareTo(i2.ButtonInfo.LockedTimeStamp));
        items.ForEach(i => i.transform.SetSiblingIndex(items.IndexOf(i)));

        OnReleaseGasEstimated();
    }

    private void OnReleaseGasEstimated()
    {
        items.ForEach(item => item.UpdateTransactionGas(ReleasePurposeHelper));
    }

    private void RemoveItemButton(HodlerItem item)
    {
        var sameItems = items.Where(i => i.ButtonInfo.ReleaseTime == item.ReleaseTime);

        if (sameItems.Count() > 0)
            items.Remove(sameItems.Single());
    }

    private LockedPRPSItemButton CreateItemButton(HodlerItem item)
    {
        var newItem = lockedPRPSItemFactory.Create();
        var rectTransform = newItem.GetComponent<RectTransform>();

        rectTransform.parent = itemSpawnTransform;
        rectTransform.localScale = Vector3.one;

        items.Add(newItem);

        return newItem;
    }

}