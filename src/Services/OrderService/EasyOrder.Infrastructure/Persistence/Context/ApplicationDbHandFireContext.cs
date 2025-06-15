using Castle.Core.Configuration;
using EasyOrderIdentity.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Persistence.Context
{
    public class ApplicationDbHandFireContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbHandFireContext(DbContextOptions<ApplicationDbHandFireContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

      //  public DbSet<JobLog> JobLog { get; set; }

    }
}
