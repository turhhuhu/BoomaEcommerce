using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api.Requests
{
    public class UserRegistrationRequest
    {
        [Required]
        public UserDto UserInfo { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
