namespace BoomaEcommerce.Services.External.Payment.Requests
{
    public class CancelPayRequest : BaseRequest
    {
        public string transaction_id;

        public CancelPayRequest(string actionType, string transactionId) : base(actionType)
        {
            transaction_id = transactionId;
        }
    }
}