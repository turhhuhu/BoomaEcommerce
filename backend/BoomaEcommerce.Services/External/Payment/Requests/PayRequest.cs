namespace BoomaEcommerce.Services.External.Payment.Requests
{
    public class PayRequest : BaseRequest
    {
        public string card_number{ get; set; }
        public string month{ get; set; }
        public string year{ get; set; }
        public string holder{ get; set; }
        public string ccv{ get; set; }
        public string id{ get; set; }

        public PayRequest(string actionType, string cardNumber, string month, string year, string holder, string ccv, string id) : base(actionType)
        {
            card_number = cardNumber;
            this.month = month;
            this.year = year;
            this.holder = holder;
            this.ccv = ccv;
            this.id = id;
        }
            
    }
}