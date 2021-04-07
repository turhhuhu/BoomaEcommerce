namespace BoomaEcommerce.Services.DTO
{
    public class StoreOwnershipDto : BaseEntityDto
    {
        public StoreDto Store { get; set; }
        public UserDto User { get; set; }
    }
}