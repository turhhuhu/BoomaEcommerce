namespace BoomaEcommerce.Services.DTO
{
    public class PaymentDetailsDto
    {
        public string CardNumber { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string HolderName { get; set; }
        public string Ccv { get; set; }
        public string Id { get; set; }
    }
}