using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contract.Messaging;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using MassTransit;
using NServiceBus;
using Ocelot.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Infrastructure.Messaging.Handlers
{
    public class ReserveInventoryHandler : IHandleMessages<ReserveInventory>
    {
        private readonly IUnitOfWork _repo;
        private readonly IBus _bus;
        public ReserveInventoryHandler(IUnitOfWork repo, IBus bus)
        {
            _repo = repo; _bus = bus;
        }
        public async Task Handle(ReserveInventory msg,IMessageHandlerContext context)
        {
            var ok = await _repo.InventoryRepository.TryReserveAsync(msg.ProductItemId, msg.Quantity);
            if (ok) await _bus.Publish(new InventoryReserved(msg.OrderId));
            else await _bus.Publish(new InventoryNotAvailable(msg.OrderId));
        }


    }
}
