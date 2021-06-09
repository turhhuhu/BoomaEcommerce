namespace BoomaEcommerce.Services.External.Supply.Requests
{
    public class CancelSupplyRequest : BaseRequest
    {
        private string transaction_id;

        public CancelSupplyRequest(string actionType, string transactionId) : base(actionType)
        {
            transaction_id = transactionId;
        }
    }
}