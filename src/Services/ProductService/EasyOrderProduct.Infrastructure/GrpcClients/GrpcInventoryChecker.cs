// EasyOrderProduct.Infrastructure/Grpc/InventoryCheckerService.cs
using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contract;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using EasyOrderProduct.Application.Contracts.Protos;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;

public class InventoryCheckerService : InventoryChecker.InventoryCheckerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryCheckerService> _logger;

    public InventoryCheckerService(ILogger<InventoryCheckerService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
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
}
