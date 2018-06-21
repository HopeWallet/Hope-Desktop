using Hope.Utils.EthereumUtils;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Vector3 = UnityEngine.Vector3;

public class PRPSLockPopup : ExitablePopupComponent<PRPSLockPopup>, IPeriodicUpdater, IStandardGasPriceObservable
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

    public float UpdateInterval => 10f;

    public GasPrice StandardGasPrice { get; set; }

    [Inject]
    public void Construct(HodlerContract hodlerContract, 
        TradableAssetManager tradableAssetManager, 
        UserWalletManager userWalletManager, 
        PeriodicUpdateManager periodicUpdateManager,
        EthereumNetworkManager ethereumNetworkManager,
        LockedPRPSItemButton.Factory lockedPRPSItemFactory)
    {
        this.hodlerContract = hodlerContract;
        this.tradableAssetManager = tradableAssetManager;
        this.userWalletManager = userWalletManager;
        this.periodicUpdateManager = periodicUpdateManager;
        this.lockedPRPSItemFactory = lockedPRPSItemFactory;
        ethereumNetwork = ethereumNetworkManager.CurrentNetwork;
    }

    private void Awake()
    {
        StartLockPurposeHelper();
        UpdatePRPSDisplay();
    }

    private void OnEnable()
    {
        TradableAssetManager.OnBalancesUpdated += UpdatePRPSDisplay;
        periodicUpdateManager.AddPeriodicUpdater(this, true);
    }

    private void OnDisable()
    {
        TradableAssetManager.OnBalancesUpdated -= UpdatePRPSDisplay;

        periodicUpdateManager.RemovePeriodicUpdater(this);

        //lockPurposeHelper.Stop();
        //ReleasePurposeHelper.Stop();
    }

    public void PeriodicUpdate() => StartNewItemSearch();

    private void UpdatePRPSDisplay() => prpsBalanceText.text = StringUtils.LimitEnd(tradableAssetManager.ActiveTradableAsset.AssetBalance + "", 18, "...");

    private void StartLockPurposeHelper()
    {
        //lockPurposeHelper.Start(hodlerContract[HodlerContract.FUNC_HODL],
        //                        null,
        //                        new BigInteger(999999),
        //                        SolidityUtils.ConvertToUInt(tradableAssetManager.ActiveTradableAsset.AssetBalance, 18),
        //                        new BigInteger(12));
    }

    private void StartNewItemSearch()
    {
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
        //ReleasePurposeHelper.Start(hodlerContract[HodlerContract.FUNC_RELEASE],
        //                           OnReleaseGasEstimated,
        //                           item.Id);

        item.LockedTimeStamp = timeStamp;

        var sameItems = items.Where(i => i.ButtonInfo.ReleaseTime == item.ReleaseTime);
        var currentItemButton = sameItems.Count() == 0 ? CreateItemButton(item) : sameItems.Single();

        currentItemButton.SetButtonInfo(item);

        items.Sort((i1, i2) => i1.ButtonInfo.LockedTimeStamp.CompareTo(i2.ButtonInfo.LockedTimeStamp));
        items.ForEach(i => i.transform.SetSiblingIndex(items.IndexOf(i)));

        //OnReleaseGasEstimated();
    }

    //private void OnReleaseGasEstimated()
    //{
    //    items.ForEach(item => item.UpdateTransactionGas(ReleasePurposeHelper));
    //}

    public void OnGasPricesUpdated()
    {
    }

    private void RemoveItemButton(HodlerItem item)
    {
        var sameItems = items.Where(i => i.ButtonInfo.ReleaseTime == item.ReleaseTime);

        if (sameItems.Count() > 0)
        {
            var sameItem = sameItems.Single();
            items.Remove(sameItem);
            Destroy(sameItem.gameObject);
        }
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