using EasyOrderProduct.Application.Contracts.Protos;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.Interfaces.GrpsServices
{
    public interface IInventoryChecker
    {
        Task<InventoryResponse> CheckAvailability(InventoryRequest request, ServerCallContext context);
    }
}
