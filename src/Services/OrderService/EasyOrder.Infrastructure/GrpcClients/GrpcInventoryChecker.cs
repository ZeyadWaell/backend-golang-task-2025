using EasyOrder.Application.Contracts.Interfaces.GrpcServices;
using EasyOrderProduct.Application.Contracts.Protos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.GrpcClients
{
    public class GrpcInventoryChecker : IInventoryChecker
    {
        private readonly InventoryChecker.InventoryCheckerClient _client;
        private readonly ILogger<GrpcInventoryChecker> _logger;

        public GrpcInventoryChecker(InventoryChecker.InventoryCheckerClient client, ILogger<GrpcInventoryChecker> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<bool> CheckAvailabilityAsync(int productItemId)
        {
            var req = new InventoryRequest { ProductItemId = productItemId };
            var res = await _client.CheckAvailabilityAsync(req);
            return res.IsAvailable;
        }
        public async Task<bool> ReserveAsync(int productItemId, int qty)
        {
            var req = new QuantityRequest
            {
                ProductItemId = productItemId,
                Quantity = qty
            };

            var res = await _client.ReserveInventoryAsync(req);
            return res.IsAvailable;
        }
        public async Task<bool> IncrementAsync(int productItemId, int qty)
        {
            var req = new QuantityRequest
            {
                ProductItemId = productItemId,
                Quantity = qty
            };
            var res = await _client.ReleaseInventoryAsync(req);
            return res.IsAvailable;
        }
    }
}
