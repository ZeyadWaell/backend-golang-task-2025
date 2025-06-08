using EasyOrder.Application.Contracts.InterfaceCommon;
using EasyOrder.Domain.Entities;
using EasyOrder.Infrastructure.Persistence.Context;
using EasyOrder.Infrastructure.Persistence.Repositories.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Persistence.Repositories
{
    public class OrderRepository: GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ReadDbContext readContext,WriteDbContext writeContext): base(readContext, writeContext)
        {
        }

    }
}
