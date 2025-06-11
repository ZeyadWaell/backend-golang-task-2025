using EasyOrderIdentity.Api.Routes;
using EasyOrderIdentity.Application.DTOs;
using EasyOrderIdentity.Application.Interfaces;
using EasyOrderIdentity.Domain.Entites;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyOrderIdentity.Api.Controllers
{
    namespace EasyOrderIdentity.Api.Controllers
    {
        [ApiController]
        public class AuthController : ControllerBase
        {
            private readonly IAuthService _authService;

            public AuthController(IAuthService authService)
            {
                _authService = authService;
            }

            [HttpPost(AuthRoutes.Login)]
            [AllowAnonymous]
            public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
            {
                var response = await _authService.LoginAsync(request);
                return StatusCode(response.StatusCode, response);
            }

            [HttpPost(AuthRoutes.CreateUser)]
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
            public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
            {
                var response = await _authService.CreateUserAsync(request);
                return StatusCode(response.StatusCode, response);
            }

            [HttpGet(AuthRoutes.GetUser)]
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            public async Task<IActionResult> GetUser([FromRoute] string id)
            {
                var response = await _authService.GetUserAsync(id);
                return StatusCode(response.StatusCode, response);
            }

            [HttpPut(AuthRoutes.UpdateUser)]
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserRequestDto request)
            {
                var response = await _authService.UpdateUserAsync(id, request);
                return StatusCode(response.StatusCode, response);
            }
        }
    }
}

