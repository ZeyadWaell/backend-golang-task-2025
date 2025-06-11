using EasyOrderIdentity.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Infrastructure.Services.Internal
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId
        {
            get
            {
                // Now check for ClaimTypes.NameIdentifier
                var userClaim = _httpContextAccessor.HttpContext?.User?.Claims
                                  .FirstOrDefault(x => x.Type == "uid");
                return userClaim?.Value;
            }
        }

        public string? UserRole
        {
            get
            {
                var roleClaim = _httpContextAccessor.HttpContext?.User?.Claims
                                  .FirstOrDefault(x => x.Type.Contains("role"));
                return roleClaim?.Value;
            }
        }
    }
}
