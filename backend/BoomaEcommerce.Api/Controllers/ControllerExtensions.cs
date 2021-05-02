using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BoomaEcommerce.Api.Controllers
{
    public static class ControllerExtensions
    {
        public static string GetBaseUrl(this ControllerBase controller)
        {
            return $"{controller.HttpContext.Request.Scheme}://{controller.HttpContext.Request.Host.ToUriComponent()}";
        }
    }
}
