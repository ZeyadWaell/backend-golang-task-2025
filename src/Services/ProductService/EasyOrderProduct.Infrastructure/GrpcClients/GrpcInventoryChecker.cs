using EasyOrderProduct.Application.Contract.Interfaces.GrpsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Infrastructure.GrpcClients
{
    public class GrpcInventoryChecker : IInventoryCheckerService
    {
        private readonly InventoryService.InventoryServiceClient _client;

        public GrpcInventoryChecker(InventoryService.InventoryServiceClient client)
        {
            _client = client;
        }

        public async Task<bool> CheckAvailabilityAsync(Dictionary<int, int> productQuantities)
        {
            var request = new InventoryRequest
            {
                Products = { productQuantities.Select(p => new ProductQuantity
                {
                    ProductItemId = p.Key,
                    Quantity = p.Value
                }) }
            };

            var response = await _client.CheckAvailabilityAsync(request);
            return response.IsAvailable;
        }
    }
}
