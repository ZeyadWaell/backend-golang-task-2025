using EasyOrderProduct.Application.Contracts.Interfaces;
using EasyOrderProduct.Domain.Entities;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using EasyOrderProduct.Infrastructure.Persistence.Repositories.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Infrastructure.Persistence.Repositories
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ReadDbContext readContext, WriteDbContext writeContext) : base(readContext, writeContext)
        {
        }
    }
}
