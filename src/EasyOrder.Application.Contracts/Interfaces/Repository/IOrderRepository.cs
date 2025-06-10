using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Interfaces.Repository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
    }
}
