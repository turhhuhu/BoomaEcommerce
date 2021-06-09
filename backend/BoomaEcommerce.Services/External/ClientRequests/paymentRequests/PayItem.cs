namespace BoomaEcommerce.Services.ClientRequests.paymentRequests
{
    public class PayItem : TodoItem
    {
        private string card_number{ get; set; }
        private string month{ get; set; }
        private string year{ get; set; }
        private string holder{ get; set; }
        private string ccv{ get; set; }
        private string id{ get; set; }

        public PayItem(string actionType, string cardNumber, string month, string year, string holder, string ccv, string id) : base(actionType)
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