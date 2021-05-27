
namespace BoomaEcommerce.Services.DTO
{
    public class StoreManagementDto : BaseEntityDto
    {
        public StoreDto Store { get; set; }
        public UserDto User { get; set; }
        public StoreManagementPermissionsDto Permissions { get; set; }
    }
}
