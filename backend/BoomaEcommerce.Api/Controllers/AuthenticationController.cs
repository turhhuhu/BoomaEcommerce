using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Api.Requests;
using BoomaEcommerce.Api.Responses;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.Authentication;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost(ApiRoutes.Auth.Register)]
        public async Task<ActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var registrationRes = await _authService.RegisterAsync(request.UserInfo, request.Password);

            if (!registrationRes.Success)
            {
                return BadRequest(registrationRes.Errors);
            }

            return Ok(new SuccessAuthResponse
            {
                UserGuid = registrationRes.UserGuid,
                Token = registrationRes.Token,
                RefreshToken = registrationRes.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest request)
        {
            var loginRes = await _authService.LoginAsync(request.Username, request.Password);

            if (!loginRes.Success)
            {
                return BadRequest(loginRes.Errors);
            }

            return Ok(new SuccessAuthResponse
            {
                UserGuid = loginRes.UserGuid,
                Token = loginRes.Token,
                RefreshToken = loginRes.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Auth.Refresh)]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var refreshRes = await _authService.RefreshJwtToken(request.Token, request.RefreshToken);

            if (!refreshRes.Success)
            {
                return BadRequest(refreshRes.Errors);
            }

            return Ok(new SuccessAuthResponse
            {
                UserGuid = refreshRes.UserGuid,
                Token = refreshRes.Token,
                RefreshToken = refreshRes.RefreshToken
            });
        }
    }
}
