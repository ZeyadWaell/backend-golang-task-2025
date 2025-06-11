using EasyOrderIdentity.Domain.Entites;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Application.Interfaces
{
    public interface IJWtHelper
    {
        string GenerateToken(ApplicationUser user, IList<string> roles, IConfiguration config);
    }
}
