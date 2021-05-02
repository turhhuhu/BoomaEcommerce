using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Api.Responses
{
    public class SuccessAuthResponse
    {
        public string Token { get; set; }
        public Guid UserGuid { get; set; }
        public string RefreshToken { get; set; }
    }
}
