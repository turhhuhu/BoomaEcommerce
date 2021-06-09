namespace BoomaEcommerce.Services.ClientRequests.orderRequests
{
    public class CancelSupplyItem : TodoItem
    {
        private string transaction_id;

        public CancelSupplyItem(string actionType, string transactionId) : base(actionType)
        {
            transaction_id = transactionId;
        }
    }
}