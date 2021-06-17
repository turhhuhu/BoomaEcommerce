using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.String;

namespace BoomaEcommerce.Services.DTO
{
    public class BasicUserInfoDto : BaseEntityDto
    {
        public string UserName { get; set; } = Empty;
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
