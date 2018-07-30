using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

/// <summary>
/// Class which represents the popup used for displaying current locked purpose.
/// </summary>
public class LockedPRPSPopup : ExitablePopupComponent<LockedPRPSPopup>
{
    public Transform itemSpawnTransform;

    private readonly List<LockedPRPSItemButton> lockedPRPSItems = new List<LockedPRPSItemButton>();

    private LockedPRPSManager lockedPRPSManager;
    private LockedPRPSItemButton.Factory lockedPRPSItemFactory;

    [Inject]
    public void Construct(
        LockedPRPSManager lockedPRPSManager,
        LockedPRPSItemButton.Factory lockedPRPSItemFactory)
    {
        this.lockedPRPSManager = lockedPRPSManager;
        this.lockedPRPSItemFactory = lockedPRPSItemFactory;
    }

    protected override void Awake()
    {
        base.Awake();

        CreateInitialItemList();
        (Animator as LockedPRPSPopupAnimator).LockedPurposeItems = lockedPRPSItems.Select(item => item.gameObject).ToArray();
    }

    private void CreateInitialItemList()
    {
        lockedPRPSManager.UnfulfilledItems.ForEach(CreateItem);
    }

    private void CreateItem(HodlerMimic.Output.Item item)
    {
        LockedPRPSItemButton itemButton = lockedPRPSItemFactory.Create().SetButtonInfo(item);
        itemButton.transform.parent = itemSpawnTransform;

        lockedPRPSItems.Add(itemButton);
    }
}

//public Transform itemSpawnTransform;
//public Dropdown lockPeriodDropdown;
//public Button lockButton;
//public Text prpsBalanceText;
//public InputField lockAmountField,
//                  rewardAmountField;

//private readonly List<LockedPRPSItemButton> items = new List<LockedPRPSItemButton>();
//private readonly HashSet<BigInteger> usedIds = new HashSet<BigInteger>();

//private HodlerContract hodlerContract;
//private TradableAssetManager tradableAssetManager;
//private UserWalletManager userWalletManager;
//private PeriodicUpdateManager periodicUpdateManager;
//private EthereumNetwork ethereumNetwork;
//private LockedPRPSItemButton.Factory lockedPRPSItemFactory;

//private FunctionGasEstimator lockPurposeEstimator;
//private FunctionGasEstimator releasePurposeEstimator;

//private decimal purposeToLock;
//private bool closed;

//public float UpdateInterval => 10f;

///// <summary>
///// Adds all required dependencies to this class.
///// </summary>
///// <param name="hodlerContract"> The HodlerContract. </param>
///// <param name="tradableAssetManager"> The active TradableAssetManager. </param>
///// <param name="userWalletManager"> The active UserWalletManager. </param>
///// <param name="periodicUpdateManager"> The active PeriodicUpdateManager. </param>
///// <param name="ethereumNetworkManager"> The active EthereumNetworkManager. </param>
///// <param name="lockedPRPSItemFactory"> The factory for creating LockedPRPSItemButtons. </param>
///// <param name="lockPurposeEstimator"> The FunctionGasEstimator for estimating the cost of locking purpose. </param>
///// <param name="releasePurposeEstimator"> The FunctionGasEstimator for estimating the cost of releasing purpose. </param>
//[Inject]
//public void Construct(HodlerContract hodlerContract, 
//    TradableAssetManager tradableAssetManager, 
//    UserWalletManager userWalletManager, 
//    PeriodicUpdateManager periodicUpdateManager,
//    EthereumNetworkManager ethereumNetworkManager,
//    LockedPRPSItemButton.Factory lockedPRPSItemFactory,
//    FunctionGasEstimator lockPurposeEstimator,
//    FunctionGasEstimator releasePurposeEstimator)
//{
//    this.hodlerContract = hodlerContract;
//    this.tradableAssetManager = tradableAssetManager;
//    this.userWalletManager = userWalletManager;
//    this.periodicUpdateManager = periodicUpdateManager;
//    this.lockedPRPSItemFactory = lockedPRPSItemFactory;
//    this.lockPurposeEstimator = lockPurposeEstimator;
//    this.releasePurposeEstimator = releasePurposeEstimator;

//    ethereumNetwork = ethereumNetworkManager.CurrentNetwork;
//}

///// <summary>
///// Sets the text of the purpose display as well as starts the LockPurposeEstimator.
///// </summary>
//protected override void Awake()
//{
//    base.Awake();
//    OnPurposeUpdated();
//}

///// <summary>
///// Adds all required callbacks to the events, managers, and buttons.
///// </summary>
//private void OnEnable()
//{
//    closed = false;

//    TradableAssetManager.OnBalancesUpdated += OnPurposeUpdated;
//    periodicUpdateManager.AddPeriodicUpdater(this, true);

//    lockAmountField.onValueChanged.AddListener(_ => UpdateLockFields());
//    lockPeriodDropdown.onValueChanged.AddListener(_ => UpdateLockFields());
//    lockButton.onClick.AddListener(LockPurpose);
//}

///// <summary>
///// Removes the callbacks and stops the estimators.
///// </summary>
//private void OnDisable()
//{
//    closed = true;

//    TradableAssetManager.OnBalancesUpdated -= OnPurposeUpdated;

//    periodicUpdateManager.RemovePeriodicUpdater(this);

//    lockPurposeEstimator.StopEstimation();
//    releasePurposeEstimator.StopEstimation();
//}

///// <summary>
///// Searches for any new locked items, or any locked items that are now unlocked.
///// </summary>
//public void PeriodicUpdate() => StartNewItemSearch();

///// <summary>
///// Estimates the gas limit of the lock function, updates purpose display to the newest balance, and fixes the lock input fields.
///// </summary>
//private void OnPurposeUpdated()
//{
//    lockPurposeEstimator.Estimate(hodlerContract[HodlerContract.FUNC_HODL],
//                                   null,
//                                   new BigInteger(999999),
//                                   SolidityUtils.ConvertToUInt(tradableAssetManager.ActiveTradableAsset.AssetBalance, 18),
//                                   new BigInteger(12));

//    prpsBalanceText.text = StringUtils.LimitEnd(tradableAssetManager.ActiveTradableAsset.AssetBalance + "", 18, "...");

//    UpdateLockFields();
//}

///// <summary>
///// Searches for any token transfers from the user's address to the hodl contract.
///// </summary>
//private void StartNewItemSearch()
//{
//    UnityWebUtils.DownloadString(ethereumNetwork.Api.GetTokenTransfersFromAndToUrl(tradableAssetManager.ActiveTradableAsset.AssetAddress,
//                                                                                userWalletManager.WalletAddress,
//                                                                                hodlerContract.ContractAddress),
//                                                                                ProcessTxList);
//}

///// <summary>
///// Processes the list of purpose transactions sent out to the hodl contract.
///// </summary>
///// <param name="txList"> The list of transactions. </param>
//private void ProcessTxList(string txList)
//{
//    var transactionsJson = JsonUtils.GetJsonData<EtherscanAPIJson<TokenTransactionJson>>(txList);
//    if (transactionsJson == null)
//        return;

//    transactionsJson.result.Reverse().ForEach(GetTransactionInputData);
//}

///// <summary>
///// Checks the details of each purpose transaction sent to the hodl contract.
///// </summary>
///// <param name="tokenTransactionJson"> The json of the token transaction. </param>
//private void GetTransactionInputData(TokenTransactionJson tokenTransactionJson)
//{
//    TransactionUtils.CheckTransactionDetails(tokenTransactionJson.transactionHash, 
//        tx => UpdateItems(SolidityUtils.ExtractFunctionParameters(tx.Input), tokenTransactionJson.timeStamp.ConvertFromHex()));
//}

///// <summary>
///// Updates the current list of items with the newest item found from the transaction list.
///// </summary>
///// <param name="inputData"> The input data found in the transaction. </param>
///// <param name="timeStamp"> The time stamp of the transaction. </param>
//private void UpdateItems(string[] inputData, BigInteger timeStamp)
//{
//    hodlerContract.GetItem(userWalletManager.WalletAddress, inputData[1].ConvertFromHex(), item =>
//    {
//        if (closed)
//            return;

//        if (item.Fulfilled)
//            RemoveItemButton(item);
//        else
//            UpdateList(item, timeStamp);
//    });
//}

///// <summary>
///// Updates the list with an item that is still unclaimed and locked into the smart contract.
///// </summary>
///// <param name="item"> The item found and still unclaimed. </param>
///// <param name="timeStamp"> The timestamp the purpose was locked in. </param>
//private void UpdateList(HodlerMimic.Output.Item item, BigInteger timeStamp)
//{
//    item.LockedTimeStamp = timeStamp;

//    var sameItems = items.Where(i => i.ButtonInfo.ReleaseTime == item.ReleaseTime);
//    var currentItemButton = !sameItems.Any() ? CreateItemButton(item) : sameItems.Single();

//    currentItemButton.SetButtonInfo(item);

//    items.Sort((i1, i2) => i1.ButtonInfo.LockedTimeStamp.CompareTo(i2.ButtonInfo.LockedTimeStamp));
//    items.ForEach(i => i.transform.SetSiblingIndex(items.IndexOf(i)));

//    EstimateReleaseGas();
//    UpdateItemTransactionDetails();
//}

///// <summary>
///// Estimates the gas required to release the purpose from the contract.
///// </summary>
//private void EstimateReleaseGas()
//{
//    if (items.Count == 0 || releasePurposeEstimator.GasLimit != null)
//        return;

//    releasePurposeEstimator.Estimate(hodlerContract[HodlerContract.FUNC_RELEASE],
//                                      UpdateItemTransactionDetails,
//                                      items[0].ButtonInfo.Id);
//}

///// <summary>
///// Called when the release gas is estimated or every time new items are added.
///// Makes sure each item has the transaction details to release the purpose.
///// </summary>
//private void UpdateItemTransactionDetails()
//{
//    items.ForEach(item => item.UpdateTransactionDetails(releasePurposeEstimator));
//}

///// <summary>
///// Removes an item button from the current list.
///// </summary>
///// <param name="item"> The item to remove. </param>
//private void RemoveItemButton(HodlerMimic.Output.Item item)
//{
//    var sameItems = items.Where(i => i.ButtonInfo.ReleaseTime == item.ReleaseTime);

//    if (sameItems.Any())
//    {
//        var sameItem = sameItems.Single();
//        items.Remove(sameItem);
//        Destroy(sameItem.gameObject);
//    }

//    else
//    {
//        usedIds.Add(item.Id);
//    }
//}

///// <summary>
///// Creates a new item button from a HodlerItem.
///// </summary>
///// <param name="item"> The item found still unclaimed and locked in the smart contract. </param>
///// <returns> The newly created item object. </returns>
//private LockedPRPSItemButton CreateItemButton(HodlerMimic.Output.Item item)
//{
//    var newItem = lockedPRPSItemFactory.Create();
//    var rectTransform = newItem.GetComponent<RectTransform>();

//    rectTransform.parent = itemSpawnTransform;
//    rectTransform.localScale = Vector3.one;

//    items.Add(newItem);
//    usedIds.Add(item.Id);

//    return newItem;
//}

///// <summary>
///// Updates the lock purpose fields to reflect the newly changed transaction estimates, or the changed input values.
///// </summary>
//private void UpdateLockFields()
//{
//    lockAmountField.RestrictToBalance(tradableAssetManager.ActiveTradableAsset);
//    purposeToLock = string.IsNullOrEmpty(lockAmountField.text) ? 0 : decimal.Parse(lockAmountField.text);
//    rewardAmountField.text = (purposeToLock * Math.Round(((decimal)(lockPeriodDropdown.value + 1) / 100) * (decimal)1.2, 2)).ToString();

//    lockButton.interactable = lockPurposeEstimator.CanExecuteTransaction
//                              && (purposeToLock >= (decimal)0.0000000000000001
//                              && purposeToLock <= tradableAssetManager.ActiveTradableAsset.AssetBalance);
//}

///// <summary>
///// Executes the 'hodl' function of the hodler smart contract.
///// Locks in the amount of purpose entered in the input field.
///// </summary>
//private void LockPurpose()
//{
//    hodlerContract.Hodl(userWalletManager,
//                        lockPurposeEstimator.GasLimit,
//                        lockPurposeEstimator.StandardGasPrice.FunctionalGasPrice,
//                        RandomUtils.GenerateRandomBigInteger(usedIds),
//                        purposeToLock,
//                        (int)(Math.Round((lockPeriodDropdown.value + 1) * (decimal)1.2) * 3));
//}