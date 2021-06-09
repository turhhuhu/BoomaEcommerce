namespace BoomaEcommerce.Services.External.Supply.Requests
{
    public class SupplyRequest : BaseRequest
    {
        public string name{ get; set; }
        public string address{ get; set; }
        public string city{ get; set; }
        public string country{ get; set; }
        public string zip{ get; set; }

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