using EasyOrderIdentity.Application.DTOs;
using EasyOrderIdentity.Application.DTOs.Responses.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<BaseApiResponse> LoginAsync(LoginRequestDto request);
        Task<BaseApiResponse> CreateUserAsync(CreateUserRequestDto request);
        Task<BaseApiResponse> GetUserAsync(string id);
        Task<BaseApiResponse> UpdateUserAsync(string id, UpdateUserRequestDto request);

    }
}
