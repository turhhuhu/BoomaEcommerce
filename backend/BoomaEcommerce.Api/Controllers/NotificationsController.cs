using BoomaEcommerce.Services;
using BoomaEcommerce.Services.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {

        public NotificationsController(INotificationHub notificationHub)
        {
        }

    }
}
