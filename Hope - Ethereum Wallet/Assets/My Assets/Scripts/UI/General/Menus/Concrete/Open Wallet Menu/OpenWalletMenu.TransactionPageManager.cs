using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TabType = OpenWalletMenu.TransactionTabManager.TabType;
using TransactionType = TransactionInfo.TransactionType;

public sealed partial class OpenWalletMenu
{
    public sealed class TransactionPageManager
    {
        private readonly TradableAssetManager tradableAssetManager;
        private readonly EthereumTransactionManager ethereumTransactionManager;

        private readonly GameObject pagesSection;

        private readonly Button leftButton,
                                rightButton;

        private readonly TMP_Text pageNumText;

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

            TransactionTabManager.OnTabChanged += UpdatePageSection;
            TradableAssetManager.OnBalancesUpdated += () => UpdatePageSection(activeTabType);
            ethereumTransactionManager.OnTransactionsAdded += () => UpdatePageSection(activeTabType);
        }

        private void UpdatePageSection(TabType tabType)
        {
            activeTabType = tabType;

            var activeAsset = tradableAssetManager.ActiveTradableAsset;
            var address = activeAsset == null ? EtherAsset.ETHER_ADDRESS : activeAsset.AssetAddress;
            var transactionList = ethereumTransactionManager.GetTransactionListByAddress(address);

            if (transactionList != null && activeTabType != TabType.All)
                transactionList = ethereumTransactionManager.GetTransactionsByAddressAndType(address, activeTabType == TabType.Received ? TransactionType.Receive : TransactionType.Send);

            pagesSection.SetActive(transactionList?.Count > 50);
            pageNumText.text = "1";
            leftButton.interactable = false;
            rightButton.interactable = true;
        }
    }
}