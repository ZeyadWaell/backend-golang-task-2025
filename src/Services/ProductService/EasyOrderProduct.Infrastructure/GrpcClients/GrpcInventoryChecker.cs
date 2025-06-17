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
        var reserved = await _unitOfWork.InventoryRepository
            .TryReserveAsync(request.ProductItemId, request.Quantity);

        if (!reserved)
        {
            throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                "Insufficient stock for reservation"
            ));
        }

        // 2) Fire-and-forget the SignalR notification
        _ = SafeNotifyClientsAsync(request);

        // 3) Return immediately
        return new InventoryResponse
        {
            IsAvailable = true,
            Message = "Inventory reserved successfully"
        };
    }
    public override async Task<InventoryResponse> ReserveInventory(QuantityRequest request,ServerCallContext context)
    {
        try
        {
            await EnsureProductExistsAsync(request.ProductItemId);
            var updated = await TryIncrementInventoryAsync(request.ProductItemId, request.Quantity);

            if (!updated)
                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "Unable to update inventory – record not found."));

            await SafeNotifyClientsAsync(request);

            return CreateSuccessResponse();
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ReserveInventory for ProductId={ProductId}", request.ProductItemId);
            throw new RpcException(new Status(
                StatusCode.Internal,
                "An unexpected error occurred while reserving inventory."));
        }
    }

    #region ─── Private Helpers ─────────────────────────────────────────────────────

    private async Task EnsureProductExistsAsync(int productItemId)
    {
        var exists = await _unitOfWork.ProductItemRepository
                                     .AnyAsync(x => x.Id == productItemId);

        if (!exists)
            throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Product item with ID {productItemId} not found"));
    }

    private async Task<bool> TryIncrementInventoryAsync(int productItemId, int qty)
    {
        var rows = await _unitOfWork.InventoryRepository
                                     .IncrementAsync(productItemId, qty);

        return rows;
    }

    private async Task SafeNotifyClientsAsync(QuantityRequest request)
    {
        try
        {
            await _hub.Clients
                      .Group(GetGroupName(request.ProductItemId))
                      .SendAsync("InventoryUpdated", new InventoryDto
                      {
                          ProductItemId = request.ProductItemId,
                          QuantityOnHand = request.Quantity,
                          WarehouseLocation = "Remote warehouse"
                      });
        }
        catch (Exception hubEx)
        {
            _logger.LogWarning(hubEx,
                "SignalR notification failed for ProductId={ProductId}",
                request.ProductItemId);
        }
    }

    private InventoryResponse CreateSuccessResponse()
        => new InventoryResponse
        {
            IsAvailable = true,
            Message = "Inventory incremented successfully"
        };

    private static string GetGroupName(int productItemId)
        => $"product_{productItemId}";

    #endregion

}
