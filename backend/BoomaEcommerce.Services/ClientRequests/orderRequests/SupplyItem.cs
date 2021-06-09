namespace BoomaEcommerce.Services.ClientRequests.orderRequests
{
    public class SupplyItem : TodoItem
    {
        private string name{ get; set; }
        private string address{ get; set; }
        private string city{ get; set; }
        private string country{ get; set; }
        private string zip{ get; set; }

        public SupplyItem(string actionType, string name, string address, string city, string country, string zip) : base(actionType)
        {
            this.name = name;
            this.address = address;
            this.city = city;
            this.country = country;
            this.zip = zip;
        }
        
    }
}