public sealed partial class EthereumPendingTransactionManager
{
    public class PendingTransaction
    {
        public enum TransactionResult { Success, Failure };

        public bool isPending;

        public string addressFrom;
        public string txHash;
        public string message;

        public TransactionResult? result;

        public PendingTransaction(string addressFrom, string txHash, string message)
        {
            this.addressFrom = addressFrom;
            this.txHash = txHash;
            this.message = message;

            isPending = true;
        }
    }
}