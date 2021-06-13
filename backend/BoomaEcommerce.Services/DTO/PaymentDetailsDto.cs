namespace BoomaEcommerce.Services.DTO
{
    public class PaymentDetailsDto
    {
        public int CardNumber { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string HolderName { get; set; }
        public int Ccv { get; set; }
        public int Id { get; set; }
    }
}