using System;
using System.Linq;
using TMPro;

public sealed partial class OpenWalletMenu
{
    public sealed class TransactionTabManager
    {
        public static event Action<TabType> OnTabChanged;

        private readonly TradableAssetManager tradableAssetManager;
        private readonly EthereumTransactionManager ethereumTransactionManager;

        private readonly IconButtons transactionTabs;

        private readonly TMP_Text allTabText;
        private readonly TMP_Text sentTabText;
        private readonly TMP_Text receivedTabText;

        public TransactionTabManager(
            TradableAssetManager tradableAssetManager,
            EthereumTransactionManager ethereumTransactionManager,
            IconButtons transactionTabs)
        {
            this.tradableAssetManager = tradableAssetManager;
            this.ethereumTransactionManager = ethereumTransactionManager;
            this.transactionTabs = transactionTabs;

            allTabText = transactionTabs.gameObject.GetComponentsInChildren<TMP_Text>()[0];
            sentTabText = transactionTabs.gameObject.GetComponentsInChildren<TMP_Text>()[1];
            receivedTabText = transactionTabs.gameObject.GetComponentsInChildren<TMP_Text>()[2];

            tradableAssetManager.OnBalancesUpdated += ChangeText;
            ethereumTransactionManager.OnTransactionsAdded += ChangeText;
            transactionTabs.OnButtonChanged += TabChanged;

            AccountsPopup.OnAccountChanged += _ => ChangeText();
        }

        private void ChangeText()
        {
            var transactionList = ethereumTransactionManager.GetTransactionListByAddress(tradableAssetManager.ActiveTradableAsset?.AssetAddress);

            if (transactionList == null)
            {
                allTabText.text = "All...";
                sentTabText.text = "Sent...";
                receivedTabText.text = "Received...";
            }
            else
            {
                var transactionCount = transactionList.Count;
                var sentTransactionCount = transactionList.Count(transaction => transaction.Type == TransactionInfo.TransactionType.Send);
                var receivedTransactionCount = transactionCount - sentTransactionCount;

                allTabText.text = "All (" + (transactionCount > 999 ? "999+" : transactionCount.ToString()) + ")";
                sentTabText.text = "Sent (" + (sentTransactionCount > 999 ? "999+" : sentTransactionCount.ToString()) + ")";
                receivedTabText.text = "Received (" + (receivedTransactionCount > 999 ? "999+" : receivedTransactionCount.ToString()) + ")";
            }
        }

        private void TabChanged(int tabNum)
        {
            OnTabChanged?.Invoke((TabType)tabNum);
        }

        public enum TabType
        {
            All,
            Sent,
            Received
        }
    }
}