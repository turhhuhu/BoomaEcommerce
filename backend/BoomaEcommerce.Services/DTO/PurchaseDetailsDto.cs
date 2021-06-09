namespace BoomaEcommerce.Services.DTO
{
    public class PurchaseDetailsDto
    {
        public PurchaseDto Purchase { get; set; }
        public PaymentDetailsDto PaymentDetails { get; set; }
        public SupplyDetailsDto SupplyDetails { get; set; }
        
    }
}