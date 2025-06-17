using Castle.Core.Configuration;
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
        public ApplicationDbHandFireContext(DbContextOptions<ApplicationDbHandFireContext> options)
            : base(options)
        {
        }


    }
}
