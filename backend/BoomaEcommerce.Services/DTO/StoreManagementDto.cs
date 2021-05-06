
namespace BoomaEcommerce.Services.DTO
{
    public class StoreManagementDto : BaseEntityDto
    {
        public StoreDto Store { get; set; }
        public UserDto User { get; set; }
        public StoreManagementPermissionDto Permissions { get; set; }
    }
}
