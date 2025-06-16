// EasyOrderProduct.Infrastructure/Grpc/InventoryCheckerService.cs
using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contract;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using EasyOrderProduct.Application.Contracts.Protos;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Application.Contract.Hubs;
using Microsoft.AspNetCore.SignalR;
using EasyOrderProduct.Application.Contract.DTOs.Responses;
using EasyOrderProduct.Domain.Entities;

public class InventoryCheckerService : InventoryChecker.InventoryCheckerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryCheckerService> _logger;
    private readonly IHubContext<InventoryHub> _hub;

    public InventoryCheckerService(ILogger<InventoryCheckerService> logger, IUnitOfWork unitOfWork, IHubContext<InventoryHub> hub)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _hub = hub;
    }

    public override async Task<InventoryResponse> CheckAvailability(InventoryRequest request,ServerCallContext context)
    {
        var productItemCheck = await _unitOfWork.ProductItemRepository.AnyAsync(x => x.Id == request.ProductItemId);

        if(!productItemCheck)
            throw new RpcException(new Status(StatusCode.NotFound, $"Product item with ID {request.ProductItemId} not found"));
    
        var available = await _unitOfWork.ProductItemRepository.CheckAvailabilityAsync(request.ProductItemId);

        return new InventoryResponse
        {
            IsAvailable = available,
            Message = available ? "In stock" : "Out of stock"
        };
    }
    public override async Task<InventoryResponse> ReleaseInventory(QuantityRequest request, ServerCallContext context)
    {
        var productItemCheck = await _unitOfWork.ProductItemRepository.AnyAsync(x => x.Id == request.ProductItemId);
        if (!productItemCheck)
            throw new RpcException(new Status(StatusCode.NotFound, $"Product item with ID {request.ProductItemId} not found"));
        var reserved = await _unitOfWork.InventoryRepository.TryReserveAsync(request.ProductItemId, request.Quantity);
        return new InventoryResponse
        {
            IsAvailable = reserved,
            Message = reserved ? "Reservation successful" : "Insufficient stock for reservation"
        };
    }
    public override async Task<InventoryResponse> ReserveInventory(QuantityRequest request, ServerCallContext context)
    {
        var productItemCheck = await _unitOfWork.ProductItemRepository.AnyAsync(x => x.Id == request.ProductItemId);
        if (!productItemCheck)
            throw new RpcException(new Status(StatusCode.NotFound, $"Product item with ID {request.ProductItemId} not found"));

        await _unitOfWork.InventoryRepository.IncrementAsync(request.ProductItemId, request.Quantity);

        await _hub.Clients.Group($"product_{request.ProductItemId}")
                .SendAsync("InventoryUpdated", new InventoryDto
                {
                    ProductItemId = request.ProductItemId,
                    QuantityOnHand = request.Quantity,
                    WarehouseLocation = "Remote work i hope in Easy Order "
                });
        return new InventoryResponse
        {
            IsAvailable = true,
            Message = "Inventory incremented successfully"
        };
    }
}
