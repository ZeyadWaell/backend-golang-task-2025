using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contract.Messaging;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Infrastructure.Messaging.Handlers
{
    // MessagingHandlers/ReleaseInventoryHandler.cs
    public class ReleaseInventoryHandler : IHandleMessages<ReleaseInventory>
    {
        private readonly IUnitOfWork _repo;
        public ReleaseInventoryHandler(IUnitOfWork repo) => _repo = repo;
        public async Task Handle(ReleaseInventory msg, IMessageHandlerContext context)
            => await _repo.InventoryRepository.IncrementAsync(msg.ProductItemId, msg.Quantity);

    }
}
