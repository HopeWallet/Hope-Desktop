using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TabType = OpenWalletMenu.TransactionTabManager.TabType;
using TransactionType = TransactionInfo.TransactionType;
using System.Collections.Generic;

public sealed partial class OpenWalletMenu
{
    public sealed class TransactionPageManager
    {
        public static event Action<int> OnPageChanged;

        private readonly TradableAssetManager tradableAssetManager;
        private readonly EthereumTransactionManager ethereumTransactionManager;

        private readonly GameObject pagesSection;

        private readonly Button leftButton,
                                rightButton;

        private readonly TMP_Text pageNumText;

        private int pageNumber;

        private string previousAssetAddress;

        private TabType activeTabType;

        public TransactionPageManager(
            TradableAssetManager tradableAssetManager,
            EthereumTransactionManager ethereumTransactionManager,
            GameObject pagesSection)
        {
            this.tradableAssetManager = tradableAssetManager;
            this.ethereumTransactionManager = ethereumTransactionManager;
            this.pagesSection = pagesSection;
            leftButton = pagesSection.GetComponentsInChildren<Button>()[0];
            rightButton = pagesSection.GetComponentsInChildren<Button>()[1];
            pageNumText = pagesSection.GetComponentInChildren<TMP_Text>();

            TransactionTabManager.OnTabChanged += TabChanged;
            TradableAssetManager.OnBalancesUpdated += TransactionListOrAssetUpdated;
            ethereumTransactionManager.OnTransactionsAdded += TransactionListOrAssetUpdated;

            leftButton.onClick.AddListener(() => PageChanged(true));
            rightButton.onClick.AddListener(() => PageChanged(false));
        }

        private void TransactionListOrAssetUpdated()
        {
            var transactionList = GetTransactionList();
            var enableSection = transactionList?.Count > EthereumTransactionButtonManager.MAX_TRANSACTIONS_PER_PAGE;

            if ((enableSection && !pagesSection.activeInHierarchy) || !tradableAssetManager.ActiveTradableAsset.AssetAddress.EqualsIgnoreCase(previousAssetAddress))
                ResetPageSection();
            else
                UpdatePageSection(transactionList);

            pagesSection.SetActive(enableSection);

            previousAssetAddress = tradableAssetManager.ActiveTradableAsset.AssetAddress;
        }

        private void TabChanged(TabType tabType)
        {
            activeTabType = tabType;

            var transactionList = GetTransactionList();

            pagesSection.SetActive(transactionList?.Count > EthereumTransactionButtonManager.MAX_TRANSACTIONS_PER_PAGE);
            ResetPageSection();
        }

        private void PageChanged(bool left)
        {
            pageNumber = left ? --pageNumber : ++pageNumber;

            var transactionList = GetTransactionList();
            UpdatePageSection(transactionList);
        }

        private void ResetPageSection()
        {
            pageNumber = 0;
            pageNumText.text = 1.ToString();
            leftButton.interactable = false;
            rightButton.interactable = true;
        }

        private void UpdatePageSection(List<TransactionInfo> transactionList)
        {
            var upperLimit = EthereumTransactionButtonManager.MAX_TRANSACTIONS_PER_PAGE * (pageNumber + 1);

            pageNumText.text = (pageNumber + 1).ToString();
            rightButton.interactable = transactionList?.Count > upperLimit;
            leftButton.interactable = pageNumber != 0;
        }

        private List<TransactionInfo> GetTransactionList()
        {
            var activeAsset = tradableAssetManager.ActiveTradableAsset;
            var address = activeAsset == null ? EtherAsset.ETHER_ADDRESS : activeAsset.AssetAddress;
            var transactionList = ethereumTransactionManager.GetTransactionListByAddress(address);

            if (transactionList != null && activeTabType != TabType.All)
                transactionList = ethereumTransactionManager.GetTransactionsByAddressAndType(address, activeTabType == TabType.Received ? TransactionType.Receive : TransactionType.Send);

            return transactionList;
        }
    }
}