using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.Interfaces.GrpsServices
{
    public interface IInventoryCheckerService
    {
        Task<bool> CheckAvailabilityAsync(Dictionary<int, int> productQuantities);
    }
}
