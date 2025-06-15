using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Interfaces.GrpcServices
{
    public interface IInventoryChecker
    {
        Task<bool> CheckAvailabilityAsync(int productItemId);
        Task<bool> ReserveAsync(int productItemId, int qty);
        Task<bool> IncrementAsync(int productItemId, int qty);
    }
}
