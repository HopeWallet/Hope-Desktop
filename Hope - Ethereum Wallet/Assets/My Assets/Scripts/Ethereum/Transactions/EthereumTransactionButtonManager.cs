using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TabType = OpenWalletMenu.TransactionTabManager.TabType;
using TransactionType = TransactionInfo.TransactionType;

/// <summary>
/// Class which manages the transaction buttons.
/// </summary>
public sealed class EthereumTransactionButtonManager
{
    private readonly Settings settings;
    private readonly TradableAssetManager tradableAssetManager;
    private readonly EthereumTransactionManager transactionManager;
    private readonly TransactionInfoButton.Factory buttonFactory;

    private readonly List<TransactionInfoButton> transactionButtons = new List<TransactionInfoButton>();

    private TabType activeTabType;

    public const int MAX_TRANSACTIONS_PER_PAGE = 50;

    /// <summary>
    /// Initializes the EthereumTransactionButtonManager by assigning the settings.
    /// </summary>
    /// <param name="settings"> The settings of this manager. </param>
    /// <param name="tradableAssetManager"> The TradableAssetManager to retrieve the current asset from. </param>
    /// <param name="transactionManager"> The EthereumTransactionManager to use to get the current transaction list. </param>
    /// <param name="buttonFactory"> The TransactionInfoButton factory. </param>
    public EthereumTransactionButtonManager(Settings settings,
        TradableAssetManager tradableAssetManager,
        EthereumTransactionManager transactionManager,
        TransactionInfoButton.Factory buttonFactory)
    {
        this.settings = settings;
        this.tradableAssetManager = tradableAssetManager;
        this.transactionManager = transactionManager;
        this.buttonFactory = buttonFactory;

        transactionManager.OnTransactionsAdded += ProcessTransactions;
        OpenWalletMenu.TransactionTabManager.OnTabChanged += OnTransactionTabChanged;
    }

    /// <summary>
    /// Refreshes the transaction info for all transaction buttons.
    /// </summary>
    public void Refresh()
    {
        transactionButtons.ForEach(transaction => transaction.SetButtonInfo(transaction.ButtonInfo));
    }

    /// <summary>
    /// Method called when a new asset is set to active.
    /// </summary>
    public void ProcessNewAssetList()
    {
        ProcessTransactions();
        RefreshScrollBar();
    }

    /// <summary>
    /// Processes the transactions when they are updated in the EthereumTransactionManager.
    /// </summary>
    private void ProcessTransactions()
    {
        var transactionList = GetValidTransactionList();

        UpdateButtons(transactionList);
        UpdateTransactionList(transactionList);
        UpdateLoadingVisuals(transactionList);
    }

    /// <summary>
    /// Gets the valid transaction list based on the current tab.
    /// </summary>
    /// <returns> The transaction list filtered to only include transactions valid based on the current tab. </returns>
    private List<TransactionInfo> GetValidTransactionList()
    {
        var activeAsset = tradableAssetManager.ActiveTradableAsset;
        var address = activeAsset == null ? EtherAsset.ETHER_ADDRESS : activeAsset.AssetAddress;
        var transactionList = transactionManager.GetTransactionListByAddress(address);

        if (transactionList != null && activeTabType != TabType.All)
            transactionList = transactionManager.GetTransactionsByAddressAndType(address, activeTabType == TabType.Received ? TransactionType.Receive : TransactionType.Send);

        return transactionList;
    }

    /// <summary>
    /// Updates the list of transaction buttons to correspond with the list of TransactionInfo objects.
    /// </summary>
    /// <param name="transactionList"> The list of TransactionInfo objects to set buttons for. </param>
    private void UpdateTransactionList(List<TransactionInfo> transactionList)
    {
        if (transactionList == null)
            return;

        for (int i = transactionList.Count - 1; i >= 0; i--)
            SetTransactionButton(transactionList[i], transactionList.Count - i - 1);
    }

    /// <summary>
    /// Gets the button for a transaction at a given index. Creates a new one if it doesn't exist already.
    /// </summary>
    /// <param name="transactionInfo"> The TransactionInfo object to assign to the TransactionInfoButton. </param>
    /// <param name="index"> The index of the button in the list. </param>
    /// <returns> The button at that given index, or newly created. </returns>
    private void SetTransactionButton(TransactionInfo transactionInfo, int index)
    {
        if (index >= transactionButtons.Count)
            transactionButtons.Add(buttonFactory.Create().SetButtonInfo(transactionInfo));

        transactionButtons[index].SetButtonInfo(transactionInfo);
    }

    /// <summary>
    /// Updates the text to reflect the list of transactions.
    /// Text will display "Loading Transactions..." if the list is null.
    /// Text will display "No transactions found." if the list is empty.
    /// </summary>
    /// <param name="transactionList"> The list of transactions to use to update the text. </param>
    private void UpdateLoadingVisuals(List<TransactionInfo> transactionList)
    {
        if (settings.loadingText == null)
            return;

        settings.loadingIconObject.SetActive(transactionList == null);
        settings.loadingText.GetComponent<LoadingTextAnimator>().IsStopped = transactionList != null;
        settings.loadingText.gameObject.SetActive(transactionList == null || transactionList.Count == 0);
        settings.loadingText.text = transactionList?.Count == 0 ? "No transactions found." : "Loading transactions";
    }

    /// <summary>
    /// Updates the button visibility based on the number of transactions in the list.
    /// </summary>
    /// <param name="transactionList"> The list to use to set the button visibility. </param>
    private void UpdateButtons(List<TransactionInfo> transactionList)
    {
        var buttonCount = transactionList?.Count ?? 0;

        for (int i = 0; i < transactionButtons.Count; i++)
            if (transactionButtons?[i] != null)
                transactionButtons[i].transform.parent.gameObject.SetActive(i < buttonCount);
    }

    /// <summary>
    /// Resets the value of the scroll bar once an asset changes.
    /// Also refreshes the scrollview.
    /// </summary>
    private void RefreshScrollBar()
    {
        settings.scrollBar.value = 1;
        OptimizedScrollview.GetScrollview("transactions_scrollview").Refresh();
    }

    /// <summary>
    /// Called when the transaction tab changes.
    /// </summary>
    /// <param name="tabType"> The new transaction tab type. </param>
    private void OnTransactionTabChanged(TabType tabType)
    {
        activeTabType = tabType;
        ProcessNewAssetList();
    }

    /// <summary>
    /// Class which represents the settings of this transaction button manager.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public GameObject loadingIconObject;
        public Transform spawnTransform;
        public TMP_Text loadingText;
        public Scrollbar scrollBar;
    }
}
