using EasyOrderIdentity.Application.DTOs;
using EasyOrderIdentity.Application.DTOs.Responses;
using EasyOrderIdentity.Application.DTOs.Responses.Global;
using EasyOrderIdentity.Application.Interfaces;
using EasyOrderIdentity.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Application.Services
{
    namespace EasyOrderIdentity.Infrastructure.Services
    {
        public class AuthService : IAuthService
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly IConfiguration _configuration;
            private readonly IJWtHelper _jwtHelper;
            public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IJWtHelper jwtHelper)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _configuration = configuration;
                _jwtHelper = jwtHelper;
            }

            public async Task<BaseApiResponse> LoginAsync(LoginRequestDto request)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                    return ErrorResponse.Unauthorized("Invalid credentials");

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!result.Succeeded)
                    return ErrorResponse.Unauthorized("Invalid credentials");

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtHelper.GenerateToken(user, roles, _configuration);
                var data = new AuthTokenDto { Token = token };
                return new SuccessResponse<object>("Login successful", data);
            }

            public async Task<BaseApiResponse> CreateUserAsync(CreateUserRequestDto request)
            {
                var user = new ApplicationUser { UserName = request.Email, Email = request.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                    return ErrorResponse.BadRequest("User creation failed", string.Join(", ", result.Errors.Select(e => e.Description)));

                var role = request.Role ?? "User";
                await _userManager.AddToRoleAsync(user, role);
                var roles = new List<string> { role };
                var token = _jwtHelper.GenerateToken(user, roles, _configuration);
                var data = new AuthResultDto { UserId = user.Id, Email = user.Email, Roles = roles };
                return new SuccessResponse<object>("User created successfully", data, 201);
            }

            public async Task<BaseApiResponse> GetUserAsync(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return ErrorResponse.NotFound("User not found");

                var roles = await _userManager.GetRolesAsync(user);
                var data = new AuthResultDto { UserId = user.Id, Email = user.Email, Roles = roles };
                return new SuccessResponse<object>("User retrieved successfully", data);
            }

            public async Task<BaseApiResponse> UpdateUserAsync(string id, UpdateUserRequestDto request)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return ErrorResponse.NotFound("User not found");

                user.Email = request.Email ?? user.Email;
                user.NormalizedEmail = request.Email?.ToUpperInvariant() ?? user.NormalizedEmail;
                user.UserName = request.Email ?? user.UserName;
                user.NormalizedUserName = request.Email?.ToUpperInvariant() ?? user.NormalizedUserName;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return ErrorResponse.BadRequest("User update failed", string.Join(", ", updateResult.Errors.Select(e => e.Description)));

                if (!string.IsNullOrEmpty(request.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var pwdResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
                    if (!pwdResult.Succeeded)
                        return ErrorResponse.BadRequest("Password reset failed", string.Join(", ", pwdResult.Errors.Select(e => e.Description)));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var data = new AuthResultDto { UserId = user.Id, Email = user.Email, Roles = roles };
                return new SuccessResponse<object>("User updated successfully", data);
            }
        }
    }
}