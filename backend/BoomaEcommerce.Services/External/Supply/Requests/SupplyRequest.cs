namespace BoomaEcommerce.Services.External.Supply.Requests
{
    public class SupplyRequest : BaseRequest
    {
        private string name{ get; set; }
        private string address{ get; set; }
        private string city{ get; set; }
        private string country{ get; set; }
        private string zip{ get; set; }

        public SupplyRequest(string actionType, string name, string address, string city, string country, string zip) : base(actionType)
        {
            this.name = name;
            this.address = address;
            this.city = city;
            this.country = country;
            this.zip = zip;
        }
        
    }
}