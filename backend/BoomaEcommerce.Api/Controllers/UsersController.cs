using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersService _userService;

        public UsersController(ILogger<UsersController> logger, IUsersService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [Authorize]
        [HttpGet(ApiRoutes.Me)]
        public async Task<IActionResult> GetUserInfo()
        {
            var userGuid = User.GetUserGuid();
            var userInfo = await _userService.GetUserInfoAsync(userGuid);

            if (userInfo == null)
            {
                return NotFound();
            }
            return Ok(userInfo);
        }

    }
}
