using EasyOrderIdentity.Api.Routes;
using EasyOrderIdentity.Application.DTOs;
using EasyOrderIdentity.Application.Interfaces;
using EasyOrderIdentity.Domain.Entites;
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
            public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
            {
                var response = await _authService.LoginAsync(request);
                return StatusCode(response.StatusCode, response);
            }

            [HttpPost(AuthRoutes.CreateUser)]
            public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
            {
                var response = await _authService.CreateUserAsync(request);
                return StatusCode(response.StatusCode, response);
            }

            [HttpGet(AuthRoutes.GetUser)]
            public async Task<IActionResult> GetUser([FromRoute] string id)
            {
                var response = await _authService.GetUserAsync(id);
                return StatusCode(response.StatusCode, response);
            }

            [HttpPut(AuthRoutes.UpdateUser)]
            public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserRequestDto request)
            {
                var response = await _authService.UpdateUserAsync(id, request);
                return StatusCode(response.StatusCode, response);
            }
        }
    }
}

