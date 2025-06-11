using EasyOrderIdentity.Infrastructure.Seed.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Infrastructure.Extentions
{
   public static class AppExtensions
    {
        public static void SeedIdentity(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
            seeder.SeedAsync().GetAwaiter().GetResult();
        }
    }
}
