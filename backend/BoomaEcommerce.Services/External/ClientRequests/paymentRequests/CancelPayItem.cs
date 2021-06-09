namespace BoomaEcommerce.Services.ClientRequests.paymentRequests
{
    public class CancelPayItem : TodoItem
    {
        private string transaction_id;

        public CancelPayItem(string actionType, string transactionId) : base(actionType)
        {
            transaction_id = transactionId;
        }
    }
}