using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomaEcommerce.Services.DTO
{
    public class AuthenticationResponse
    {
        public bool Success { get; set; } = false;
        public IList<string> Errors { get; set; }
        public string Token { get; set; }
    }
}
